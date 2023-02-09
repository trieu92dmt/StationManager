using Azure.Core;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.ReceiptFromProduction;
using MES.Application.DTOs.MES.NKTPSX;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INKTPSXQuery
    {
        /// <summary>
        /// Lấy data lệnh sản xuất
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchWOResponse>> GetWO(SearchNKTPSXCommand command);

        /// <summary>
        /// Lấy data nktpsx
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNKTPSXResponse>> GetNKTPSX(SearchNKTPSXCommand command);
    }

    public class NKTPSXQuery : INKTPSXQuery
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<ReceiptFromProductionModel> _rfProdRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<OrderTypeModel> _orderTypeRepo;

        public NKTPSXQuery(IUnitOfWork unitOfWork, IRepository<WorkOrderModel> woRepo, IRepository<ReceiptFromProductionModel> rfProdRepo,
                           IRepository<ProductModel> prodRepo, IRepository<OrderTypeModel> orderTypeRepo)
        {
            _unitOfWork = unitOfWork;
            _woRepo = woRepo;
            _rfProdRepo = rfProdRepo;
            _prodRepo = prodRepo;
            _orderTypeRepo = orderTypeRepo;
        }

        public Task<List<SearchNKTPSXResponse>> GetNKTPSX(SearchNKTPSXCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SearchWOResponse>> GetWO(SearchNKTPSXCommand command)
        {
            #region Format Day

            //Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                command.ScheduledStartFrom = command.ScheduledStartFrom.Value.Date;
            }
            if (command.ScheduledStartTo.HasValue)
            {
                command.ScheduledStartTo = command.ScheduledStartTo.Value.Date.AddDays(1).AddSeconds(-1);
            }

            //Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                command.WeightDateFrom = command.WeightDateFrom.Value.Date;
            }
            if (command.WeightDateTo.HasValue)
            {
                command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Tạo query
            var query = _woRepo.GetQuery(x => x.SystemStatus.StartsWith("REL") &&
                                              !x.SystemStatus.StartsWith("REL CNF") &&
                                              !x.SystemStatus.StartsWith("REL TECO") &&
                                              x.DeletionFlag != "X")
                               .Include(x => x.DetailWorkOrderModel)
                               .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.Plant == command.Plant);
            }

            //Theo Order Type
            if (!string.IsNullOrEmpty(command.OrderType))
            {
                query = query.Where(x => x.OrderTypeCode == command.OrderType);
            }

            //Theo lệnh sản xuát
            if (!string.IsNullOrEmpty(command.WorkOrderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.WorkOrderTo))
                    command.WorkOrderTo = command.WorkOrderFrom;
                query = query.Where(x => x.WorkOrderCode.CompareTo(command.WorkOrderFrom) >= 0 &&
                                         x.WorkOrderCode.CompareTo(command.WorkOrderTo) <= 0);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SaleOrderFrom))
            {

                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SaleOrderTo))
                    command.SaleOrderTo = command.SaleOrderFrom;
                query = query.Where(x => x.SalesOrder.CompareTo(command.SaleOrderFrom) >= 0 &&
                                         x.SalesOrder.CompareTo(command.SaleOrderTo) <= 0);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;
                query = query.Where(x => x.ProductCode.CompareTo(command.MaterialFrom) >= 0 &&
                                         x.ProductCode.CompareTo(command.MaterialTo) <= 0);
            }

            //Theo Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                //Nếu không có To thì search 1
                if (!command.ScheduledStartTo.HasValue)
                    command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.ScheduledStartDate >= command.ScheduledStartFrom &&
                                         x.ScheduledStartDate <= command.ScheduledStartTo);
            }

            //Get query data material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.OrderByDescending(x => x.WorkOrderCode).Select(x => new SearchWOResponse
            {
                //Plant
                Plant = x.Plant,
                //Production Order
                WorkOrder = long.Parse(x.WorkOrderCode).ToString(),
                //Material
                Material = long.Parse(x.ProductCode).ToString(),
                //Material Desc
                MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.ProductCode).ProductName,
                //Storage Location
                Sloc = x.StorageLocation,
                //Batch
                Batch = x.Batch,
                //Total Quantity
                TotalQuantity = x.TargetQuantity,
                //Delivery Quantity
                DeliveryQuantity = x.DeliveredQuantity,
                //UoM
                Unit = x.Unit,
                //Order Type
                OrderType = !string.IsNullOrEmpty(x.OrderTypeCode) ? 
                            $"{x.OrderTypeCode} | {orderTypes.FirstOrDefault(o => o.OrderTypeCode == x.OrderTypeCode).ShortText}" : "",
                //Sales Order
                SalesOrder =x.SalesOrder,
                //Sales order item
                SaleOrderItem = x.SalesOrderItem
            }).ToListAsync();

            return data;
        }
    }
}
