using IntegrationNS.Application.Commands.PurchaseOrders;
using IntegrationNS.Application.DTOs;
using Infrastructure.Models;
using MediatR;
using Microsoft.Graph.TermStore;
using Microsoft.Graph;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode.Internal;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Exceptions;
using Core.Properties;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using System.Security.Cryptography.X509Certificates;

namespace IntegrationNS.Application.Commands.OutboundDelivery
{
    public class OutboundDeliveryIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<OutboundDeliveryIntegration> OutboundDeliveries { get; set; } = new List<OutboundDeliveryIntegration>();
    }

    #region Dtos
    public class OutboundDeliveryIntegration
    {
        public string DeliveryCode { get; set; }    //Mã Delivery
        public string ShippingPoint { get; set; }    //Shipping Point
        public string SaleOrgCode { get; set; }    //Sales Organization
        public string DeliveryType { get; set; }    //Delivery Type
        public string DeliveryTypeDescription { get; set; }    //Delivery Type Description
        public string VehicleCode { get; set; }    //Số phương tiện
        public DateTime? DeliveryDate { get; set; }    //Delivery Date
        public DateTime? PickingDate { get; set; }    //PickingDate
        public string ShiptoParty { get; set; }    //Ship-to Party
        public string ShiptoPartyName { get; set; }    //Ship-to Party Name
        public string SoldtoParty { get; set; }    //Sold-to Party
        public string SoldtoPartyName { get; set; }    //Sold-to Party Name
        public string PurchasingDocType { get; set; }    //Purchasing Doc.Type
        public string SalesDocumentType { get; set; }    //Sales Document Type
        public string DistribChannelCode { get; set; }    //Distrib Channel
        public string DivisionCode { get; set; }    //Division
        public string SupplierCode { get; set; }    //Supplier
        public string SupplierName { get; set; }    //Supplier Name
        public DateTime? DocumentDate { get; set; }    //Document Date
        public DateTime? ActGdsMvmntDate { get; set; }    //Act.Gds Mvmnt Date
        public string Order { get; set; }    //Order
        public string ReceivingPlant { get; set; }    //Receiving Plant
        public string Reference { get; set; }    //Reference
        public string TransactionCode { get; set; }    //Transaction Code
        public string StorageLocChange { get; set; }    //Storage Loc.Change
        public string OverallStatus { get; set; }    //Overall Status
        public string PickConfirmation { get; set; }    //Pick Confirmation
        public string PickingStatus { get; set; }    //Picking Status
        public string OverallBlockStatus { get; set; }    //Overall Block Status
        public string OverallHeader { get; set; }    //Overall Header
        public string AllItems { get; set; }    //All Items
        public string PickingPutawayHeader { get; set; }    //Picking/Putaway – Header
        public string PickingPtwyAllItems { get; set; }    //Picking/Ptwy – All Items
        public string DeliveryHeader { get; set; }    //Delivery – Header
        public string DeliveryAllItems { get; set; }    //Delivery – All Items
        public string GoodsMvtHeader { get; set; }    //Goods Mvt – Header
        public string GoodsMvtAllItems { get; set; }    //Goods Mvt – All Items
        public string GoodsMovementSts { get; set; }    //Goods Movement Sts

        public List<OutboundDeliveryDetailIntegration> OutboundDeliveryDetails { get; set; } = new List<OutboundDeliveryDetailIntegration>();

    }

    public class OutboundDeliveryDetailIntegration
    {
        public string OutboundDeliveryItem { get; set; }         //Delivery Item
        public string ProductCode { get; set; }         //Material
        public string Plant { get; set; }         //Plant
        public string StorageLocation { get; set; }         //Storage Location
        public string Batch { get; set; }         //Số lô
        public decimal? DeliveryQuantity { get; set; }         //Delivery Quantity
        public string Unit { get; set; }         //Base Unit of Measure
        public decimal? PickedQuantityPUoM { get; set; }         //Picked Quantity PUoM
        public string SalesUnit { get; set; }         //Sales Unit
        public decimal? NetWeight { get; set; }         //Net weight
        public decimal? GrossWeight { get; set; }         //Gross Weight
        public string WeightUnit { get; set; }         //Weight unit
        public decimal? ActualDeliveryQty { get; set; }         //Actual Delivery Qty
        public string ItemDescription { get; set; }         //Item Description
        public string ReferenceDocument1 { get; set; }         //Reference Document
        public string ReferenceItem { get; set; }         //Reference Item
        public string SalesOffice { get; set; }         //Sales Office
        public string SalesGroup { get; set; }         //Sales group
        public string DivisionCode { get; set; }         //Division
        public string Order { get; set; }         //Order
        public string OrderItem { get; set; }         //Order item number
        public string SalesOrder { get; set; }         //Sales Order
        public string SalesOrderItem { get; set; }         //Sales order item
        public string ReferenceDocument2 { get; set; }         //Reference Document
        public string ReferenceDocItem { get; set; }         //ReferenceDocItem
        public string GoodsMvmntControl { get; set; }         //Goods mvmnt control
        public string DeliveryCompleted { get; set; }         //Delivery Completed
        public decimal? OriginalQuantity { get; set; }         //Original Quantity of Delivery Item
        public string ItemNumberDocument { get; set; }         //Item Number in Document
        public string OverallStatus { get; set; }         //Overall Status
        public string PickingStatus { get; set; }         //Picking Status
        public string Item { get; set; }         //Item
        public string PickingPutawayItem { get; set; }         //Picking/Putaway – Item
        public string DeliveryItem { get; set; }         //Delivery – Item
        public string GoodsMvtItem { get; set; }         //Goods Mvt – Item
        public string GoodsMovementSts { get; set; }         //Goods Movement Sts
        public string DistributionChannel { get; set; }             //Distribution Channel
    }
    #endregion

    public class OutboundDeliveryIntegrationNSCommandHandler : IRequestHandler<OutboundDeliveryIntegrationNSCommand, IntegrationNSResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OutboundDeliveryModel> _deliveryRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _deliDetailRepo;

        public OutboundDeliveryIntegrationNSCommandHandler(IUnitOfWork unitOfWork, IRepository<OutboundDeliveryModel> deliveryRepo, IRepository<DetailOutboundDeliveryModel> deliDetailRepo)
        {
            _unitOfWork = unitOfWork;
            _deliveryRepo = deliveryRepo;
            _deliDetailRepo = deliDetailRepo;
        }

        public async Task<IntegrationNSResponse> Handle(OutboundDeliveryIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            //Kiểm tra tồn tại dữ liệu đồng bộ
            if (!request.OutboundDeliveries.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            //Tổng số record đồng bộ
            response.TotalRecord = request.OutboundDeliveries.Count();

            foreach (var outboundDelivery in request.OutboundDeliveries)
            {
                try
                {
                    //Outbound Delivery
                    var delivery = await _deliveryRepo.GetQuery(x => x.DeliveryCode == outboundDelivery.DeliveryCode)
                                                    .Include(x => x.DetailOutboundDeliveryModel)
                                                    .FirstOrDefaultAsync();

                    //Không có tạo mới
                    if (delivery is null)
                    {
                        //Hearder 
                        delivery = new OutboundDeliveryModel();
                        delivery.OutboundDeliveryId = Guid.NewGuid();
                        delivery.DeliveryCode = outboundDelivery.DeliveryCode;
                        delivery.DeliveryCodeInt = long.Parse(outboundDelivery.DeliveryCode);
                        delivery.ShippingPoint = outboundDelivery.ShippingPoint;
                        delivery.SaleOrgCode = outboundDelivery.SaleOrgCode;
                        delivery.DeliveryType = outboundDelivery.DeliveryType;
                        delivery.DeliveryTypeDescription = outboundDelivery.DeliveryTypeDescription;
                        delivery.VehicleCode = outboundDelivery.VehicleCode;
                        delivery.DeliveryDate = outboundDelivery.DeliveryDate;
                        delivery.PickingDate = outboundDelivery.PickingDate;
                        delivery.ShiptoParty = outboundDelivery.ShiptoParty;
                        delivery.ShiptoPartyName = outboundDelivery.ShiptoPartyName;
                        delivery.SoldtoParty = outboundDelivery.SoldtoParty;
                        delivery.SoldtoPartyName = outboundDelivery.SoldtoPartyName;
                        delivery.PurchasingDocType = outboundDelivery.PurchasingDocType;
                        delivery.SalesDocumentType = outboundDelivery.SalesDocumentType;
                        delivery.DistribChannelCode = outboundDelivery.DistribChannelCode;
                        delivery.DivisionCode = outboundDelivery.DivisionCode;
                        delivery.SupplierCode = outboundDelivery.SupplierCode;
                        delivery.SupplierName = outboundDelivery.SupplierName;
                        delivery.DocumentDate = outboundDelivery.DocumentDate;
                        delivery.ActGdsMvmntDate = outboundDelivery.ActGdsMvmntDate;
                        delivery.Order = outboundDelivery.Order;
                        delivery.ReceivingPlant = outboundDelivery.ReceivingPlant;
                        delivery.Reference = outboundDelivery.Reference;
                        delivery.TransactionCode = outboundDelivery.TransactionCode;
                        delivery.StorageLocChange = outboundDelivery.StorageLocChange;
                        delivery.OverallStatus = outboundDelivery.OverallStatus;
                        delivery.PickConfirmation = outboundDelivery.PickConfirmation;
                        delivery.PickingStatus = outboundDelivery.PickingStatus;
                        delivery.OverallBlockStatus = outboundDelivery.OverallBlockStatus;
                        delivery.OverallHeader = outboundDelivery.OverallHeader;
                        delivery.AllItems = outboundDelivery.AllItems;
                        delivery.PickingPutawayHeader = outboundDelivery.PickingPutawayHeader;
                        delivery.PickingPtwyAllItems = outboundDelivery.PickingPtwyAllItems;
                        delivery.DeliveryHeader = outboundDelivery.DeliveryHeader;
                        delivery.DeliveryAllItems = outboundDelivery.DeliveryAllItems;
                        delivery.GoodsMvtHeader = outboundDelivery.GoodsMvtHeader;
                        delivery.GoodsMvtAllItems = outboundDelivery.GoodsMvtAllItems;
                        delivery.GoodsMovementSts = outboundDelivery.GoodsMovementSts;


                        delivery.CreateTime = DateTime.Now;

                        //Detail
                        var detailDeliveries = new List<DetailOutboundDeliveryModel>();
                        foreach (var item in outboundDelivery.OutboundDeliveryDetails)
                        {
                            detailDeliveries.Add(new DetailOutboundDeliveryModel
                            {
                                DetailOutboundDeliveryId = Guid.NewGuid(),
                                OutboundDeliveryId = delivery.OutboundDeliveryId,
                                OutboundDeliveryItem = item.OutboundDeliveryItem,
                                ProductCode = item.ProductCode,
                                ProductCodeInt = long.Parse(item.ProductCode),
                                Plant = item.Plant,
                                StorageLocation = item.StorageLocation,
                                Batch = item.Batch,
                                DeliveryQuantity = item.DeliveryQuantity,
                                Unit = item.Unit,
                                PickedQuantityPUoM = item.PickedQuantityPUoM,
                                SalesUnit = item.SalesUnit,
                                NetWeight = item.NetWeight,
                                GrossWeight = item.GrossWeight,
                                WeightUnit = item.WeightUnit,
                                ActualDeliveryQty = item.ActualDeliveryQty,
                                ItemDescription = item.ItemDescription,
                                ReferenceDocument1 = item.ReferenceDocument1,
                                ReferenceItem = item.ReferenceItem,
                                SalesOffice = item.SalesOffice,
                                SalesGroup = item.SalesGroup,
                                DivisionCode = item.DivisionCode,
                                Order = item.Order,
                                OrderItem = item.OrderItem,
                                SalesOrder = item.SalesOrder,
                                SalesOrderItem = item.SalesOrderItem,
                                ReferenceDocument2 = item.ReferenceDocument2,
                                ReferenceDocItem = item.ReferenceDocItem,
                                GoodsMvmntControl = item.GoodsMvmntControl,
                                DeliveryCompleted = item.DeliveryCompleted,
                                OriginalQuantity = item.OriginalQuantity,
                                ItemNumberDocument = item.ItemNumberDocument,
                                OverallStatus = item.OverallStatus,
                                PickingStatus = item.PickingStatus,
                                Item = item.Item,
                                PickingPutawayItem = item.PickingPutawayItem,
                                DeliveryItem = item.DeliveryItem,
                                GoodsMvtItem = item.GoodsMvtItem,
                                GoodsMovementSts = item.GoodsMovementSts,
                                DistributionChannel = item.DistributionChannel,

                            });

                        }

                        delivery.DetailOutboundDeliveryModel = detailDeliveries;
                        _deliveryRepo.Add(delivery);
                    }
                    //Có thì cập nhật
                    else
                    {
                        #region Header
                        //Cập nhật header
                        delivery.ShippingPoint = outboundDelivery.ShippingPoint;
                        delivery.SaleOrgCode = outboundDelivery.SaleOrgCode;
                        delivery.DeliveryType = outboundDelivery.DeliveryType;
                        delivery.DeliveryTypeDescription = outboundDelivery.DeliveryTypeDescription;
                        delivery.VehicleCode = outboundDelivery.VehicleCode;
                        delivery.DeliveryDate = outboundDelivery.DeliveryDate;
                        delivery.PickingDate = outboundDelivery.PickingDate;
                        delivery.ShiptoParty = outboundDelivery.ShiptoParty;
                        delivery.ShiptoPartyName = outboundDelivery.ShiptoPartyName;
                        delivery.SoldtoParty = outboundDelivery.SoldtoParty;
                        delivery.SoldtoPartyName = outboundDelivery.SoldtoPartyName;
                        delivery.PurchasingDocType = outboundDelivery.PurchasingDocType;
                        delivery.SalesDocumentType = outboundDelivery.SalesDocumentType;
                        delivery.DistribChannelCode = outboundDelivery.DistribChannelCode;
                        delivery.DivisionCode = outboundDelivery.DivisionCode;
                        delivery.SupplierCode = outboundDelivery.SupplierCode;
                        delivery.SupplierName = outboundDelivery.SupplierName;
                        delivery.DocumentDate = outboundDelivery.DocumentDate;
                        delivery.ActGdsMvmntDate = outboundDelivery.ActGdsMvmntDate;
                        delivery.Order = outboundDelivery.Order;
                        delivery.ReceivingPlant = outboundDelivery.ReceivingPlant;
                        delivery.Reference = outboundDelivery.Reference;
                        delivery.TransactionCode = outboundDelivery.TransactionCode;
                        delivery.StorageLocChange = outboundDelivery.StorageLocChange;
                        delivery.OverallStatus = outboundDelivery.OverallStatus;
                        delivery.PickConfirmation = outboundDelivery.PickConfirmation;
                        delivery.PickingStatus = outboundDelivery.PickingStatus;
                        delivery.OverallBlockStatus = outboundDelivery.OverallBlockStatus;
                        delivery.OverallHeader = outboundDelivery.OverallHeader;
                        delivery.AllItems = outboundDelivery.AllItems;
                        delivery.PickingPutawayHeader = outboundDelivery.PickingPutawayHeader;
                        delivery.PickingPtwyAllItems = outboundDelivery.PickingPtwyAllItems;
                        delivery.DeliveryHeader = outboundDelivery.DeliveryHeader;
                        delivery.DeliveryAllItems = outboundDelivery.DeliveryAllItems;
                        delivery.GoodsMvtHeader = outboundDelivery.GoodsMvtHeader;
                        delivery.GoodsMvtAllItems = outboundDelivery.GoodsMvtAllItems;
                        delivery.GoodsMovementSts = outboundDelivery.GoodsMovementSts;
                        delivery.LastEditTime = DateTime.Now;

                        #endregion

                        #region Detail
                        //Cập nhật detail
                        foreach (var item in outboundDelivery.OutboundDeliveryDetails)
                        {
                            var detailDelivery = await _deliDetailRepo.FindOneAsync(x => x.OutboundDeliveryId == delivery.OutboundDeliveryId && x.OutboundDeliveryItem == item.OutboundDeliveryItem);
                            if (detailDelivery == null)
                            {

                                _deliDetailRepo.Add(new DetailOutboundDeliveryModel
                                {
                                    DetailOutboundDeliveryId = Guid.NewGuid(),
                                    OutboundDeliveryId = delivery.OutboundDeliveryId,
                                    OutboundDeliveryItem = item.OutboundDeliveryItem,
                                    ProductCode = item.ProductCode,
                                    ProductCodeInt = long.Parse(item.ProductCode),
                                    Plant = item.Plant,
                                    StorageLocation = item.StorageLocation,
                                    Batch = item.Batch,
                                    DeliveryQuantity = item.DeliveryQuantity,
                                    Unit = item.Unit,
                                    PickedQuantityPUoM = item.PickedQuantityPUoM,
                                    SalesUnit = item.SalesUnit,
                                    NetWeight = item.NetWeight,
                                    GrossWeight = item.GrossWeight,
                                    WeightUnit = item.WeightUnit,
                                    ActualDeliveryQty = item.ActualDeliveryQty,
                                    ItemDescription = item.ItemDescription,
                                    ReferenceDocument1 = item.ReferenceDocument1,
                                    ReferenceItem = item.ReferenceItem,
                                    SalesOffice = item.SalesOffice,
                                    SalesGroup = item.SalesGroup,
                                    DivisionCode = item.DivisionCode,
                                    Order = item.Order,
                                    OrderItem = item.OrderItem,
                                    SalesOrder = item.SalesOrder,
                                    SalesOrderItem = item.SalesOrderItem,
                                    ReferenceDocument2 = item.ReferenceDocument2,
                                    ReferenceDocItem = item.ReferenceDocItem,
                                    GoodsMvmntControl = item.GoodsMvmntControl,
                                    DeliveryCompleted = item.DeliveryCompleted,
                                    OriginalQuantity = item.OriginalQuantity,
                                    ItemNumberDocument = item.ItemNumberDocument,
                                    OverallStatus = item.OverallStatus,
                                    PickingStatus = item.PickingStatus,
                                    Item = item.Item,
                                    PickingPutawayItem = item.PickingPutawayItem,
                                    DeliveryItem = item.DeliveryItem,
                                    GoodsMvtItem = item.GoodsMvtItem,
                                    GoodsMovementSts = item.GoodsMovementSts,
                                    DistributionChannel = item.DistributionChannel,
                                });
                            }
                            else
                            {
                                detailDelivery.ProductCode = item.ProductCode;
                                detailDelivery.ProductCodeInt = long.Parse(item.ProductCode);
                                detailDelivery.Plant = item.Plant;
                                detailDelivery.StorageLocation = item.StorageLocation;
                                detailDelivery.Batch = item.Batch;
                                detailDelivery.DeliveryQuantity = item.DeliveryQuantity;
                                detailDelivery.Unit = item.Unit;
                                detailDelivery.PickedQuantityPUoM = item.PickedQuantityPUoM;
                                detailDelivery.SalesUnit = item.SalesUnit;
                                detailDelivery.NetWeight = item.NetWeight;
                                detailDelivery.GrossWeight = item.GrossWeight;
                                detailDelivery.WeightUnit = item.WeightUnit;
                                detailDelivery.ActualDeliveryQty = item.ActualDeliveryQty;
                                detailDelivery.ItemDescription = item.ItemDescription;
                                detailDelivery.ReferenceDocument1 = item.ReferenceDocument1;
                                detailDelivery.ReferenceItem = item.ReferenceItem;
                                detailDelivery.SalesOffice = item.SalesOffice;
                                detailDelivery.SalesGroup = item.SalesGroup;
                                detailDelivery.DivisionCode = item.DivisionCode;
                                detailDelivery.Order = item.Order;
                                detailDelivery.OrderItem = item.OrderItem;
                                detailDelivery.SalesOrder = item.SalesOrder;
                                detailDelivery.SalesOrderItem = item.SalesOrderItem;
                                detailDelivery.ReferenceDocument2 = item.ReferenceDocument2;
                                detailDelivery.ReferenceDocItem = item.ReferenceDocItem;
                                detailDelivery.GoodsMvmntControl = item.GoodsMvmntControl;
                                detailDelivery.DeliveryCompleted = item.DeliveryCompleted;
                                detailDelivery.OriginalQuantity = item.OriginalQuantity;
                                detailDelivery.ItemNumberDocument = item.ItemNumberDocument;
                                detailDelivery.OverallStatus = item.OverallStatus;
                                detailDelivery.PickingStatus = item.PickingStatus;
                                detailDelivery.Item = item.Item;
                                detailDelivery.PickingPutawayItem = item.PickingPutawayItem;
                                detailDelivery.DeliveryItem = item.DeliveryItem;
                                detailDelivery.GoodsMvtItem = item.GoodsMvtItem;
                                detailDelivery.GoodsMovementSts = item.GoodsMovementSts;
                                detailDelivery.DistributionChannel = item.DistributionChannel;

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
                        RecordFail = outboundDelivery.DeliveryCode,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
