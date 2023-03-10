using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.OutboundDelivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        /// <summary>
        /// Lấy data theo od và oditem 
        /// </summary>
        /// <param name="ODCode"></param>
        /// <param name="ODItem"></param>
        /// <returns></returns>
        Task<GetDataByODODItemResponse> GetDataByODODItem(string ODCode, string ODItem);

        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);
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
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<Document_Image_Mapping> _docImgRepo;

        public OutboundDeliveryQuery(IRepository<DetailOutboundDeliveryModel> detailODRepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                                     IRepository<GoodsReturnModel> nkhtRepo, IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo, IRepository<PlantModel> plantRepo,
                                     IRepository<ScaleModel> scaleRepo, IRepository<Document_Image_Mapping> docImgRepo)
        {
            _detailODRepo = detailODRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _nkhtRepo = nkhtRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _plantRepo = plantRepo;
            _scaleRepo = scaleRepo;
            _docImgRepo = docImgRepo;
            _docImgRepo = docImgRepo;
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

            //Plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

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

                query = query.Where(x => x.ReferenceDocument1.CompareTo(command.SalesOrderFrom) >= 0 &&
                                         x.ReferenceDocument1.CompareTo(command.SalesOrderTo) <= 0);
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


            //Data NKHT
            var data = await query.Select(x => new OutboundDeliveryResponse
            {
                Id = Guid.NewGuid(),
                Plant = x.Plant,
                PlantName = plants.FirstOrDefault(p => p.PlantCode == x.Plant).PlantName,
                ShipToParty = x.OutboundDelivery.ShiptoParty,
                ShipToPartyName = x.OutboundDelivery.ShiptoPartyName,
                DetailODId = x.DetailOutboundDeliveryId,
                OutboundDelivery = long.Parse(x.OutboundDelivery.DeliveryCode).ToString(),
                OutboundDeliveryItem = x.OutboundDeliveryItem,
                Material = long.Parse(x.ProductCode).ToString(),
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                Sloc = x.StorageLocation,
                SlocDesc = slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName,
                Batch = x.Batch,
                VehicleCode = x.OutboundDelivery.VehicleCode,
                TotalQty = x.DeliveryQuantity,
                DeliveryQty = x.PickedQuantityPUoM,
                Unit = x.Unit,
                DocumentDate = x.OutboundDelivery.DocumentDate,
                
            }).ToListAsync();

            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo && data.Count == 0)
            {
                data.Add(new OutboundDeliveryResponse
                {
                    Plant = command.PlantCode,
                    Material = long.Parse(command.MaterialFrom).ToString(),
                    MaterialDesc = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName,
                    Unit = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).Unit
                });
            }

            return data;
        }


        public async Task<List<GoodsReturnResponse>> GetGoodsReturn(SearchOutboundDeliveryCommand command)
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

            //Img mapping
            var imgMappings = _docImgRepo.GetQuery().AsNoTracking();

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

                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument1.CompareTo(command.SalesOrderFrom) >= 0 &&
                                         x.DetailOD.ReferenceDocument1.CompareTo(command.SalesOrderTo) <= 0 : false);
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

                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
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

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var scale = _scaleRepo.GetQuery().AsNoTracking();

            var data = await query.OrderByDescending(x => x.CreateTime).Select(x => new GoodsReturnResponse
            {
                GoodsReturnId = x.GoodsReturnId,
                //Plant
                Plant = x.PlantCode,
                //Ship to party
                ShipToParty = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoParty : "",
                ShipToPartyName = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoPartyName : "",
                //od
                OutboundDelivery = x.DetailODId.HasValue ? long.Parse(x.DetailOD.OutboundDelivery.DeliveryCode).ToString() : null,
                //od item
                OutboundDeliveryItem = x.DetailODId.HasValue ? x.DetailOD.OutboundDeliveryItem : null,
                //Material code
                Material = long.Parse(x.MaterialCode).ToString(),
                //Material
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName,
                //Sloc code
                Sloc = x.SlocCode,
                //Sloc name
                SlocName = string.IsNullOrEmpty(x.SlocCode) ? "" : $"{x.SlocCode} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.SlocCode).StorageLocationName}",
                //Số batch
                Batch = x.Batch ?? "",
                //Số lượng bao
                BagQuantity = x.BagQuantity,
                //Đơn trọng
                SingleWeight = x.SingleWeight,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode,
                ScaleType = !string.IsNullOrEmpty(x.WeightHeadCode) ? scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).isCantai == true ? "CANXETAI" :
                                                                      scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).ScaleType == true ? "TICHHOP" : "KHONGTICHHOP" : "KHONGTICHHOP",
                //Trọng lượng cân
                Weight = x.Weight,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty,
                //Sl kèm bao bì
                QtyWithPackage = x.QuantityWithPackaging,
                //Số phương tiện
                VehicleCode = x.VehicleCode,
                //Số lần cân
                QtyWeight = x.QuantityWeitght,
                TotalQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : x.DetailODId.HasValue ? x.DetailOD.DeliveryQuantity : 0,
                DeliveryQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.DeliveredQuantity : x.DetailODId.HasValue ? x.DetailOD.PickedQuantityPUoM : 0,
                //UOM
                UOM = x.UOM,
                //ghi chú
                Description = x.Description,
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"https://itp-mes.isdcorp.vn/{x.Image}" : "",
                ListImage = imgMappings.Where(img => img.DocumentId == x.GoodsReturnId).Select(img => img.Image).ToList(),
                //Trạng thái
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                WeightVote = x.WeightVote,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                DocumentDate = x.DocumentDate,
                CreateById = x.CreateBy,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                CreateOn = x.CreateTime,
                ChangeById = x.LastEditBy,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                ChangeOn = x.LastEditTime,
                MatDoc = x.MaterialDocument,
                ReverseDoc = x.ReverseDocument,
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
                //isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true

            }).ToListAsync();

            return data;

        }

        public async Task<GetDataByODODItemResponse> GetDataByODODItem(string ODCode, string ODItem)
        {
            //Lấy ra od
            var odDetails = await _detailODRepo.GetQuery().Include(x => x.OutboundDelivery)
                                              .FirstOrDefaultAsync(x => x.OutboundDeliveryItem == ODItem && x.OutboundDelivery.DeliveryCodeInt == long.Parse(ODCode));

            if (odDetails == null)
                return null;

            //Danh sách product
            var prods = _prdRepo.GetQuery().AsNoTracking();

            var response = new GetDataByODODItemResponse
            {
                //Ship to party
                ShipToPartyName = odDetails.OutboundDelivery.ShiptoPartyName,
                //Material
                Material = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(odDetails.ProductCode)).ProductCodeInt.ToString(),
                //Material Desc
                MaterialName = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(odDetails.ProductCode)).ProductName,
                //Batch
                Batch = odDetails.Batch,
                //Số phương tiện
                VehicleCode = odDetails.OutboundDelivery.VehicleCode,
                //Total Quantity
                TotalQty = odDetails.DeliveryQuantity,
                //Delivered Quantity
                DeliveryQty = odDetails.PickedQuantityPUoM,
                //Open Quantity
                OpenQty = odDetails.DeliveryQuantity - odDetails.PickedQuantityPUoM
            };

            return response;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nkhtRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }
    }
}
