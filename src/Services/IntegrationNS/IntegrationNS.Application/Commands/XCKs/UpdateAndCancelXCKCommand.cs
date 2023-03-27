using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.XCKs
{
    public class UpdateAndCancelXCKCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXCK> XCKs { get; set; } = new List<UpdateAndCancelXCK>();
    }

    public class UpdateAndCancelXCK
    {
        public Guid XckId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXCKCommandHandler : IRequestHandler<UpdateAndCancelXCKCommand, bool>
    {
        private readonly IRepository<WarehouseExportTransferModel> _xckRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<DetailReservationModel> _dtResRepo;

        public UpdateAndCancelXCKCommandHandler(IRepository<WarehouseExportTransferModel> xckRep, IUnitOfWork unitOfWork, IRepository<AccountModel> userRepo, 
                                                IRepository<DetailReservationModel> dtResRepo)
        {
            _xckRep = xckRep;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _dtResRepo = dtResRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXCKCommand request, CancellationToken cancellationToken)
        {
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            if (request.IsCancel == true)
            {
                //Get query chứng từ
                var documentQuery = _dtResRepo.GetQuery().AsNoTracking();


                if (!request.XCKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất chuyển kho");

                foreach (var item in request.XCKs)
                {
                    //Phiếu xuất chuyển kho
                    var xck = await _xckRep.FindOneAsync(x => x.WarehouseTransferId == item.XckId);

                    //Check
                    if (xck is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất chuyển kho");

                    //Nếu đã reverse thì không reverse nữa
                    if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    {
                        throw new ISDException(CommonResource.Msg_Canceled, $"Phiếu nhập kho {item.XckId}");
                    }

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xck.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xck.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xck.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                    xck.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xck);
                    var xckNew = JsonConvert.DeserializeObject<WarehouseExportTransferModel>(serialized);

                    //Chứng từ
                    var document = documentQuery.FirstOrDefault(x => x.DetailReservationId == xck.DetailReservationId);

                    xckNew.WarehouseTransferId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    xckNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    xckNew.LastEditBy = null;
                    xckNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    xckNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    xckNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    xckNew.Status = "NOT";
                    xckNew.TotalQuantity = 0;
                    xckNew.DeliveredQuantity = 0;
                    xckNew.OpenQuantity = 0;
                    xckNew.MaterialDocument = null;
                    xckNew.ReverseDocument = null;

                    _xckRep.Add(xckNew);
                }
            }
            else
            {

                if (!request.XCKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XCK");

                foreach (var item in request.XCKs)
                {
                    //Phiếu xuất chuyển kho
                    var xck = await _xckRep.GetQuery().Include(x => x.DetailReservation).FirstOrDefaultAsync(x => x.WarehouseTransferId == item.XckId);

                    //Check
                    if (xck is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất chuyển kho");

                    //Cập nhật Batch và MaterialDocument
                    xck.Batch = item.Batch;
                    xck.MaterialDocument = item.MaterialDocument;
                    xck.TotalQuantity = xck.DetailReservationId.HasValue ? xck.DetailReservation.RequirementQty : 0;
                    xck.DeliveredQuantity = xck.DetailReservationId.HasValue ? xck.DetailReservation.QtyWithdrawn : 0;
                    xck.OpenQuantity = xck.TotalQuantity - xck.DeliveredQuantity;
                    if (!string.IsNullOrEmpty(xck.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xck.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
