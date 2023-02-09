using IntegrationNS.Application.Commands.PurchaseOrders;
using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Models;
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
using System.Diagnostics;
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
        public string WorkOrderItem { get; set; }      //Item LSX
        public string ProductCode { get; set; }      //NVL
        public DateTime? RequirementDate { get; set; }      //
        public decimal? RequirementQuantiy { get; set; }      //
        public decimal? QuantityWithdrawn { get; set; }      //
        public string BaseUnit1 { get; set; }      //EINHEIT
        public string BaseUnit2 { get; set; }      //MEINS
        public string Batch { get; set; }      //Số lô
        public string Activity { get; set; }      //
        public string ReservationItem { get; set; }      //
        public decimal? OpenQuantity { get; set; }      //
        public decimal? Shortage { get; set; }      //
        public string StorageLocation { get; set; }      //Storage Location
        public string IconMessType { get; set; }      //
        public string SystemStatus { get; set; }      //Trạng thái
        public decimal? ConfirmedQty { get; set; }      //Confirm Qty
        public decimal? QuantityFixed1 { get; set; }      //DUM_FMENG
        public string PurchasingDoc { get; set; }      //
        public string PurchasingDocItem { get; set; }      //
        public string Supplier { get; set; }      //
        public string MovementType { get; set; }      //
        public decimal? QuantityFixed2 { get; set; }      //FMENG
        public string FinalIssue { get; set; }                  //


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
                            WorkOrderCodeInt = long.Parse(woIntegration.WorkOrderCode),
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
                                RequirementDate = item.RequirementDate,
                                RequirementQuantiy = item.RequirementQuantiy,
                                QuantityWithdrawn = item.QuantityWithdrawn,
                                BaseUnit1 = item.BaseUnit1,
                                BaseUnit2 = item.BaseUnit2,
                                Batch = item.Batch,
                                Activity = item.Activity,
                                ReservationItem = item.ReservationItem,
                                OpenQuantity = item.OpenQuantity,
                                Shortage = item.Shortage,
                                StorageLocation = item.StorageLocation,
                                IconMessType = item.IconMessType,
                                SystemStatus = item.SystemStatus,
                                ConfirmedQty = item.ConfirmedQty,
                                QuantityFixed1 = item.QuantityFixed1,
                                PurchasingDoc = item.PurchasingDoc,
                                PurchasingDocItem = item.PurchasingDocItem,
                                Supplier = item.Supplier,
                                MovementType = item.MovementType,
                                QuantityFixed2 = item.QuantityFixed2,
                                FinalIssue = item.FinalIssue,


                                CreateTime = DateTime.Now,
                                Actived = true,

                            });

                        }
                        workorder.DetailWorkOrderModel = detailWOs;
                        _woRepo.Add(workorder);
                    }
                    else
                    {
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
                                    RequirementDate = item.RequirementDate,
                                    RequirementQuantiy = item.RequirementQuantiy,
                                    QuantityWithdrawn = item.QuantityWithdrawn,
                                    BaseUnit1 = item.BaseUnit1,
                                    BaseUnit2 = item.BaseUnit2,
                                    Batch = item.Batch,
                                    Activity = item.Activity,
                                    ReservationItem = item.ReservationItem,
                                    OpenQuantity = item.OpenQuantity,
                                    Shortage = item.Shortage,
                                    StorageLocation = item.StorageLocation,
                                    IconMessType = item.IconMessType,
                                    SystemStatus = item.SystemStatus,
                                    ConfirmedQty = item.ConfirmedQty,
                                    QuantityFixed1 = item.QuantityFixed1,
                                    PurchasingDoc = item.PurchasingDoc,
                                    PurchasingDocItem = item.PurchasingDocItem,
                                    Supplier = item.Supplier,
                                    MovementType = item.MovementType,
                                    QuantityFixed2 = item.QuantityFixed2,
                                    FinalIssue = item.FinalIssue,
                                    CreateTime = DateTime.Now,
                                    Actived = true,
                                });
                            }
                            else
                            {
                                detailWO.ProductCode = item.ProductCode;
                                detailWO.RequirementDate = item.RequirementDate;
                                detailWO.RequirementQuantiy = item.RequirementQuantiy;
                                detailWO.QuantityWithdrawn = item.QuantityWithdrawn;
                                detailWO.BaseUnit1 = item.BaseUnit1;
                                detailWO.BaseUnit2 = item.BaseUnit2;
                                detailWO.Batch = item.Batch;
                                detailWO.Activity = item.Activity;
                                detailWO.ReservationItem = item.ReservationItem;
                                detailWO.OpenQuantity = item.OpenQuantity;
                                detailWO.Shortage = item.Shortage;
                                detailWO.StorageLocation = item.StorageLocation;
                                detailWO.IconMessType = item.IconMessType;
                                detailWO.SystemStatus = item.SystemStatus;
                                detailWO.ConfirmedQty = item.ConfirmedQty;
                                detailWO.QuantityFixed1 = item.QuantityFixed1;
                                detailWO.PurchasingDoc = item.PurchasingDoc;
                                detailWO.PurchasingDocItem = item.PurchasingDocItem;
                                detailWO.Supplier = item.Supplier;
                                detailWO.MovementType = item.MovementType;
                                detailWO.QuantityFixed2 = item.QuantityFixed2;
                                detailWO.FinalIssue = item.FinalIssue;


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
