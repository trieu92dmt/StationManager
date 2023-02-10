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
        //Task <List<SearchWOResponse>> GetInputData(SearchNKPPPPCommand command);

        /// <summary>
        /// Lấy data nktpsx
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //Task<List<SearchNKTPSXResponse>> GetNKTPSX(SearchNKTPSXCommand command);
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);
    }

    public class KPPPPQuery : INKPPPPQuery
    {
        private readonly IRepository<ScrapFromProductionModel> _nkppppRepo;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<DetailWorkOrderModel> _detailWoRepo;

        public KPPPPQuery(IRepository<ScrapFromProductionModel> nkppppRepo, IRepository<WorkOrderModel> woRepo, IRepository<DetailWorkOrderModel> detailWoRepo)
        {
            _nkppppRepo = nkppppRepo;
            _woRepo = woRepo;
            _detailWoRepo = detailWoRepo;
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

        //public Task<List<SearchWOResponse>> GetInputData(SearchNKPPPPCommand command)
        //{
        //    #region Format Day

        //    //Scheduled Start
        //    if (command.ScheduledStartFrom.HasValue)
        //    {
        //        command.ScheduledStartFrom = command.ScheduledStartFrom.Value.Date;
        //    }
        //    if (command.ScheduledStartTo.HasValue)
        //    {
        //        command.ScheduledStartTo = command.ScheduledStartTo.Value.Date.AddDays(1).AddSeconds(-1);
        //    }
        //    #endregion

        //    //Tạo query
        //    var query = _detailWoRepo.GetQuery(x => x.SystemStatus.StartsWith("REL"))
        //                             .Include(x => x.WorkOrder)
        //                             .AsNoTracking();

        //    //Lọc điều kiện
        //    //Theo plant
        //    if (!string.IsNullOrEmpty(command.Plant))
        //    {
        //        query = query.Where(x => x.WorkOrder.Plant == command.Plant);
        //    }

        //    //Theo Material
        //    if (!string.IsNullOrEmpty(command.Material))
        //    {
        //        query = query.Where(x => x.ProductCode == command.Material);
        //    }

        //    //Theo Component
        //    if (!string.IsNullOrEmpty(command.ComponentFrom))
        //    {
        //        //Nếu không có To thì search 1
        //        if (string.IsNullOrEmpty(command.ComponentFrom))
        //            command.ComponentTo = command.ComponentFrom;
        //        query = query.Where(x => x.DetailWorkOrderModel.ProductCode.CompareTo(command.ComponentFrom) >= 0 &&
        //                                 x.ProductCode.CompareTo(command.ComponentTo) <= 0);
        //    }

        //    //Theo Order Type
        //    if (!string.IsNullOrEmpty(command.OrderType))
        //    {
        //        query = query.Where(x => x.OrderTypeCode == command.OrderType);
        //    }

        //    //Theo lệnh sản xuát
        //    if (!string.IsNullOrEmpty(command.WorkOrderFrom))
        //    {
        //        //Nếu không có To thì search 1
        //        if (string.IsNullOrEmpty(command.WorkOrderTo))
        //            command.WorkOrderTo = command.WorkOrderFrom;
        //        query = query.Where(x => x.WorkOrderCode.CompareTo(command.WorkOrderFrom) >= 0 &&
        //                                 x.WorkOrderCode.CompareTo(command.WorkOrderTo) <= 0);
        //    }

        //    //Theo sale order
        //    if (!string.IsNullOrEmpty(command.SaleOrderFrom))
        //    {

        //        //Nếu không có To thì search 1
        //        if (string.IsNullOrEmpty(command.SaleOrderTo))
        //            command.SaleOrderTo = command.SaleOrderFrom;
        //        query = query.Where(x => x.SalesOrder.CompareTo(command.SaleOrderFrom) >= 0 &&
        //                                 x.SalesOrder.CompareTo(command.SaleOrderTo) <= 0);
        //    }


        //    //Theo Scheduled Start
        //    if (command.ScheduledStartFrom.HasValue)
        //    {
        //        //Nếu không có To thì search 1
        //        if (!command.ScheduledStartTo.HasValue)
        //            command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
        //        query = query.Where(x => x.ScheduledStartDate >= command.ScheduledStartFrom &&
        //                                 x.ScheduledStartDate <= command.ScheduledStartTo);
        //    }

        //    //Get query data material
        //    var materials = _prodRepo.GetQuery().AsNoTracking();

        //    //Get query data order type
        //    var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

        //    //Get data
        //    var data = await query.OrderByDescending(x => x.WorkOrderCode).Select(x => new SearchWOResponse
        //    {
        //        //Plant
        //        Plant = x.Plant ?? "",
        //        //Production Order
        //        WorkOrder = long.Parse(x.WorkOrderCode).ToString() ?? "",
        //        //Material
        //        Material = long.Parse(x.ProductCode).ToString() ?? "",
        //        //Material Desc
        //        MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.ProductCode).ProductName ?? "",
        //        //Storage Location
        //        Sloc = x.StorageLocation ?? "",
        //        //Batch
        //        Batch = x.Batch ?? "",
        //        //Total Quantity
        //        TotalQuantity = x.TargetQuantity ?? 0,
        //        //Delivery Quantity
        //        DeliveryQuantity = x.DeliveredQuantity ?? 0,
        //        //UoM
        //        Unit = x.Unit ?? "",
        //        //Order Type
        //        OrderType = !string.IsNullOrEmpty(x.OrderTypeCode) ?
        //                    $"{x.OrderTypeCode} | {orderTypes.FirstOrDefault(o => o.OrderTypeCode == x.OrderTypeCode).ShortText}" : "",
        //        //Sales Order
        //        SalesOrder = x.SalesOrder ?? "",
        //        //Sales order item
        //        SaleOrderItem = x.SalesOrderItem ?? ""
        //    }).ToListAsync();

        //    //Tính open quantity
        //    foreach (var item in data)
        //    {
        //        item.OpenQuantity = item.TotalQuantity - item.DeliveryQuantity;
        //    }

        //    //Thêm dòng trống nếu search theo material
        //    if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
        //    {
        //        data.Add(new SearchWOResponse
        //        {
        //            Plant = command.Plant,
        //            Material = long.Parse(command.MaterialFrom).ToString(),
        //            MaterialDesc = materials.FirstOrDefault(x => x.ProductCode == command.MaterialFrom).ProductName,
        //        });
        //    }

        //    return data;
        //}
    }
}
