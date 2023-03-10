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

namespace IntegrationNS.Application.Commands.XKLXH
{
    public class UpdateAndCancelXKLXHCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXKLXH> XKLXHs { get; set; } = new List<UpdateAndCancelXKLXH>();
    }

    public class UpdateAndCancelXKLXH
    {
        public Guid XklxhId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXKLXHCommandHandler : IRequestHandler<UpdateAndCancelXKLXHCommand, bool>
    {
        private readonly IRepository<ExportByCommandModel> _xklxhRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailOutboundDeliveryModel> _obDetailRepo;
        private readonly IRepository<AccountModel> _userRepo;

        public UpdateAndCancelXKLXHCommandHandler(IRepository<ExportByCommandModel> xklxhRep, IUnitOfWork unitOfWork, IRepository<AccountModel> userRepo,
                                                  IRepository<DetailOutboundDeliveryModel> obDetailRepo)
        {
            _xklxhRep = xklxhRep;
            _unitOfWork = unitOfWork;
            _obDetailRepo = obDetailRepo;
            _userRepo = userRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXKLXHCommand request, CancellationToken cancellationToken)
        {
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            if (request.IsCancel == true)
            {

                if (!request.XKLXHs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất kho theo lệnh xuất hàng");

                foreach (var item in request.XKLXHs)
                {
                    //Phiếu xuất kho theo lệnh xuất hàng
                    var xklxh = await _xklxhRep.FindOneAsync(x => x.ExportByCommandId == item.XklxhId);

                    //Check
                    if (xklxh is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất kho theo lệnh xuất hàng");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xklxh.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xklxh.MaterialDocument))
                        xklxh.Status = "POST";
                    xklxh.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xklxh);
                    var xklxhNew = JsonConvert.DeserializeObject<ExportByCommandModel>(serialized);

                    //Chứng từ
                    var document = await _obDetailRepo.FindOneAsync(x => x.DetailOutboundDeliveryId == xklxh.DetailODId);

                    xklxhNew.ExportByCommandId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    xklxhNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    xklxhNew.LastEditBy = null;
                    xklxhNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    xklxhNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    xklxhNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    xklxhNew.Status = "NOT";
                    xklxhNew.TotalQuantity = 0;
                    xklxhNew.DeliveryQuantity = 0;
                    xklxhNew.OpenQuantity = 0;
                    xklxhNew.MaterialDocument = null;
                    xklxhNew.ReverseDocument = null;

                    _xklxhRep.Add(xklxhNew);
                }
            }
            else
            {

                if (!request.XKLXHs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XKLXH");

                foreach (var item in request.XKLXHs)
                {
                    //Phiếu xuất kho theo lệnh xuất hàng
                    var xklxh = await _xklxhRep.GetQuery().Include(x => x.DetailOD).FirstOrDefaultAsync(x => x.ExportByCommandId == item.XklxhId);

                    //Check
                    if (xklxh is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất kho theo lệnh xuất hàng");

                    //Cập nhật Batch và MaterialDocument
                    xklxh.Batch = item.Batch;
                    xklxh.MaterialDocument = item.MaterialDocument;
                    xklxh.TotalQuantity = xklxh.DetailODId.HasValue ? xklxh.DetailOD.DeliveryQuantity : 0;
                    xklxh.DeliveryQuantity = xklxh.DetailODId.HasValue ? xklxh.DetailOD.PickedQuantityPUoM : 0;
                    xklxh.OpenQuantity = xklxh.TotalQuantity - xklxh.OpenQuantity;
                    if (!string.IsNullOrEmpty(xklxh.MaterialDocument))
                        xklxh.Status = "POST";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
