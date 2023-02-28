using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.Commands.NKMHs;
using IntegrationNS.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationNS.Application.Queries
{
    public interface IOutboundDeliveryQuery
    {

        /// <summary>
        /// Get nhập kho hàng trả
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<GoodsReturnResponse>> GetGoodsReturn(NKHTIntegrationCommand command);
    }

    public class OutboundDeliveryQuery : IOutboundDeliveryQuery
    {
        private readonly IRepository<DetailOutboundDeliveryModel> _detailODRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<GoodsReturnModel> _nkhtRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<PlantModel> _plantRepo;

        public OutboundDeliveryQuery(IRepository<DetailOutboundDeliveryModel> detailODRepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                                     IRepository<GoodsReturnModel> nkhtRepo, IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo, IRepository<PlantModel> plantRepo)
        {
            _detailODRepo = detailODRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _nkhtRepo = nkhtRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _plantRepo = plantRepo;
        }

        public async Task<List<GoodsReturnResponse>> GetGoodsReturn(NKHTIntegrationCommand command)
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

            //Ngày cân
            if (command.WeightDateFrom.HasValue)
            {
                command.WeightDateFrom = command.WeightDateFrom.Value.Date;
            }
            if (command.WeightDateTo.HasValue)
            {
                command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            var user = _userRepo.GetQuery().AsNoTracking();

            //Products
            var prods = _prdRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query nkht
            var query = _nkhtRepo.GetQuery().Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery).AsNoTracking();

            //Lọc theo điều kiện
            //Lọc theo plant
            if (!string.IsNullOrEmpty(command.PlantCode))
            {
                query = query.Where(x => x.PlantCode == command.PlantCode);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            {
                if (string.IsNullOrEmpty(command.SalesOrderTo))
                    command.SalesOrderTo = command.SalesOrderFrom;

                query = query.Where(x => x.SalesOrder.CompareTo(command.SalesOrderFrom) >= 0 &&
                                         x.DetailOD.SalesOrder.CompareTo(command.SalesOrderTo) <= 0);
            }

            //Theo outbound deliver
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
                                                                 x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0 : false);
            }

            //Theo ship to party
            if (!string.IsNullOrEmpty(command.ShipToPartyFrom))
            {
                if (string.IsNullOrEmpty(command.ShipToPartyTo))
                    command.ShipToPartyTo = command.ShipToPartyFrom;
                query = query.Where(x => x.ShipToParty.CompareTo(command.ShipToPartyFrom) >= 0 &&
                                         x.ShipToParty.CompareTo(command.ShipToPartyTo) <= 0);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;

                query = query.Where(x => x.MaterialCode.CompareTo(command.MaterialFrom) >= 0 &&
                                         x.MaterialCode.CompareTo(command.MaterialTo) <= 0);
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.DocumentDate >= command.DocumentDateFrom &&
                                         x.DocumentDate <= command.DocumentDateTo);
            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(command.WeightHeadCode))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == command.WeightHeadCode.Trim().ToLower() : false);
            }

            //Check Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                if (!command.WeightDateTo.HasValue) command.WeightDateTo = command.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.CreateTime >= command.WeightDateFrom &&
                                         x.CreateTime <= command.WeightDateTo);
            }

            //Check số phiếu cân
            if (command.WeightVotes != null && command.WeightVotes.Any())
            {
                query = query.Where(x => command.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (command.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == command.CreateBy);
            }

            //Search Status
            if (!string.IsNullOrEmpty(command.Status))
            {
                query = query.Where(x => x.Status == command.Status && x.ReverseDocument == null);
            }

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.CreateTime).Select(x => new GoodsReturnResponse
            {
                GoodsReturnId = x.GoodsReturnId,
                Plant = x.PlantCode,
                ShipToParty = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoParty : "",
                ShipToPartyName = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoPartyName : "",
                OutboundDelivery = x.DetailODId.HasValue ? long.Parse(x.DetailOD.OutboundDelivery.DeliveryCode).ToString() : "",
                OutboundDeliveryItem = x.DetailODId.HasValue ? x.DetailOD.OutboundDeliveryItem : "",
                Material = x.MaterialCode,
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName,
                Sloc = x.SlocCode,
                SlocName = string.IsNullOrEmpty(x.SlocCode) ? "" : $"{x.SlocCode} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.SlocCode).StorageLocationName}",
                Batch = x.Batch,
                SalesOrder = x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument1 : "",
                BagQuantity = x.BagQuantity,
                SingleWeight = x.SingleWeight,
                WeightHeadCode = x.WeightHeadCode,
                Weight = x.Weight,
                ConfirmQty = x.ConfirmQty,
                QtyWithPackage = x.QuantityWithPackaging,
                VehicleCode = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.VehicleCode : "",
                QtyWeight = x.QuantityWeitght,
                TotalQty = x.DetailODId.HasValue ? x.DetailOD.DeliveryQuantity : 0,
                DeliveryQty = x.DetailODId.HasValue ? x.DetailOD.PickedQuantityPUoM : 0,
                UOM = x.UOM,
                Description = x.Description,
                Image = !string.IsNullOrEmpty(x.Image) ? $"https://itp-mes.isdcorp.vn/{x.Image}" : "",
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                WeightVote = x.WeightVote,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                DocumentDate = x.DocumentDate,
                CreateById = x.CreateBy,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                ChangeById = x.LastEditBy,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                MatDoc = x.MaterialDocument,
                ReverseDoc = x.ReverseDocument,
                isDelete = x.Status == "DEL" ? true : false

            }).ToListAsync();

            //Tính open quantity
            foreach (var item in data)
            {
                item.OpenQty = item.TotalQty - item.DeliveryQty;
            }

            return data;

        }
    }
}
