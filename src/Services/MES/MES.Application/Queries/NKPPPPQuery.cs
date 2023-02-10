using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.NKPPPP;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NKPPPP;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INKPPPPQuery
    {
        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task <List<GetDataInputResponse>> GetInputData(SearchNKPPPPCommand command);

        /// <summary>
        /// Lấy data nkpppp
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNKPPPPResponse>> GetNKPPPP(SearchNKPPPPCommand command);
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data theo wo
        /// </summary>
        /// <param name="workorder"></param>
        /// <returns></returns>
        Task<GetDataByWoResponse> GetDataByWo(string workorder);
    }

    public class KPPPPQuery : INKPPPPQuery
    {
        private readonly IRepository<ScrapFromProductionModel> _nkppppRepo;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<DetailWorkOrderModel> _detailWoRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<OrderTypeModel> _orderTypeRepo;

        public KPPPPQuery(IRepository<ScrapFromProductionModel> nkppppRepo, IRepository<WorkOrderModel> woRepo, IRepository<DetailWorkOrderModel> detailWoRepo,
                          IRepository<ProductModel> prdRepo, IRepository<OrderTypeModel> orderTypeRepo)
        {
            _nkppppRepo = nkppppRepo;
            _woRepo = woRepo;
            _detailWoRepo = detailWoRepo;
            _prdRepo = prdRepo;
            _orderTypeRepo = orderTypeRepo;
        }

        public Task<GetDataByWoResponse> GetDataByWo(string workorder)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nkppppRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }

        public async Task<List<GetDataInputResponse>> GetInputData(SearchNKPPPPCommand command)
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
            #endregion

            //Tạo query
            var query = _detailWoRepo.GetQuery(x => x.SystemStatus.StartsWith("REL"))
                                     .Include(x => x.WorkOrder)
                                     .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.WorkOrder.Plant == command.Plant);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.Material))
            {
                query = query.Where(x => x.WorkOrder.ProductCode == command.Material);
            }

            //Theo Component
            if (!string.IsNullOrEmpty(command.ComponentFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.ComponentFrom))
                    command.ComponentTo = command.ComponentFrom;
                query = query.Where(x => x.ProductCode.CompareTo(command.ComponentFrom) >= 0 &&
                                         x.ProductCode.CompareTo(command.ComponentTo) <= 0);
            }

            //Theo lệnh sản xuát
            if (!string.IsNullOrEmpty(command.WorkorderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.WorkorderTo))
                    command.WorkorderTo = command.WorkorderFrom;
                query = query.Where(x => x.WorkOrder.WorkOrderCode.CompareTo(command.WorkorderFrom) >= 0 &&
                                         x.WorkOrder.WorkOrderCode.CompareTo(command.WorkorderTo) <= 0);
            }

            //Theo Order Type
            if (!string.IsNullOrEmpty(command.OrderTypeFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.OrderTypeFrom))
                    command.SalesOrderTo = command.OrderTypeTo;
                query = query.Where(x => x.WorkOrder.OrderTypeCode.CompareTo(command.OrderTypeFrom) >= 0 &&
                                         x.WorkOrder.OrderTypeCode.CompareTo(command.OrderTypeTo) <= 0);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            {

                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SalesOrderTo))
                    command.SalesOrderTo = command.SalesOrderFrom;
                query = query.Where(x => x.WorkOrder.SalesOrder.CompareTo(command.SalesOrderFrom) >= 0 &&
                                         x.WorkOrder.SalesOrder.CompareTo(command.SalesOrderTo) <= 0);
            }


            //Theo Scheduled Start
            if (command.ScheduledStartFrom.HasValue)
            {
                //Nếu không có To thì search 1
                if (!command.ScheduledStartTo.HasValue)
                    command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.WorkOrder.ScheduledStartDate >= command.ScheduledStartFrom &&
                                         x.WorkOrder.ScheduledStartDate <= command.ScheduledStartTo);
            }

            //Get query data material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Get query data order type
            var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.Select(x => new GetDataInputResponse
            {
                //Plant
                Plant = x.WorkOrder.Plant ?? "",
                //Production Order
                WorkOrder = long.Parse(x.WorkOrder.WorkOrderCode).ToString() ?? "",
                //Material
                Material = long.Parse(x.WorkOrder.ProductCode).ToString() ?? "",
                //Material Desc
                MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.WorkOrder.ProductCode).ProductName ?? "",
                //Component
                Component = long.Parse(x.ProductCode).ToString() ?? "",
                //Component Desc
                ComponentDesc = materials.FirstOrDefault(m => m.ProductCode == x.ProductCode).ProductName ?? "",
                //Order Type
                OrderType = !string.IsNullOrEmpty(x.WorkOrder.OrderTypeCode) ?
                            $"{x.WorkOrder.OrderTypeCode} | {orderTypes.FirstOrDefault(o => o.OrderTypeCode == x.WorkOrder.OrderTypeCode).ShortText}" : "",
                //Sales Order
                SalesOrder = x.WorkOrder.SalesOrder ?? "",
                //Storage Location
                Sloc = x.StorageLocation ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //UoM
                Unit = x.WorkOrder.Unit ?? "",
                //Schedule Start Time
                ScheduleStartTime = x.WorkOrder.ScheduledStartDate ?? null,
                //Schedule Finish Time
                ScheduleFinishTime = x.WorkOrder.ScheduledFinishDate ?? null,
                //Requirement Qty
                RequirementQty = x.RequirementQuantiy ?? 0,
                //Withdraw Qty
                WithdrawQty = x.QuantityWithdrawn ?? 0
            }).ToListAsync();

            var index = 1;
            foreach (var item in data)
            {
                item.IndexKey = index;
                index++;
            }

            //Thêm dòng trống nếu search theo material
            if (!string.IsNullOrEmpty(command.ComponentFrom) && command.ComponentFrom == command.ComponentTo)
            {
                data.Add(new GetDataInputResponse
                {
                    Plant = command.Plant,
                    Component = long.Parse(command.ComponentFrom).ToString(),
                    ComponentDesc = materials.FirstOrDefault(x => x.ProductCode == command.ComponentFrom).ProductName,
                });
            }

            return data;
        }

        public Task<List<SearchNKPPPPResponse>> GetNKPPPP(SearchNKPPPPCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
