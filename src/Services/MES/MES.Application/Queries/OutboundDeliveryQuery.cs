using Azure.Core;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.DTOs.MES.OutboundDelivery;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface IOutboundDeliveryQuery
    {
        /// <summary>
        /// Get Outbound Delivery
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<OutboundDeliveryResponse>> GetOutboundDelivery(SearchOutboundDeliveryCommand command);
    }

    //public class OutboundDeliveryQuery : IOutboundDeliveryQuery
    //{
    //    private readonly IRepository<DetailOutboundDeliveryModel> _detailODRepo;

    //    public OutboundDeliveryQuery(IRepository<DetailOutboundDeliveryModel> detailODRepo)
    //    {
    //        _detailODRepo = detailODRepo;
    //    }

    //    //public async Task<List<OutboundDeliveryResponse>> GetOutboundDelivery(SearchOutboundDeliveryCommand command)
    //    //{
    //    //    #region Format Day

    //    //    if (command.DocumentDateFrom.HasValue)
    //    //    {
    //    //        command.DocumentDateFrom = command.DocumentDateFrom.Value.Date;
    //    //    }
    //    //    if (command.DocumentDateTo.HasValue)
    //    //    {
    //    //        command.DocumentDateTo = command.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
    //    //    }
    //    //    #endregion

    //    //    //Dữ liệu Outbound Delivery
    //    //    var query = await _detailODRepo.GetQuery()
    //    //                                .Include(x => x.OutboundDelivery)
    //    //                                .AsNoTracking().ToListAsync();

    //    //    //Lọc điều kiện
    //    //    //Theo plant
    //    //    if (!string.IsNullOrEmpty(command.PlantCode))
    //    //    {
    //    //        query = query.Where(x => x.Plant == command.PlantCode).ToList();
    //    //    }

    //    //    //Theo sale order
    //    //    if (!string.IsNullOrEmpty(command.SalesOrderFrom))
    //    //    {
    //    //        if (string.IsNullOrEmpty(command.SalesOrderTo))
    //    //            command.SalesOrderTo = command.SalesOrderFrom;

    //    //        query = query.Where(x => long.Parse(x.SalesOrder) == )
    //    //    }
            
    //    //}
    //}
}
