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

namespace IntegrationNS.Application.Commands.XNVLGCs
{
    public class UpdateAndCancelXNVLGCCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXNVLGC> XNVLGCs { get; set; } = new List<UpdateAndCancelXNVLGC>();
    }

    public class UpdateAndCancelXNVLGC
    {
        public Guid XnvlgcId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXNVLGCCommandHandler : IRequestHandler<UpdateAndCancelXNVLGCCommand, bool>
    {
        private readonly IRepository<ComponentExportModel> _xnvlgcRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailOutboundDeliveryModel> _obDetailRepo;
        private readonly IRepository<DetailReservationModel> _dtResRepo;

        public UpdateAndCancelXNVLGCCommandHandler(IRepository<ComponentExportModel> xnvlgcRep, IUnitOfWork unitOfWork,
                                                 IRepository<DetailOutboundDeliveryModel> obDetailRepo, IRepository<DetailReservationModel> dtResRepo)
        {
            _xnvlgcRep = xnvlgcRep;
            _unitOfWork = unitOfWork;
            _obDetailRepo = obDetailRepo;
            _dtResRepo = dtResRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXNVLGCCommand request, CancellationToken cancellationToken)
        {
            //Get query reservation
            var reservations = _dtResRepo.GetQuery().AsNoTracking();
            if (request.IsCancel == true)
            {

                if (!request.XNVLGCs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất nguyên vật liệu gia công");

                foreach (var item in request.XNVLGCs)
                {
                    //Phiếu nhập kho dcnb
                    var xnvlgc = await _xnvlgcRep.FindOneAsync(x => x.ComponentExportId == item.XnvlgcId);

                    //Check
                    if (xnvlgc is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất nguyên vật liệu gia công");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xnvlgc.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xnvlgc.MaterialDocument))//) && string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                        xnvlgc.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                    //    xnvlgc.Status = "NOT";
                    xnvlgc.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xnvlgc);
                    var xnvlgcNew = JsonConvert.DeserializeObject<ComponentExportModel>(serialized);

                    xnvlgcNew.ComponentExportId = Guid.NewGuid();
                    xnvlgcNew.Status = "NOT";
                    xnvlgcNew.TotalQuantity = 0;
                    xnvlgcNew.RequirementQuantity = 0;
                    xnvlgcNew.MaterialDocument = null;
                    xnvlgcNew.ReverseDocument = null;

                    _xnvlgcRep.Add(xnvlgcNew);
                }
            }
            else
            {

                if (!request.XNVLGCs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XNVLGC");

                foreach (var item in request.XNVLGCs)
                {
                    //Phiếu xuất nguyên vật liệu gia công
                    var xnvlgc = await _xnvlgcRep.GetQuery().Include(x => x.PurchaseOrderDetail).ThenInclude(x => x.PurchaseOrder).FirstOrDefaultAsync(x => x.ComponentExportId == item.XnvlgcId);

                    //Check
                    if (xnvlgc is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất nguyên vật liệu gia công");

                    //Cập nhật Batch và MaterialDocument
                    xnvlgc.Batch = item.Batch;
                    xnvlgc.MaterialDocument = item.MaterialDocument;
                    xnvlgc.TotalQuantity = xnvlgc.PurchaseOrderDetailId.HasValue ? xnvlgc.PurchaseOrderDetail.OrderQuantity : 0;
                    xnvlgc.RequirementQuantity = xnvlgc.PurchaseOrderDetailId.HasValue ? reservations.FirstOrDefault(x => x.PurchasingDoc == xnvlgc.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode &&
                                                                                                                          x.Item == xnvlgc.PurchaseOrderDetail.POLine).RequirementQty : 0;
                    if (!string.IsNullOrEmpty(xnvlgc.MaterialDocument))// && string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                        xnvlgc.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xnvlgc.ReverseDocument))
                    //    xnvlgc.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
