using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.NKDCNB;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NKDCNB;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MES.Application.Queries
{
    public interface INKDCNBQuery
    {
        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchNKDCNBCommand command);

        /// <summary>
        /// Lấy data nkdcnb
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNKDCNBResponse>> GetNKDCNB(SearchNKDCNBCommand command);

        /// <summary>
        /// Drop down số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data theo wo
        /// </summary>
        /// <param name="od"></param>
        /// <param name="odItem"></param>
        /// <returns></returns>
        Task<GetDataByOdAndOdItem> GetDataByOdAndOdItem(string od, string odItem);

        /// <summary>
        /// Lấy dropdown od theo điều kiện
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownOutboundDelivery(string keyword);

    }

    public class NKDCNBQuery : INKDCNBQuery
    {
        private readonly IRepository<InhouseTransferModel> _nkdcnbRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailOdRepo;
        private readonly IRepository<PurchaseOrderDetailModel> _detailPoRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<OutboundDeliveryModel> _odRepo;

        public NKDCNBQuery(IRepository<InhouseTransferModel> nkdcnbRepo, IRepository<DetailOutboundDeliveryModel> detailOdRepo,
                           IRepository<PurchaseOrderDetailModel> detailPoRepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                           IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo, IRepository<OutboundDeliveryModel> odRepo)
        {
            _nkdcnbRepo = nkdcnbRepo;
            _detailOdRepo = detailOdRepo;
            _detailPoRepo = detailPoRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _odRepo = odRepo;
        }

        public async Task<GetDataByOdAndOdItem> GetDataByOdAndOdItem(string od, string odItem)
        {
            //Lấy ra od
            var odDetails = await _detailOdRepo.GetQuery().Include(x => x.OutboundDelivery)
                                              .FirstOrDefaultAsync(x => x.OutboundDeliveryItem == odItem && x.OutboundDelivery.DeliveryCodeInt == long.Parse(od));

            if (odDetails == null)
                return null;

            //Danh sách product
            var prods = _prdRepo.GetQuery().AsNoTracking();

            //NKDCNB
            var nkdcnbs = _nkdcnbRepo.GetQuery().AsNoTracking();

            var totalQuantity = odDetails.DeliveryQuantity.HasValue ? odDetails.DeliveryQuantity : 0;
            var deliveryQuantity = nkdcnbs.Where(n => n.DetailODId == odDetails.DetailOutboundDeliveryId).Sum(n => n.ConfirmQty) ?? 0;
            var openQuantity = totalQuantity - deliveryQuantity;

            var response = new GetDataByOdAndOdItem
            {
                //Material
                Material = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(odDetails.ProductCode)).ProductCodeInt.ToString(),
                //Material Desc
                MaterialName = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(odDetails.ProductCode)).ProductName,
                //Batch
                Batch = odDetails.Batch,
                //Số phương tiện
                VehicleCode = odDetails.OutboundDelivery.VehicleCode,
                //Total Quantity
                TotalQty = totalQuantity,
                //Delivered Quantity
                DeliveryQty = deliveryQuantity,
                //Open Quantity
                OpenQty = openQuantity
            };

            return response;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nkdcnbRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }

        public async Task<List<GetInputDataResponse>> GetInputData(SearchNKDCNBCommand command)
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

            //Tạo query detail po
            var poQuery = _detailPoRepo.GetQuery().Include(x => x.PurchaseOrder).AsNoTracking();

            //Tạo query detail od
            var query = _detailOdRepo.GetQuery()
                                        .Include(x => x.OutboundDelivery)
                                        //Lọc delivery type
                                        .Where(x => (x.OutboundDelivery.DeliveryType == "ZNLC" || x.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                    //Lấy delivery đã hoàn tất giao dịch
                                                    x.OutboundDelivery.GoodsMovementSts == "C" &&
                                                    x.GoodsMovementSts == "C")
                                        .AsNoTracking();

            //Check điều kiện 3
                                    //Loại trừ các delivery có po đã hoàn tất nhập kho
            query = query.Where(x => poQuery.FirstOrDefault(p => p.POLine == x.ReferenceItem && p.PurchaseOrder.PurchaseOrderCode == x.ReferenceDocument1).DeliveryCompleted != "X" &&
                                    //Loại trừ các delivery đã đánh dấu xóa
                                     poQuery.FirstOrDefault(p => p.POLine == x.ReferenceItem && p.PurchaseOrder.PurchaseOrderCode == x.ReferenceDocument1).DeletionInd != "L");
            

            //Products
            var prods = _prdRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Plant
            //var plants = _plantRepo.GetQuery().AsNoTracking();

            //NKDCNB
            var nkdcnbs = _nkdcnbRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.OutboundDelivery.ReceivingPlant == command.Plant);
            }

            //Theo shipping point
            if (!string.IsNullOrEmpty(command.ShippingPoint))
            {
                query = query.Where(x => x.OutboundDelivery.ShippingPoint == command.ShippingPoint);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;

                query = query.Where(x => x.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.ProductCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo Purchase order
            if (!string.IsNullOrEmpty(command.PurchaseOrderFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.PurchaseOrderTo))
                    command.PurchaseOrderTo = command.PurchaseOrderFrom;

                query = query.Where(x => x.ReferenceDocument1.CompareTo(command.PurchaseOrderFrom) >= 0 &&
                                         x.ReferenceDocument1.CompareTo(command.PurchaseOrderTo) <= 0);
            }

            //Theo outbound deliver
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

                query = query.Where(x => x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
                                         x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0);
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                //Không có to thì search 1
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                         x.OutboundDelivery.DocumentDate <= command.DocumentDateTo);
            }

            //Get data
            var data = await query.Select(x => new GetInputDataResponse
            {
                //Plant
                Plant = x.OutboundDelivery.ReceivingPlant ?? "",
                //Shipping point
                ShippingPoint = x.Plant ?? "",
                //OD
                OutboundDelivery = x.OutboundDelivery.DeliveryCodeInt.ToString(),
                //OD item
                OutboundDeliveryItem = x.OutboundDeliveryItem,
                //Material
                Material = x.ProductCodeInt.ToString() ?? "",
                //Material Desc
                MaterialDesc = !string.IsNullOrEmpty(x.ProductCode) ? prods.FirstOrDefault(m => m.ProductCodeInt == x.ProductCodeInt).ProductName : "",
                //Storage Location
                Sloc = x.StorageLocation ?? "",
                SlocDesc = string.IsNullOrEmpty(x.StorageLocation) ? "" : $"{x.StorageLocation} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName}",
                //Batch
                Batch = x.Batch ?? "",
                //Total quantity
                TotalQty = x.DeliveryQuantity.HasValue ? x.DeliveryQuantity : 0,
                //Delivery Quantity
                DeliveryQty = nkdcnbs.Where(n => n.DetailODId == x.DetailOutboundDeliveryId).Sum(n => n.ConfirmQty) ?? 0,
                //UoM
                Unit = x.Unit ?? "",
                //Purchase Order
                PurchasOrder = x.ReferenceDocument1 ?? "",
                //Document Date
                DocumentDate = x.OutboundDelivery.DocumentDate
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

        public async Task<List<SearchNKDCNBResponse>> GetNKDCNB(SearchNKDCNBCommand command)
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

            //Tạo query detail po
            var poQuery = _detailPoRepo.GetQuery().Include(x => x.PurchaseOrder).AsNoTracking();

            //Tạo query detail od
            var query = _nkdcnbRepo.GetQuery()
                                        .Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                        //Lọc delivery type
                                        .Where(x => x.DetailODId.HasValue ?
                                                    (x.DetailOD.OutboundDelivery.DeliveryType == "ZNLC" || x.DetailOD.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                    //Lấy delivery đã hoàn tất giao dịch
                                                    (x.DetailOD.OutboundDelivery.GoodsMovementSts == "C") &&
                                                    (x.DetailOD.GoodsMovementSts == "C") : true)
                                        .AsNoTracking();

            //Check điều kiện 3
            //Loại trừ các delivery có po đã hoàn tất nhập kho
            query = query.Where(x => x.DetailODId.HasValue ? poQuery.FirstOrDefault(p => p.POLine == x.DetailOD.ReferenceItem && p.PurchaseOrder.PurchaseOrderCode == x.DetailOD.ReferenceDocument1).DeliveryCompleted != "X" &&
                                                             //Loại trừ các delivery đã đánh dấu xóa
                                                             poQuery.FirstOrDefault(p => p.POLine == x.DetailOD.ReferenceItem && p.PurchaseOrder.PurchaseOrderCode == x.DetailOD.ReferenceDocument1).DeletionInd != "L" : true);


            //Products
            var prods = _prdRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Plant
            //var plants = _plantRepo.GetQuery().AsNoTracking();

            //NKDCNB
            var nkdcnbs = _nkdcnbRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var user = _userRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo shipping point
            if (!string.IsNullOrEmpty(command.ShippingPoint))
            {
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShippingPoint == command.ShippingPoint : false);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;

                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo Purchase order
            if (!string.IsNullOrEmpty(command.PurchaseOrderFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.PurchaseOrderTo))
                    command.PurchaseOrderTo = command.PurchaseOrderFrom;

                query = query.Where(x => x.DetailODId.HasValue ? (x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderFrom) >= 0 &&
                                                                 x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderTo) <= 0) : false);
            }

            //Theo outbound deliver
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

                query = query.Where(x => x.DetailODId.HasValue ? (x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
                                                                 x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0) : false);
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                //Không có to thì search 1
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.DetailODId.HasValue ? (x.DetailOD.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                                                 x.DetailOD.OutboundDelivery.DocumentDate <= command.DocumentDateTo) : false);

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

            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchNKDCNBResponse
            {
                //Id
                NKDCNBId = x.InhouseTransferId,
                //Plant
                Plant = x.PlantCode ?? "",
                //Shipping point
                ShippingPoint = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShippingPoint : "",
                //OD
                OutboundDelivery = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCodeInt.ToString() : "",
                //OD item
                OutboundDeliveryItem = x.DetailODId.HasValue ? x.DetailOD.OutboundDeliveryItem : "",
                //Material
                Material = x.MaterialCodeInt.ToString() ?? "",
                //Material Desc
                MaterialDesc = prods.FirstOrDefault(m => m.ProductCodeInt == x.MaterialCodeInt).ProductName ?? "",
                //Storage Location
                Sloc = x.SlocCode ?? "",
                SlocDesc = x.SlocName ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //Số lượng bao
                BagQuantity = x.BagQuantity ?? 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QtyWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QtyWeight = x.QuantityWeitght ?? 0,
                //Total quantity
                TotalQty = x.DetailODId.HasValue ? x.DetailOD.DeliveryQuantity : 0,
                //Delivery Quantity
                DeliveryQty = nkdcnbs.Where(n => n.DetailODId == x.DetailODId).Sum(n => n.ConfirmQty) ?? 0,
                //UoM
                Unit = x.UOM ?? "",
                //Purchase Order
                PurchaseOrder = x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument1 : "",
                //Số xe tải
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? x.Image : "",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote ?? "",
                //Thời gian bắt đầu
                StartTime = x.StartTime,
                //Thời gian kết thúc
                EndTime = x.EndTime,
                //Document Date
                DocumentDate = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate : null,
                //Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //Crete On
                CreateOn = x.CreateTime ?? null,
                //Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //Material Doc
                MatDoc = x.MaterialDocument ?? null,
                //Reverse Doc
                RevDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
                //isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true
            }).ToListAsync();

            return data;
        }
        #region Dropdown Outbound Delivery
        public async Task<List<CommonResponse>> GetDropdownOutboundDelivery(string keyword)
        {

            return await _odRepo.GetQuery().Include(x => x.DetailOutboundDeliveryModel)
                                .Where(x => (string.IsNullOrEmpty(keyword) ? true : x.DeliveryCode.Trim().ToLower().Contains(keyword.Trim().ToLower())) &&
                                            (x.DeliveryType == "ZNLC" || x.DeliveryType == "ZNLN") &&
                                            //Lấy delivery đã hoàn tất giao dịch
                                            x.GoodsMovementSts == "C" &&
                                            x.DetailOutboundDeliveryModel.FirstOrDefault(p => p.GoodsMovementSts == "C") != null)
                                         .OrderBy(x => x.DeliveryCode)
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.DeliveryCode,
                                             Value = x.DeliveryCode
                                         }).Take(10).ToListAsync();
        }
        #endregion
    }
}
