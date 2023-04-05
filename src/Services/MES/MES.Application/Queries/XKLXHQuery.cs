using Core.Extensions;
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
        Task<List<SearchXKLXHResponse>> GetDataXKLXH(SearchXKLXHCommand command);

        /// <summary>
        /// Lấy data theo od và oditem 
        /// </summary>
        /// <param name="ODCode"></param>
        /// <param name="ODItem"></param>
        /// <returns></returns>
        Task<GetDataByODODItemResponse> GetDataByODODItem(string outboundDelivery, string outboundDeliveryItem);
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
        private readonly IRepository<ScaleModel> _scaleRepo;

        public XKLXHQuery(IRepository<OutboundDeliveryModel> odRepo, IRepository<DetailOutboundDeliveryModel> dtOdRepo, IRepository<ProductModel> prodRepo,
                          IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<ExportByCommandModel> xklxhRepo,
                          IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo, IRepository<ScaleModel> scaleRepo)
        {
            _odRepo = odRepo;
            _dtOdRepo = dtOdRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
            _xklxhRepo = xklxhRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
            _scaleRepo = scaleRepo;
        }

        public async Task<GetDataByODODItemResponse> GetDataByODODItem(string outboundDelivery, string outboundDeliveryItem)
        {
            //Lấy ra od
            var odDetails = await _dtOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                              .FirstOrDefaultAsync(x => x.OutboundDeliveryItem == outboundDeliveryItem && x.OutboundDelivery.DeliveryCodeInt == long.Parse(outboundDelivery));

            if (odDetails == null)
                return null;

            //Danh sách product
            var prods = _prodRepo.GetQuery().AsNoTracking();

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
                //DocumentDate
                DocumentDate = odDetails.OutboundDelivery.DocumentDate
            };

            return response;
        }

        public async Task<List<SearchXKLXHResponse>> GetDataXKLXH(SearchXKLXHCommand command)
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
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query xklxh
            var query = _xklxhRepo.GetQuery()
                                  .Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery).AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo delivery type
            if (!string.IsNullOrEmpty(command.DeliveryType))
            {
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryType == command.DeliveryType : false);
            }

            //Theo purchase order
            if (!string.IsNullOrEmpty(command.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(command.PurchaseOrderTo))
                    command.PurchaseOrderTo = command.PurchaseOrderFrom;

                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderFrom) >= 0 &&
                                                                 x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderTo) <= 0 : false);
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
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyFrom) >= 0 &&
                                                                 x.DetailOD.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyTo) <= 0 : false);
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
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                                                 x.DetailOD.OutboundDelivery.DocumentDate <= command.DocumentDateTo : false);
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

            //Scale
            var scale = _scaleRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.CreateTime).Select(x => new SearchXKLXHResponse
            {
                //Id
                XKLXHId = x.ExportByCommandId,
                //Plant
                Plant = x.PlantCode,
                //Ship to party name
                ShipToPartyName = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoPartyName : "",
                //Od
                OutboundDelivery = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCode : "",
                //Od item
                OutboundDeliveryItem = x.DetailODId.HasValue ? x.DetailOD.OutboundDeliveryItem : "",
                //Material
                Material = x.MaterialCodeInt.ToString(),
                //Material desc
                MaterialDesc = !string.IsNullOrEmpty(x.MaterialCode) ? prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName : "",
                //Sloc
                Sloc = x.SlocCode ?? "",
                SlocName = x.SlocName ?? "",
                //Batch
                Batch = string.IsNullOrEmpty(x.MaterialDocument) ? x.DetailODId.HasValue ? x.DetailOD.Batch : "" : x.Batch ?? "",
                //Sl bao
                BagQuantity = x.BagQuantity ?? 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                ScaleType = !string.IsNullOrEmpty(x.WeightHeadCode) ? scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).isCantai == true ? "CANXETAI" :
                                                                      scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).ScaleType == true ? "TICHHOP" : "KHONGTICHHOP" : "KHONGTICHHOP",
                //Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeight ?? 0,
                //Total quantity
                TotalQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : x.DetailODId.HasValue ? x.DetailOD.DeliveryQuantity : 0,
                //Delivered quantity
                DeliveredQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.DeliveryQuantity : x.DetailODId.HasValue ? x.DetailOD.PickedQuantityPUoM : 0,
                //UoM
                Unit = x.UOM ?? "",
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"{new ConfigManager().DomainUploadUrl}{x.Image}" : "",
                //Trạng thái
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote,
                //Thời gian bắt đầu
                StartTime = x.StartTime,
                //Thời gian kết thúc
                EndTime = x.EndTime,
                //Document date
                DocumentDate = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate : null,
                //Số xe tải
                TruckInfoId = x.TruckInfoId ?? null,
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //Trọng lượng hàng hóa
                GoodsWeight = x.GoodsWeight ?? 0,
                //Create by
                CreateById = x.CreateBy,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //Create on
                CreateOn = x.CreateTime,
                //Change by
                ChangeById = x.LastEditBy,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                ChangeOn = x.LastEditTime,
                //Material doc
                MatDoc = x.MaterialDocument,
                //Reverse doc
                RevDoc = x.ReverseDocument,
                //Đánh dấu xóa
                isDelete = x.Status == "DEL" ? true : false,
                //Được chỉnh sửa
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true

            }).ToListAsync();

            return data;
        }

        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword">Từ khóa</param>
        /// <returns></returns>
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
                query = query.Where(x => x.Plant == command.Plant);
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

            //Data input
            var data = await query.Select(x => new GetInputDataResponse
            {
                Id = Guid.NewGuid(),
                //Plant
                Plant = x.Plant,
                PlantName = plants.FirstOrDefault(p => p.PlantCode == x.Plant).PlantName,
                //Ship to party name
                ShipToPartyName = x.OutboundDelivery.ShiptoPartyName ?? "",
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
                SlocName = !string.IsNullOrEmpty(x.StorageLocation) ? slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName : "",
                //Batch
                Batch = x.Batch ?? "",
                //Số phương tiện
                VehicleCode = x.OutboundDelivery.VehicleCode ?? "",
                //Total quantity
                TotalQty = x.DeliveryQuantity,
                //Delivered quantity
                DeliveredQty = x.PickedQuantityPUoM,
                //UOM
                Unit = x.SalesUnit ?? "",
                //Document date
                DocumentDate = x.OutboundDelivery.DocumentDate,
                //Ship to party
                ShipToParty = x.OutboundDelivery.ShiptoParty ?? ""
            }).ToListAsync();

            var index = 1;
            //Gán STT
            foreach (var item in data)
            {
                item.IndexKey = index;
                index++;
            }

            //Nếu không có chứng từ SAP và có search theo material
            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo && data.Count == 0)
            {
                data.Add(new GetInputDataResponse
                {
                    //Mã nhà máy
                    Plant = command.Plant,
                    //Material
                    Material = long.Parse(command.MaterialFrom).ToString(),
                    //Material desc
                    MaterialDesc = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName,
                    //Đơn vị
                    Unit = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).Unit
                });
            }

            return data;
        }
    }
}
