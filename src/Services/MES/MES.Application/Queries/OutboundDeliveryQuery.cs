using Azure.Core;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.DTOs.MES.OutboundDelivery;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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

        /// <summary>
        /// Get nhập kho hàng trả
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<GoodsReturnResponse>> GetGoodsReturn(SearchOutboundDeliveryCommand command);
    }

    public class OutboundDeliveryQuery : IOutboundDeliveryQuery
    {
        private readonly IRepository<DetailOutboundDeliveryModel> _detailODRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<GoodsReturnModel> _nkhtRepo;

        public OutboundDeliveryQuery(IRepository<DetailOutboundDeliveryModel> detailODRepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                                     IRepository<GoodsReturnModel> nkhtRepo)
        {
            _detailODRepo = detailODRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _nkhtRepo = nkhtRepo;
        }

        public async Task<List<OutboundDeliveryResponse>> GetOutboundDelivery(SearchOutboundDeliveryCommand command)
        {
            #region Format Day

            if (command.DocumentDateFrom.HasValue)
            {
                command.DocumentDateFrom = command.DocumentDateFrom.Value.Date;
            }
            if (command.DocumentDateTo.HasValue)
            {
                command.DocumentDateTo = command.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Delivery Type lấy ra
            var deliveryType = new List<string>() { "ZLR1", "ZLR2", "ZLR3", "ZLR4", "ZLR5", "ZLR6", "ZNDH" };

            //Dữ liệu Outbound Delivery
            var query = _detailODRepo.GetQuery()
                                        .Include(x => x.OutboundDelivery)
                                        .Where(x => deliveryType.Contains(x.OutboundDelivery.DeliveryType) &&
                                                    x.GoodsMovementSts != "C")
                                        .AsNoTracking();

            //Products
            var prods = _prdRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.PlantCode))
            {
                query = query.Where(x => x.Plant == command.PlantCode);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            {
                if (string.IsNullOrEmpty(command.SalesOrderTo))
                    command.SalesOrderTo = command.SalesOrderFrom;

                query = query.Where(x => x.SalesOrder.CompareTo(command.SalesOrderFrom) >= 0 &&
                                         x.SalesOrder.CompareTo(command.SalesOrderTo) <= 0);
            }

            //Theo outbound deliver
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

                query = query.Where(x => x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
                                         x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0);
            }

            //Theo ship to party
            if (!string.IsNullOrEmpty(command.ShipToPartyFrom))
            {
                if (string.IsNullOrEmpty(command.ShipToPartyTo))
                    command.ShipToPartyTo = command.ShipToPartyFrom;
                query = query.Where(x => x.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyFrom) >= 0 &&
                                         x.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyTo) <= 0);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;

                query = query.Where(x => x.ProductCode.CompareTo(command.MaterialFrom) >= 0 &&
                                         x.ProductCode.CompareTo(command.MaterialTo) <= 0);
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom;
                }    
                query = query.Where(x => x.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                         x.OutboundDelivery.DocumentDate <= command.DocumentDateTo);
            }

            //Data NKMH
            var data = await query.Select(x => new OutboundDeliveryResponse
            {
                Plant = x.Plant,
                ShipToParty = x.OutboundDelivery.ShiptoParty,
                ShipToPartyName = x.OutboundDelivery.ShiptoPartyName,
                OutboundDelivery = long.Parse(x.OutboundDelivery.DeliveryCode).ToString(),
                OutboundDeliveryItem = x.OutboundDeliveryItem,
                Material = long.Parse(x.ProductCode).ToString(),
                MaterialDesc = x.ItemDescription,
                Sloc = x.StorageLocation,
                SlocDesc = slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName,
                Batch = x.Batch,
                VehicleCode = x.OutboundDelivery.VehicleCode,
                TotalQty = x.DeliveryQuantity,
                DeliveryQty = x.PickedQuantityPUoM,
                Unit = x.Unit,
                DocumentDate = x.OutboundDelivery.DocumentDate,
                
            }).ToListAsync();

            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
            {
                data.Add(new OutboundDeliveryResponse
                {
                    Plant = command.PlantCode,
                    Material = command.MaterialFrom,
                    MaterialDesc = prods.FirstOrDefault(x => x.ProductCode == command.MaterialFrom).ProductName,
                });
            }

            return data;
        }


        public Task<List<GoodsReturnResponse>> GetGoodsReturn(SearchOutboundDeliveryCommand command)
        {
            #region Format Day

            if (command.DocumentDateFrom.HasValue)
            {
                command.DocumentDateFrom = command.DocumentDateFrom.Value.Date;
            }
            if (command.DocumentDateTo.HasValue)
            {
                command.DocumentDateTo = command.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Get query nkht
            var query = _nkhtRepo.GetQuery().AsNoTracking();

            //Lọc theo plant
            if (!string.IsNullOrEmpty(command.PlantCode))
            {
                query = query.Where(x => x.PlantCode == command.PlantCode);
            }

            //Lọc theo sale order

            return null;

        }
    }
}
