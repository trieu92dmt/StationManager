using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class PurchaseOrderIntegrationNSCommand : IRequest<bool>
    {
        public string Plant { get; set; }
        public string PurchasingOrganization { get; set; }
        public string PurchasingGroup { get; set; }
        public string Vendor { get; set; }
        public string POType { get; set; }
        public string PurchaseOrder { get; set; }
        public string Material { get; set; }
        public DateTime? DocumentDate { get; set; }
        public List<PurchaseOrderDetailIntegration> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetailIntegration>();
    }

    public class PurchaseOrderDetailIntegration
    {
        public string PurchaseOrder { get; set; }
        public string PurchaseOrderItem { get; set; }
        public string Material { get; set; }
        public string StorageLocation { get; set; }
        public string Batch { get; set; }
        public string VehicleCode { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? OpenQuantity { get; set; }
        public string UoM { get; set; }

    }
    public class PurchaseOrderIntegrationNSCommandHandler : IRequestHandler<PurchaseOrderIntegrationNSCommand, bool>
    {
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IISDUnitOfWork _unitOfWork;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;

        public PurchaseOrderIntegrationNSCommandHandler(IRepository<PurchaseOrderMasterModel> poRep, IISDUnitOfWork unitOfWork, IRepository<PurchaseOrderDetailModel> poDetailRep)
        {
            _poRep = poRep;
            _unitOfWork = unitOfWork;
            _poDetailRep = poDetailRep;
        }
        public async Task<bool> Handle(PurchaseOrderIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            //Purchase Order
            var purchaseOrder = await _poRep.GetQuery(x => x.PurchaseOrderCode == request.PurchaseOrder)
                                            .Include(x => x.PurchaseOrderDetailModel)
                                            .FirstOrDefaultAsync();

            //Không có tạo mới
            if (purchaseOrder is null)
            {
                //Hearder 
                purchaseOrder = new PurchaseOrderMasterModel();
                purchaseOrder.PurchaseOrderId = Guid.NewGuid();
                purchaseOrder.PurchaseOrderCode = request.PurchaseOrder;
                purchaseOrder.PurchaseOrderCodeInt = int.Parse(request.PurchaseOrder);
                purchaseOrder.POType = request.POType;
                purchaseOrder.Plant = request.Plant;
                purchaseOrder.PurchasingOrg = request.PurchasingOrganization;
                purchaseOrder.PurchasingGroup = request.PurchasingGroup;
                purchaseOrder.VendorCode = request.Vendor;
                purchaseOrder.VendorCodeInt = int.Parse(request.Vendor);
                purchaseOrder.DocumentDate = request.DocumentDate;

                purchaseOrder.CreateTime = DateTime.Now;
                purchaseOrder.Actived = true;

                //Detail
                var detailPOs = request.PurchaseOrderDetails.Select(x => new PurchaseOrderDetailModel
                {
                    PurchaseOrderDetailId = Guid.NewGuid(),
                    PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                    POLine = x.PurchaseOrderItem,
                    ProductCode = x.Material,
                    VehicleCode = x.VehicleCode,
                    OrderQuantity = x.OrderQuantity,
                    OpenQuantity = x.OpenQuantity,
                    Unit = x.UoM,

                    CreateTime = DateTime.Now,
                    Actived = true

                }).ToList();

                purchaseOrder.PurchaseOrderDetailModel = detailPOs;
                _poRep.Add(purchaseOrder);
            }
            else
            {
                #region Master
                //Cập nhật master
                purchaseOrder.POType = request.POType;
                purchaseOrder.Plant = request.Plant;
                purchaseOrder.PurchasingOrg = request.PurchasingOrganization;
                purchaseOrder.PurchasingGroup = request.PurchasingGroup;
                purchaseOrder.VendorCode = request.Vendor;
                purchaseOrder.DocumentDate = request.DocumentDate;
                purchaseOrder.LastEditTime = DateTime.Now;

                #endregion

                #region Detail
                //Cập nhật detail
                foreach (var item in request.PurchaseOrderDetails)
                {
                    var detailPO = await _poDetailRep.FindOneAsync(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId && x.POLine == item.PurchaseOrderItem);
                    if (detailPO == null)
                    {

                        _poDetailRep.Add(new PurchaseOrderDetailModel
                        {
                            PurchaseOrderDetailId = Guid.NewGuid(),
                            PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                            POLine = item.PurchaseOrderItem,
                            ProductCode = item.Material,
                            Batch = item.Batch,
                            VehicleCode = item.VehicleCode,
                            OrderQuantity = item.OrderQuantity,
                            OpenQuantity = item.OpenQuantity,

                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        detailPO.ProductCode = item.Material;
                        detailPO.Batch = item.Batch;
                        detailPO.VehicleCode = item.VehicleCode;
                        detailPO.OrderQuantity = item.OrderQuantity;
                        detailPO.OpenQuantity = item.OpenQuantity;
                        detailPO.LastEditTime = DateTime.Now;
                    }
                }
                #endregion
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
