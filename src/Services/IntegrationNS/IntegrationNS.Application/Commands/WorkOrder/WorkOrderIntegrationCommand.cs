using IntegrationNS.Application.Commands.PurchaseOrders;
using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
        //Ngày bắt đầu
        public DateTime? StartDate { get; set; }
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

    }

    public class WorkOrderIntegrationCommandHandler : IRequestHandler<WorkOrderIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WorkOrderModel> _woRepo;

        public WorkOrderIntegrationCommandHandler(IUnitOfWork unitOfWork, IRepository<WorkOrderModel> woRepo)
        {
            _unitOfWork = unitOfWork;
            _woRepo = woRepo;
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
                        _woRepo.Add(new WorkOrderModel
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
                            StartDate = woIntegration.StartDate,
                            ConfirmedYieldQuantity = woIntegration.ConfirmedYieldQuantity,
                            DeletionFlag = woIntegration.DeletionFlag,
                            LongTextExists = woIntegration.LongTextExists,
                            ReferenceOrder = woIntegration.ReferenceOrder,
                            SystemStatus = woIntegration.SystemStatus,


                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
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
                        workorder.StartDate = woIntegration.StartDate;
                        workorder.ConfirmedYieldQuantity = woIntegration.ConfirmedYieldQuantity;
                        workorder.DeletionFlag = woIntegration.DeletionFlag;
                        workorder.LongTextExists = woIntegration.LongTextExists;
                        workorder.ReferenceOrder = woIntegration.ReferenceOrder;
                        workorder.SystemStatus = woIntegration.SystemStatus;

                        //Common
                        workorder.LastEditTime = DateTime.Now;
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
