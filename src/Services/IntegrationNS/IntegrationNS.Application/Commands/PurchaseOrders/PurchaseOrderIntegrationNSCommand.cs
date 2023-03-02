using IntegrationNS.Application.DTOs;
using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationNS.Application.Commands.PurchaseOrders
{
    public class PurchaseOrderIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<PurchaseOrderIntegration> PurchaseOrders { get; set; } = new List<PurchaseOrderIntegration>();
    }

    public class PurchaseOrderIntegration
    {
        public string Plant { get; set; }
        public string PurchasingOrganization { get; set; }
        public string PurchasingGroup { get; set; }
        public string Vendor { get; set; }
        public string POType { get; set; }
        public string PurchaseOrder { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string ReleaseIndicator { get; set; }
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
        public decimal? QuantityReceived { get; set; }
        public string DeletionInd { get; set; }
        public string Deliver { get; set; }
        public string VehicleOwner { get; set; }
        public string TransportUnit { get; set; }
        public string DeliveryCompleted { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public string WeightUnit { get; set; }
    }
    public class PurchaseOrderIntegrationNSCommandHandler : IRequestHandler<PurchaseOrderIntegrationNSCommand, IntegrationNSResponse>
    {
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;
        private readonly IRepository<ProductModel> _prdRepo;

        public PurchaseOrderIntegrationNSCommandHandler(IRepository<PurchaseOrderMasterModel> poRep, IUnitOfWork unitOfWork, IRepository<PurchaseOrderDetailModel> poDetailRep,
                                                        IRepository<ProductModel> prdRepo)
        {
            _poRep = poRep;
            _unitOfWork = unitOfWork;
            _poDetailRep = poDetailRep;
            _prdRepo = prdRepo;
        }
        public async Task<IntegrationNSResponse> Handle(PurchaseOrderIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.PurchaseOrders.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.PurchaseOrders.Count();

            //Dữ liệu material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            foreach (var poIntegration in request.PurchaseOrders)
            {
                try
                {
                    //Purchase Order
                    var purchaseOrder = await _poRep.GetQuery(x => x.PurchaseOrderCode == poIntegration.PurchaseOrder)
                                                    .Include(x => x.PurchaseOrderDetailModel)
                                                    .FirstOrDefaultAsync();

                    //Không có tạo mới
                    if (purchaseOrder is null)
                    {
                        //Hearder 
                        purchaseOrder = new PurchaseOrderMasterModel();
                        purchaseOrder.PurchaseOrderId = Guid.NewGuid();
                        purchaseOrder.PurchaseOrderCode = poIntegration.PurchaseOrder;
                        purchaseOrder.PurchaseOrderCodeInt = !poIntegration.PurchaseOrder.IsNullOrEmpty() ? long.Parse(poIntegration.PurchaseOrder) : null;
                        purchaseOrder.POType = poIntegration.POType;
                        purchaseOrder.Plant = poIntegration.Plant;
                        purchaseOrder.PurchasingOrg = poIntegration.PurchasingOrganization;
                        purchaseOrder.PurchasingGroup = poIntegration.PurchasingGroup;
                        purchaseOrder.VendorCode = poIntegration.Vendor;
                        //purchaseOrder.VendorCodeInt = int.Parse(poIntegration.Vendor);
                        purchaseOrder.DocumentDate = poIntegration.DocumentDate;
                        purchaseOrder.ReleaseIndicator = poIntegration.ReleaseIndicator;

                        purchaseOrder.CreateTime = DateTime.Now;
                        purchaseOrder.Actived = true;

                        //Detail
                        var detailPOs = new List<PurchaseOrderDetailModel>();
                        foreach (var item in poIntegration.PurchaseOrderDetails)
                        {
                            if (materials.FirstOrDefault(x => x.ProductCode == item.Material) == null)
                                throw new ISDException(String.Format(CommonResource.Msg_NotFound, "Material"));

                            detailPOs.Add(new PurchaseOrderDetailModel
                            {
                                PurchaseOrderDetailId = Guid.NewGuid(),
                                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                                POLine = item.PurchaseOrderItem,
                                PoLinetInt = int.Parse(item.PurchaseOrderItem),
                                ProductCode = item.Material,
                                ProductCodeInt = long.Parse(item.Material),
                                OrderQuantity = item.OrderQuantity ?? 0,
                                OpenQuantity = item.OpenQuantity ?? 0,
                                StorageLocation = item.StorageLocation,
                                Batch = item.Batch,
                                Unit = item.UoM,
                                CreateTime = DateTime.Now,
                                Actived = true,
                                QuantityReceived = item.QuantityReceived ?? 0,
                                DeletionInd = item.DeletionInd,
                                Deliver = item.Deliver,
                                VehicleCode = item.VehicleCode,
                                VehicleOwner = item.VehicleOwner,
                                TransportUnit = item.TransportUnit,
                                DeliveryCompleted = item.DeliveryCompleted,
                                GrossWeight = item.GrossWeight,
                                NetWeight = item.NetWeight,
                                WeightUnit = item.WeightUnit  
                            });

                        }
                        //var detailPOs = poIntegration.PurchaseOrderDetails.Select(x => new PurchaseOrderDetailModel
                        //{
                        //    PurchaseOrderDetailId = Guid.NewGuid(),
                        //    PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                        //    POLine = x.PurchaseOrderItem,
                        //    ProductCode = x.Material,
                        //    OrderQuantity = x.OrderQuantity,
                        //    OpenQuantity = x.OpenQuantity,
                        //    Unit = x.UoM,

                        //    CreateTime = DateTime.Now,
                        //    Actived = true

                        //}).ToList();

                        purchaseOrder.PurchaseOrderDetailModel = detailPOs;
                        _poRep.Add(purchaseOrder);
                    }
                    else
                    {
                        #region Master
                        //Cập nhật master
                        purchaseOrder.POType = poIntegration.POType;
                        purchaseOrder.Plant = poIntegration.Plant;
                        purchaseOrder.PurchasingOrg = poIntegration.PurchasingOrganization;
                        purchaseOrder.PurchasingGroup = poIntegration.PurchasingGroup;
                        purchaseOrder.VendorCode = poIntegration.Vendor;
                        purchaseOrder.DocumentDate = poIntegration.DocumentDate;
                        purchaseOrder.LastEditTime = DateTime.Now;

                        #endregion

                        #region Detail
                        //Cập nhật detail
                        foreach (var item in poIntegration.PurchaseOrderDetails)
                        {
                            var detailPO = await _poDetailRep.FindOneAsync(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId && x.POLine == item.PurchaseOrderItem);
                            if (detailPO == null)
                            {

                                _poDetailRep.Add(new PurchaseOrderDetailModel
                                {
                                    PurchaseOrderDetailId = Guid.NewGuid(),
                                    PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                                    POLine = item.PurchaseOrderItem,
                                    PoLinetInt = int.Parse(item.PurchaseOrderItem),
                                    ProductCode = item.Material,
                                    ProductCodeInt = long.Parse(item.Material),
                                    Batch = item.Batch,
                                    OrderQuantity = item.OrderQuantity ?? 0,
                                    OpenQuantity = item.OpenQuantity ?? 0,
                                    StorageLocation = item.StorageLocation,
                                    CreateTime = DateTime.Now,
                                    Unit = item.UoM,
                                    Actived = true,
                                    QuantityReceived = item.QuantityReceived ?? 0,
                                    DeletionInd = item.DeletionInd,
                                    Deliver = item.Deliver,
                                    VehicleCode = item.VehicleCode,
                                    VehicleOwner = item.VehicleOwner,
                                    TransportUnit = item.TransportUnit,
                                    DeliveryCompleted = item.DeliveryCompleted,
                                    GrossWeight = item.GrossWeight,
                                    NetWeight = item.NetWeight,
                                    WeightUnit = item.WeightUnit
                                });
                            }
                            else
                            {
                                detailPO.ProductCode = item.Material;
                                detailPO.Batch = item.Batch;
                                detailPO.OrderQuantity = item.OrderQuantity ?? 0;
                                detailPO.OpenQuantity = item.OpenQuantity ?? 0;
                                detailPO.LastEditTime = DateTime.Now;
                                detailPO.QuantityReceived = item.QuantityReceived ?? 0;
                                detailPO.DeletionInd = item.DeletionInd;
                                detailPO.StorageLocation = item.StorageLocation;
                                detailPO.Unit = item.UoM;
                                detailPO.Deliver = item.Deliver;
                                detailPO.VehicleCode = item.VehicleCode;
                                detailPO.VehicleOwner = item.VehicleOwner;
                                detailPO.TransportUnit = item.TransportUnit;
                                detailPO.DeliveryCompleted = item.DeliveryCompleted;
                                detailPO.GrossWeight = item.GrossWeight;
                                detailPO.NetWeight = item.NetWeight;
                                detailPO.WeightUnit = item.WeightUnit;
                            }
                        }
                        #endregion
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = poIntegration.PurchaseOrder,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
