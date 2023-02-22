using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.XKLXH;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XKLXH;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Queries
{
    public interface IXKLXHQuery
    {
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy dữ liệu đầu vào
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchXKLXHCommand command);

        /// <summary>
        /// Lấy dữ liệu đã lưu
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //Task<List<SearchXKLXHResponse>> GetDataXKLXH(SearchXKLXHCommand command);
    }

    public class XKLXHQuery : IXKLXHQuery
    {
        private readonly IRepository<OutboundDeliveryModel> _odRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _dtOdRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;

        public XKLXHQuery(IRepository<OutboundDeliveryModel> odRepo, IRepository<DetailOutboundDeliveryModel> dtOdRepo, IRepository<ProductModel> prodRepo,
                          IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<ExportByCommandModel> xklxhRepo,
                          IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo)
        {
            _odRepo = odRepo;
            _dtOdRepo = dtOdRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
            _xklxhRepo = xklxhRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
        }

        //public async Task<List<SearchXKLXHResponse>> GetDataXKLXH(SearchXKLXHCommand command)
        //{
        //    #region Format Day

        //    if (command.DocumentDateFrom.HasValue)
        //    {
        //        command.DocumentDateFrom = command.DocumentDateFrom.Value.Date;
        //    }
        //    if (command.DocumentDateTo.HasValue)
        //    {
        //        command.DocumentDateTo = command.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
        //    }

        //    //Ngày cân
        //    if (command.WeightDateFrom.HasValue)
        //    {
        //        command.WeightDateFrom = command.WeightDateFrom.Value.Date;
        //    }
        //    if (command.WeightDateTo.HasValue)
        //    {
        //        command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
        //    }
        //    #endregion

        //    var user = _userRepo.GetQuery().AsNoTracking();

        //    //Products
        //    var prods = _prodRepo.GetQuery().AsNoTracking();

        //    //Sloc
        //    var slocs = _slocRepo.GetQuery().AsNoTracking();

        //    //Get query xklxh
        //    var query = _xklxhRepo.GetQuery().Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery).AsNoTracking();

        //    //Lọc điều kiện
        //    //Theo plant
        //    if (!string.IsNullOrEmpty(command.Plant))
        //    {
        //        query = query.Where(x => x.PlantCode == command.Plant);
        //    }

        //    //Theo delivery type
        //    if (!string.IsNullOrEmpty(command.DeliveryType))
        //    {
        //        query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryType == command.DeliveryType : false);
        //    }

        //    //Theo purchase order
        //    if (!string.IsNullOrEmpty(command.PurchaseOrderFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.PurchaseOrderTo))
        //            command.PurchaseOrderTo = command.PurchaseOrderFrom;

        //        query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderFrom) >= 0 &&
        //                                                         x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderTo) <= 0 : false);
        //    }

        //    //Theo sale order
        //    if (!string.IsNullOrEmpty(command.SalesOrderFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.SalesOrderTo))
        //            command.SalesOrderTo = command.SalesOrderFrom;

        //        query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument2.CompareTo(command.SalesOrderFrom) >= 0 &&
        //                                                         x.DetailOD.ReferenceDocument2.CompareTo(command.SalesOrderTo) <= 0 : false);
        //    }

        //    //Theo outbound deliver
        //    if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
        //            command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

        //        query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
        //                                                         x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0 : false);
        //    }

        //    //Theo ship to party
        //    if (!string.IsNullOrEmpty(command.ShipToPartyFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.ShipToPartyTo))
        //            command.ShipToPartyTo = command.ShipToPartyFrom;
        //        query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyFrom) >= 0 &&
        //                                                         x.DetailOD.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyTo) <= 0 : false);
        //    }

        //    //Theo Material
        //    if (!string.IsNullOrEmpty(command.MaterialFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.MaterialTo))
        //            command.MaterialTo = command.MaterialFrom;

        //        query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
        //                                 x.MaterialCodeInt <= long.Parse(command.MaterialTo));
        //    }

        //    //Theo document date
        //    if (command.DocumentDateFrom.HasValue)
        //    {
        //        if (!command.DocumentDateTo.HasValue)
        //        {
        //            command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
        //        }
        //        query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
        //                                                         x.DetailOD.OutboundDelivery.DocumentDate <= command.DocumentDateTo : false);
        //    }

        //    //Search dữ liệu đã cân
        //    if (!string.IsNullOrEmpty(command.WeightHeadCode))
        //    {
        //        query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == command.WeightHeadCode.Trim().ToLower() : false);
        //    }

        //    //Check Ngày thực hiện cân
        //    if (command.WeightDateFrom.HasValue)
        //    {
        //        if (!command.WeightDateTo.HasValue) command.WeightDateTo = command.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
        //        query = query.Where(x => x.CreateTime >= command.WeightDateFrom &&
        //                                 x.CreateTime <= command.WeightDateTo);
        //    }

        //    //Check số phiếu cân
        //    if (command.WeightVotes != null && command.WeightVotes.Any())
        //    {
        //        query = query.Where(x => command.WeightVotes.Contains(x.WeightVote));
        //    }

        //    //Check create by
        //    if (command.CreateBy.HasValue)
        //    {
        //        query = query.Where(x => x.CreateBy == command.CreateBy);
        //    }

        //    //Catalog Nhập kho mua hàng status
        //    var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

        //    var data = await query.OrderByDescending(x => x.CreateTime).Select(x => new SearchXKLXHResponse
        //    {
                
        //        isDelete = x.Status == "DEL" ? true : false,
        //        isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
        //        //isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true

        //    }).ToListAsync();

        //    //Tính open quantity
        //    foreach (var item in data)
        //    {
        //        item.OpenQty = item.TotalQty - item.DeliveryQty;
        //    }

        //    return data;
        //}

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _xklxhRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                       .Select(x => new CommonResponse
                                       {
                                           Key = x.WeightVote,
                                           Value = x.WeightVote
                                       }).Distinct().Take(20).ToListAsync();
        }

        public async Task<List<GetInputDataResponse>> GetInputData(SearchXKLXHCommand command)
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
            var deliveryType = new List<string>() { "ZLF1", "ZLF2", "ZLF3", "ZLF4", "ZLF5", "ZLF6", "ZLF7", "ZLF8", "ZLF9", "ZLFA", "ZNLC", "ZNLN", "ZXDH" };

            //Dữ liệu Outbound Delivery
            var query = _dtOdRepo.GetQuery()
                                        .Include(x => x.OutboundDelivery)
                                        .Where(x => deliveryType.Contains(x.OutboundDelivery.DeliveryType) &&
                                                    x.OutboundDelivery.GoodsMovementSts != "C" &&
                                                    x.GoodsMovementSts != "C")
                                        .AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.OutboundDelivery.ShippingPoint == command.Plant);
            }

            //Theo delivery type
            if (!string.IsNullOrEmpty(command.DeliveryType))
            {
                query = query.Where(x => x.OutboundDelivery.DeliveryType == command.DeliveryType);
            }

            //Theo purchase order
            if (!string.IsNullOrEmpty(command.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(command.PurchaseOrderTo))
                    command.PurchaseOrderTo = command.PurchaseOrderFrom;

                query = query.Where(x => x.ReferenceDocument1.CompareTo(command.PurchaseOrderFrom) >= 0 &&
                                         x.ReferenceDocument1.CompareTo(command.PurchaseOrderTo) <= 0);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            {
                if (string.IsNullOrEmpty(command.SalesOrderTo))
                    command.SalesOrderTo = command.SalesOrderFrom;

                query = query.Where(x => x.ReferenceDocument2.CompareTo(command.SalesOrderFrom) >= 0 &&
                                         x.ReferenceDocument2.CompareTo(command.SalesOrderTo) <= 0);
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

                query = query.Where(x => x.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.ProductCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                         x.OutboundDelivery.DocumentDate <= command.DocumentDateTo);
            }

            //Data input
            var data = await query.Select(x => new GetInputDataResponse
            {
                //Plant
                Plant = x.Plant,
                PlantName = plants.FirstOrDefault(p => p.PlantCode == x.Plant).PlantName,
                //Ship to party name
                ShipToPartyName = x.OutboundDelivery.SoldtoPartyName ?? "",
                //Outbound delivery
                OutboundDelivery = x.OutboundDelivery.DeliveryCodeInt.ToString(),
                //Outbound delivery item
                OutboundDeliveryItem = x.OutboundDeliveryItem,
                //Material
                Material = x.ProductCodeInt.ToString(),
                //Material desc
                MaterialDesc = !string.IsNullOrEmpty(x.ProductCode) ? prods.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName : "",
                //Sloc
                Sloc = x.StorageLocation ?? "",
                //Sloc desc
                SlocDesc = !string.IsNullOrEmpty(x.StorageLocation) ? slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName : "",
                //Batch
                Batch = x.Batch ?? "",
                //Số phương tiện
                VehicleCode = x.OutboundDelivery.VehicleCode ?? "",
                //Total quantity
                TotalQty = x.DeliveryQuantity ?? 0,
                //Delivered quantity
                DeliveredQty = x.PickedQuantityPUoM ?? 0,
                //UOM
                Unit = x.SalesUnit ?? "",
                //Document date
                DocumentDate = x.OutboundDelivery.DocumentDate,
                //Ship to party
                ShipToParty = x.OutboundDelivery.ShiptoParty ?? ""
            }).ToListAsync();

            var index = 1;
            foreach (var item in data)
            {
                item.IndexKey = index;
                index++;
            }

            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
            {
                data.Add(new GetInputDataResponse
                {
                    Plant = command.Plant,
                    Material = long.Parse(command.MaterialFrom).ToString(),
                    MaterialDesc = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName,
                    Unit = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).Unit
                });
            }

            return data;
        }
    }
}
