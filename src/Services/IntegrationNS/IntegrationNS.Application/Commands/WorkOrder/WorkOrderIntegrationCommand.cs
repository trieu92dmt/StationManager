using IntegrationNS.Application.Commands.PurchaseOrders;
using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Graph.TermStore;
using NLog;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace IntegrationNS.Application.Commands.WorkOrder
{
    public class WorkOrderIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<WorkOrderIntegration> WorkOrderIntegrations { get; set; } = new List<WorkOrderIntegration>();
    }

    public class WorkOrderIntegration
    {
        //LSX
        public string WorkOrderCode { get; set; }
        //NVL
        public string ProductCode { get; set; }
        //Order Type
        public string OrderType { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Thời gian kết thúc cân
        public string StorageLocation { get; set; }
        //Số lô
        public string Batch { get; set; }
        //Số lượng chỉ tiêu
        public decimal? TargetQuantity { get; set; }
        //ĐVT
        public string Unit { get; set; }
        //Ngày kết thúc thực tế
        public DateTime? ActualFinishDate { get; set; }
        //Ngày kết thúc dự kiến
        public DateTime? ScheduledFinishDate { get; set; }
        //Ngày bắt đầu dự kiến
        public DateTime? ScheduledStartDate { get; set; }
        //Số lượng giao hàng
        public decimal? DeliveredQuantity { get; set; }
        //Sale Order
        public string SalesOrder { get; set; }
        //Sale Order Item
        public string SalesOrderItem { get; set; }
        //Order Category
        public string OrderCategory { get; set; }
        //Ngày bắt đầu thực tế
        public DateTime? ActualStartDate { get; set; }
        //Số lượng năng suất được xác nhận
        public decimal? ConfirmedYieldQuantity { get; set; }
        //Cờ xóa
        public string DeletionFlag { get; set; }
        //LongTextExists
        public string LongTextExists { get; set; }
        //Tham chiếu
        public string ReferenceOrder { get; set; }
        //Trạng thái
        public string SystemStatus { get; set; }

        public List<DetallWorkOrderIntegration> DetallWorkOrderIntegrations { get; set; } = new List<DetallWorkOrderIntegration>();
    }

    public class DetallWorkOrderIntegration
    {
        public string WorkOrderItem { get; set; }                  //Item LSX
        public string ProductCode { get; set; }                  //NVL
        public decimal? TotalQuantiy { get; set; }                  //Tổng số lượng
        public decimal? RoutingScrapQuantity { get; set; }                  //Số lượng phế liệu định tuyến
        public decimal? ScrapQuantity { get; set; }                  //Số lượng phế liệu
        public decimal? GRQuantity { get; set; }                  //SL đã post giao dịch
        public string Unit { get; set; }                  //Đơn vị tính
        public string UnloadingPoint { get; set; }                  //Điểm dỡ hàng
        public string Batch { get; set; }                  //Số lô
        public DateTime? BasicFinishDate { get; set; }                  //Basic finish date
        public DateTime? ScheduledFinishDate { get; set; }                  //Scheduled finish date
        public string DeliveryComplete { get; set; }                  //Delivery Completed
        public DateTime? PlanOpenDate { get; set; }                  //Plan Open Date
        public DateTime? CommitmentDate { get; set; }                  //Commitment Date
        public string StockType { get; set; }                  //Stock Type
        public string KanbanIndicator { get; set; }                  //Kanban Indicator
        public string SalesOrder { get; set; }                  //Sales Order
        public string SOSchedule { get; set; }                  //Sales Order Schedule
        public string SalesOrderItem { get; set; }                  //Sales Order Item
        public string ValidFrom { get; set; }                  //Valid From
        public string SoldToParty { get; set; }                  //Sold To Party
        public string TypeAvailCheck { get; set; }                  //Type Avail Check
        public string SpecStkValuation { get; set; }                  //Spec.stk valuation
        public string Consumption { get; set; }                  //Consumption
        public string StorageLocation { get; set; }                  //Storage Location
        public DateTime? ActualDeliveryDate { get; set; }                  //Actual Delivery Date
        public DateTime? PldOrderDelDate { get; set; }                  //Pld. order del. date
        public string BaseUoM { get; set; }                  //Base Unit of Measure
        public string Name { get; set; }                  //Name Name
        public string ChangeIndicator { get; set; }                  //Change Indicator
        public string PlanOrder { get; set; }                  //Plan Order
        public string SpecialProcurement { get; set; }                  //Special Procurement
        public string PlanningPlant { get; set; }                  //Planning Plant
        public string BOM { get; set; }                  // BOM
        public string SpecialStock { get; set; }                  //Special Stock
        public DateTime? PlanStartDate { get; set; }                  //Planed Start Date
        public string ProductionVersion { get; set; }                  //Production Version
        public decimal? CommitedQty { get; set; }                  //Commited Qty
        public decimal? GRProcessingTime { get; set; }                  //GRProcessing Time
        public string GoodsRecipient { get; set; }                  //Goods Recipient
        public string GoodsReceiptIndicator { get; set; }                  //Goods Recipient Indicator
        public string DeletionFlag { get; set; }                  //Cờ xóa
        public decimal? AllocatedStockQty { get; set; }                  //Allocated Stock Quantity
        public string Customer { get; set; }                  //Customer
        public string NumOfOriginalOrder { get; set; }                  //Number of original Order
        public decimal? ConfirmQty { get; set; }               //Confirm Qty

    }


    public class WorkOrderIntegrationCommandHandler : IRequestHandler<WorkOrderIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<DetailWorkOrderModel> _detailWORepo;

        public WorkOrderIntegrationCommandHandler(IUnitOfWork unitOfWork, IRepository<WorkOrderModel> woRepo, IRepository<DetailWorkOrderModel> detailWORepo)
        {
            _unitOfWork = unitOfWork;
            _woRepo = woRepo;
            _detailWORepo = detailWORepo;
        }

        public async Task<IntegrationNSResponse> Handle(WorkOrderIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.WorkOrderIntegrations.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.WorkOrderIntegrations.Count();

            var workorders = _woRepo.GetQuery();

            foreach (var woIntegration in request.WorkOrderIntegrations)
            {
                try
                {
                    //Check tồn tại
                    var workorder = await workorders.FirstOrDefaultAsync(x => x.WorkOrderCode == woIntegration.WorkOrderCode);

                    if (workorder is null)
                    {
                        workorder = new WorkOrderModel
                        {
                            WorkOrderId = Guid.NewGuid(),
                            WorkOrderCode = woIntegration.WorkOrderCode,
                            ProductCode = woIntegration.ProductCode,
                            OrderTypeCode = woIntegration.OrderType,
                            Plant = woIntegration.Plant,
                            StorageLocation = woIntegration.StorageLocation,
                            Batch = woIntegration.Batch,
                            TargetQuantity = woIntegration.TargetQuantity,
                            Unit = woIntegration.Unit,
                            ActualFinishDate = woIntegration.ActualFinishDate,
                            ScheduledFinishDate = woIntegration.ScheduledFinishDate,
                            ScheduledStartDate = woIntegration.ScheduledStartDate,
                            DeliveredQuantity = woIntegration.DeliveredQuantity,
                            SalesOrder = woIntegration.SalesOrder,
                            SalesOrderItem = woIntegration.SalesOrderItem,
                            OrderCategory = woIntegration.OrderCategory,
                            ActualStartDate = woIntegration.ActualStartDate,
                            ConfirmedYieldQuantity = woIntegration.ConfirmedYieldQuantity,
                            DeletionFlag = woIntegration.DeletionFlag,
                            LongTextExists = woIntegration.LongTextExists,
                            ReferenceOrder = woIntegration.ReferenceOrder,
                            SystemStatus = woIntegration.SystemStatus,


                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        };

                        //Detail
                        var detailWOs = new List<DetailWorkOrderModel>();
                        foreach (var item in woIntegration.DetallWorkOrderIntegrations)
                        {

                            detailWOs.Add(new DetailWorkOrderModel
                            {
                                DetailWorkOrderId = Guid.NewGuid(),
                                WorkOrderId = workorder.WorkOrderId,
                                WorkOrderItem = item.WorkOrderItem,
                                ProductCode = item.ProductCode,
                                TotalQuantiy = item.TotalQuantiy,
                                RoutingScrapQuantity = item.RoutingScrapQuantity,
                                ScrapQuantity = item.ScrapQuantity,
                                GRQuantity = item.GRQuantity,
                                Unit = item.Unit,
                                UnloadingPoint = item.UnloadingPoint,
                                Batch = item.Batch,
                                BasicFinishDate = item.BasicFinishDate,
                                ScheduledFinishDate = item.ScheduledFinishDate,
                                DeliveryComplete = item.DeliveryComplete,
                                PlanOpenDate = item.PlanOpenDate,
                                CommitmentDate = item.CommitmentDate,
                                StockType = item.StockType,
                                KanbanIndicator = item.KanbanIndicator,
                                SalesOrder = item.SalesOrder,
                                SOSchedule = item.SOSchedule,
                                SalesOrderItem = item.SalesOrderItem,
                                ValidFrom = item.ValidFrom,
                                SoldToParty = item.SoldToParty,
                                TypeAvailCheck = item.TypeAvailCheck,
                                SpecStkValuation = item.SpecStkValuation,
                                Consumption = item.Consumption,
                                StorageLocation = item.StorageLocation,
                                ActualDeliveryDate = item.ActualDeliveryDate,
                                PldOrderDelDate = item.PldOrderDelDate,
                                BaseUoM = item.BaseUoM,
                                Name = item.Name,
                                ChangeIndicator = item.ChangeIndicator,
                                PlanOrder = item.PlanOrder,
                                SpecialProcurement = item.SpecialProcurement,
                                PlanningPlant = item.PlanningPlant,
                                BOM = item.BOM,
                                SpecialStock = item.SpecialStock,
                                PlanStartDate = item.PlanStartDate,
                                ProductionVersion = item.ProductionVersion,
                                CommitedQty = item.CommitedQty,
                                GRProcessingTime = item.GRProcessingTime,
                                GoodsRecipient = item.GoodsRecipient,
                                GoodsReceiptIndicator = item.GoodsReceiptIndicator,
                                DeletionFlag = item.DeletionFlag,
                                AllocatedStockQty = item.AllocatedStockQty,
                                Customer = item.Customer,
                                NumOfOriginalOrder = item.NumOfOriginalOrder,
                                ConfirmQty = item.ConfirmQty,

                                CreateTime = DateTime.Now,
                                Actived = true,

                            });

                        }
                        workorder.DetailWorkOrderModel = detailWOs;
                        _woRepo.Add(workorder);
                    }
                    else
                    {
                        workorder.WorkOrderCode = woIntegration.WorkOrderCode;
                        workorder.ProductCode = woIntegration.ProductCode;
                        workorder.OrderTypeCode = woIntegration.OrderType;
                        workorder.Plant = woIntegration.Plant;
                        workorder.StorageLocation = woIntegration.StorageLocation;
                        workorder.Batch = woIntegration.Batch;
                        workorder.TargetQuantity = woIntegration.TargetQuantity;
                        workorder.Unit = woIntegration.Unit;
                        workorder.ActualFinishDate = woIntegration.ActualFinishDate;
                        workorder.ScheduledFinishDate = woIntegration.ScheduledFinishDate;
                        workorder.ScheduledStartDate = woIntegration.ScheduledStartDate;
                        workorder.DeliveredQuantity = woIntegration.DeliveredQuantity;
                        workorder.SalesOrder = woIntegration.SalesOrder;
                        workorder.SalesOrderItem = woIntegration.SalesOrderItem;
                        workorder.OrderCategory = woIntegration.OrderCategory;
                        workorder.ActualStartDate = woIntegration.ActualStartDate;
                        workorder.ConfirmedYieldQuantity = woIntegration.ConfirmedYieldQuantity;
                        workorder.DeletionFlag = woIntegration.DeletionFlag;
                        workorder.LongTextExists = woIntegration.LongTextExists;
                        workorder.ReferenceOrder = woIntegration.ReferenceOrder;
                        workorder.SystemStatus = woIntegration.SystemStatus;

                        //Common
                        workorder.LastEditTime = DateTime.Now;

                        #region Detail
                        //Cập nhật detail
                        foreach (var item in woIntegration.DetallWorkOrderIntegrations)
                        {
                            var detailWO = await _detailWORepo.FindOneAsync(x => x.WorkOrderId == workorder.WorkOrderId && x.WorkOrderItem == item.WorkOrderItem);
                            if (detailWO == null)
                            {

                                _detailWORepo.Add(new DetailWorkOrderModel
                                {
                                    DetailWorkOrderId = Guid.NewGuid(),
                                    WorkOrderId = workorder.WorkOrderId,
                                    WorkOrderItem = item.WorkOrderItem,
                                    ProductCode = item.ProductCode,
                                    TotalQuantiy = item.TotalQuantiy,
                                    RoutingScrapQuantity = item.RoutingScrapQuantity,
                                    ScrapQuantity = item.ScrapQuantity,
                                    GRQuantity = item.GRQuantity,
                                    Unit = item.Unit,
                                    UnloadingPoint = item.UnloadingPoint,
                                    Batch = item.Batch,
                                    BasicFinishDate = item.BasicFinishDate,
                                    ScheduledFinishDate = item.ScheduledFinishDate,
                                    DeliveryComplete = item.DeliveryComplete,
                                    PlanOpenDate = item.PlanOpenDate,
                                    CommitmentDate = item.CommitmentDate,
                                    StockType = item.StockType,
                                    KanbanIndicator = item.KanbanIndicator,
                                    SalesOrder = item.SalesOrder,
                                    SOSchedule = item.SOSchedule,
                                    SalesOrderItem = item.SalesOrderItem,
                                    ValidFrom = item.ValidFrom,
                                    SoldToParty = item.SoldToParty,
                                    TypeAvailCheck = item.TypeAvailCheck,
                                    SpecStkValuation = item.SpecStkValuation,
                                    Consumption = item.Consumption,
                                    StorageLocation = item.StorageLocation,
                                    ActualDeliveryDate = item.ActualDeliveryDate,
                                    PldOrderDelDate = item.PldOrderDelDate,
                                    BaseUoM = item.BaseUoM,
                                    Name = item.Name,
                                    ChangeIndicator = item.ChangeIndicator,
                                    PlanOrder = item.PlanOrder,
                                    SpecialProcurement = item.SpecialProcurement,
                                    PlanningPlant = item.PlanningPlant,
                                    BOM = item.BOM,
                                    SpecialStock = item.SpecialStock,
                                    PlanStartDate = item.PlanStartDate,
                                    ProductionVersion = item.ProductionVersion,
                                    CommitedQty = item.CommitedQty,
                                    GRProcessingTime = item.GRProcessingTime,
                                    GoodsRecipient = item.GoodsRecipient,
                                    GoodsReceiptIndicator = item.GoodsReceiptIndicator,
                                    DeletionFlag = item.DeletionFlag,
                                    AllocatedStockQty = item.AllocatedStockQty,
                                    Customer = item.Customer,
                                    NumOfOriginalOrder = item.NumOfOriginalOrder,
                                    ConfirmQty = item.ConfirmQty,
                                    CreateTime = DateTime.Now,
                                    Actived = true,
                                });
                            }
                            else
                            {
                                detailWO.ProductCode = item.ProductCode;
                                detailWO.TotalQuantiy = item.TotalQuantiy;
                                detailWO.RoutingScrapQuantity = item.RoutingScrapQuantity;
                                detailWO.ScrapQuantity = item.ScrapQuantity;
                                detailWO.GRQuantity = item.GRQuantity;
                                detailWO.Unit = item.Unit;
                                detailWO.UnloadingPoint = item.UnloadingPoint;
                                detailWO.Batch = item.Batch;
                                detailWO.BasicFinishDate = item.BasicFinishDate;
                                detailWO.ScheduledFinishDate = item.ScheduledFinishDate;
                                detailWO.DeliveryComplete = item.DeliveryComplete;
                                detailWO.PlanOpenDate = item.PlanOpenDate;
                                detailWO.CommitmentDate = item.CommitmentDate;
                                detailWO.StockType = item.StockType;
                                detailWO.KanbanIndicator = item.KanbanIndicator;
                                detailWO.SalesOrder = item.SalesOrder;
                                detailWO.SOSchedule = item.SOSchedule;
                                detailWO.SalesOrderItem = item.SalesOrderItem;
                                detailWO.ValidFrom = item.ValidFrom;
                                detailWO.SoldToParty = item.SoldToParty;
                                detailWO.TypeAvailCheck = item.TypeAvailCheck;
                                detailWO.SpecStkValuation = item.SpecStkValuation;
                                detailWO.Consumption = item.Consumption;
                                detailWO.StorageLocation = item.StorageLocation;
                                detailWO.ActualDeliveryDate = item.ActualDeliveryDate;
                                detailWO.PldOrderDelDate = item.PldOrderDelDate;
                                detailWO.BaseUoM = item.BaseUoM;
                                detailWO.Name = item.Name;
                                detailWO.ChangeIndicator = item.ChangeIndicator;
                                detailWO.PlanOrder = item.PlanOrder;
                                detailWO.SpecialProcurement = item.SpecialProcurement;
                                detailWO.PlanningPlant = item.PlanningPlant;
                                detailWO.BOM = item.BOM;
                                detailWO.SpecialStock = item.SpecialStock;
                                detailWO.PlanStartDate = item.PlanStartDate;
                                detailWO.ProductionVersion = item.ProductionVersion;
                                detailWO.CommitedQty = item.CommitedQty;
                                detailWO.GRProcessingTime = item.GRProcessingTime;
                                detailWO.GoodsRecipient = item.GoodsRecipient;
                                detailWO.GoodsReceiptIndicator = item.GoodsReceiptIndicator;
                                detailWO.DeletionFlag = item.DeletionFlag;
                                detailWO.AllocatedStockQty = item.AllocatedStockQty;
                                detailWO.Customer = item.Customer;
                                detailWO.NumOfOriginalOrder = item.NumOfOriginalOrder;
                                detailWO.ConfirmQty = item.ConfirmQty;

                                detailWO.LastEditTime = DateTime.Now;
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
                        RecordFail = woIntegration.WorkOrderCode,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
