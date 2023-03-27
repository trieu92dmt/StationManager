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
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.XKs
{
    public class UpdateAndCancelXKCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXK> XKs { get; set; } = new List<UpdateAndCancelXK>();
    }

    public class UpdateAndCancelXK
    {
        public Guid XkId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string MaterialDocumentItem { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXKCommandHandler : IRequestHandler<UpdateAndCancelXKCommand, bool>
    {
        private readonly IRepository<OtherExportModel> _xkRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<DetailReservationModel> _resRepo;

        public UpdateAndCancelXKCommandHandler(IRepository<OtherExportModel> xkRep, IUnitOfWork unitOfWork, IRepository<AccountModel> userRepo,
                                               IRepository<DetailReservationModel> resRepo)
        {
            _xkRep = xkRep;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _resRepo = resRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXKCommand request, CancellationToken cancellationToken)
        {
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            if (request.IsCancel == true)
            {
                //Get query chứng từ
                var documentQuery = _resRepo.GetQuery().AsNoTracking();

                if (!request.XKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất khác");

                foreach (var item in request.XKs)
                {
                    //Phiếu xuất khác
                    var xk = await _xkRep.FindOneAsync(x => x.OtherExportId == item.XkId);

                    //Check
                    if (xk is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất khác");

                    //Nếu đã reverse thì không reverse nữa
                    if (!string.IsNullOrEmpty(xk.ReverseDocument))
                    {
                        throw new ISDException(CommonResource.Msg_Canceled, $"Phiếu nhập kho {item.XkId}");
                    }

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xk.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xk.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xk.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                    xk.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xk);
                    var xkNew = JsonConvert.DeserializeObject<OtherExportModel>(serialized);

                    //Chứng từ
                    var document = documentQuery.FirstOrDefault(x => x.DetailReservationId == xk.DetailReservationId);

                    xkNew.OtherExportId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    xkNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    xkNew.LastEditBy = null;
                    xkNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    xkNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    xkNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    xkNew.Status = "NOT";
                    xkNew.TotalQuantity = 0;
                    xkNew.DeliveredQuantity = 0;
                    xkNew.OpenQuantity = 0;
                    xkNew.MaterialDocument = null;
                    xkNew.ReverseDocument = null;

                    _xkRep.Add(xkNew);
                }
            }
            else
            {

                if (!request.XKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XK");

                foreach (var item in request.XKs)
                {
                    //Phiếu xuất khác
                    var xk = await _xkRep.GetQuery().Include(x => x.DetailReservation).FirstOrDefaultAsync(x => x.OtherExportId == item.XkId);

                    //Check
                    if (xk is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất khác");

                    //Cập nhật Batch và MaterialDocument
                    xk.Batch = item.Batch;
                    xk.TotalQuantity = xk.DetailReservation.RequirementQty;
                    xk.DeliveredQuantity = xk.DetailReservation.QtyWithdrawn;
                    xk.OpenQuantity = xk.TotalQuantity - xk.OpenQuantity;
                    xk.MaterialDocument = item.MaterialDocument;
                    xk.MaterialDocumentItem = item.MaterialDocumentItem;
                    if (!string.IsNullOrEmpty(xk.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xk.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
