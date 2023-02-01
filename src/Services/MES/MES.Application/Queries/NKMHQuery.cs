using ISD.Core.Exceptions;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.MES;
using MES.Application.DTOs.MES;
using MES.Application.DTOs.MES.NKMH;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MES.Application.Queries
{
    public interface INKMHQuery
    {
        /// <summary>
        /// Get Nhập kho mua hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<ListNKMHResponse>> GetNKMHAsync(GetNKMHCommand request);

        /// <summary>
        /// Get PO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<PuchaseOrderNKMHResponse>> GetPOAsync(GetNKMHCommand request);

        /// <summary>
        /// Lấy số cân
        /// </summary>
        /// <param name="weightHead"></param>
        /// <returns></returns>
        Task<GetWeighNumResponse> GetWeighNum(string scaleCode);

        /// <summary>
        /// Lấy data theo po và poitem
        /// </summary>
        /// <param name="po"></param>
        /// <param name="poItem"></param>
        /// <returns></returns>
        Task<GetDataByPoPoItemResponse> GetDataByPoAndPoItem(string po, string poItem);
    }
    public class NKMHQuery : INKMHQuery
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<ProductModel> _prdRep;
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;
        private readonly IRepository<AccountModel> _userRep;
        private readonly IRepository<VendorModel> _vendorRep;
        private readonly IRepository<WeighSessionModel> _weighSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public NKMHQuery(IRepository<GoodsReceiptModel> nkmhRep, IRepository<ProductModel> prdRep, IRepository<PurchaseOrderMasterModel> poRep,
                         IRepository<PurchaseOrderDetailModel> poDetailRep, IRepository<AccountModel> userRep, IRepository<VendorModel> vendorRep,
                         IRepository<WeighSessionModel> weighSsRepo, IRepository<ScaleModel> scaleRepo, IRepository<CatalogModel> cataRepo,
                         IRepository<StorageLocationModel> slocRepo)
        {
            _nkmhRep = nkmhRep;
            _prdRep = prdRep;
            _poRep = poRep;
            _poDetailRep = poDetailRep;
            _userRep = userRep;
            _vendorRep = vendorRep;
            _weighSsRepo = weighSsRepo;
            _scaleRepo = scaleRepo;
            _cataRepo = cataRepo;
            _slocRepo = slocRepo;
        }


        public async Task<List<ListNKMHResponse>> GetNKMHAsync(GetNKMHCommand request)
        {
            #region Format Day

            //Ngày phát lệnh
            if (request.DocumentDateFrom.HasValue)
            {
                request.DocumentDateFrom = request.DocumentDateFrom.Value.Date;
            }
            if (request.DocumentDateTo.HasValue)
            {
                request.DocumentDateTo = request.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            var user = _userRep.GetQuery().AsNoTracking();

            //Danh sách sloc
            var slocs = _slocRepo.GetQuery(x => x.PlantCode == request.Plant).AsNoTracking();

            //Product
            var product = await _prdRep.GetQuery().AsNoTracking().ToListAsync();

            var queryNKMH = await _nkmhRep.GetQuery()
                                    .Include(x => x.PurchaseOrderDetail)
                                    .ThenInclude(x => x.PurchaseOrder)
                                    .AsNoTracking().ToListAsync();

            if (!string.IsNullOrEmpty(request.Plant))
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? false : x.PurchaseOrderDetail.PurchaseOrder.Plant.Contains(request.Plant)).ToList();
            }
            if (!string.IsNullOrEmpty(request.PurchasingOrgFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingOrgTo)) request.PurchasingOrgTo = request.PurchasingOrgFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail.PurchaseOrder.PurchasingOrg == request.PurchasingOrgFrom).ToList();
            }
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? false : long.Parse(x.PurchaseOrderDetail.PurchaseOrder.VendorCode) >= long.Parse(request.VendorFrom) &&
                                                                                        long.Parse(x.PurchaseOrderDetail.PurchaseOrder.VendorCode) <= long.Parse(request.VendorTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.POType))
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? false : x.PurchaseOrderDetail.PurchaseOrder.POType.Contains(request.POType)).ToList();
            }

            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                queryNKMH = queryNKMH.Where(x => x.MaterialCodeInt >= long.Parse(request.MaterialFrom) &&
                                                 x.MaterialCodeInt <= long.Parse(request.MaterialTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.PurchasingGroupFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingGroupTo)) request.PurchasingGroupTo = request.PurchasingGroupFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? false : int.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup) >= int.Parse(request.PurchasingGroupFrom) &&
                                                                                        int.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup) <= int.Parse(request.PurchasingGroupTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? false : long.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode) >= long.Parse(request.PurchaseOrderFrom) &&
                                                                                        long.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode) <= long.Parse(request.PurchaseOrderTo)).ToList();
            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(request.WeightHeadCode))
            {
                queryNKMH = queryNKMH.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == request.WeightHeadCode.Trim().ToLower() : false).ToList();
            }


            if (request.WeightDateFrom.HasValue)
            {
                if (!request.WeightDateTo.HasValue) request.WeightDateTo = request.WeightDateFrom;

                queryNKMH = queryNKMH.Where(x => x.WeighDate >= request.WeightDateFrom &&
                                         x.WeighDate <= request.WeightDateTo).ToList();
            }

            if (request.WeightVotes.Any())
            {
                queryNKMH = queryNKMH.Where(x => request.WeightVotes.Contains(x.WeitghtVote)).ToList();
            }

            if (request.CreateBy.HasValue)
            {
                queryNKMH = queryNKMH.Where(x => x.CreateBy == request.CreateBy).ToList();
            }    

            var vendor = await _vendorRep.GetQuery().AsNoTracking().ToListAsync();

            //Catalog Nhập kho mua hàng status
            var nkmhStatus = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            //Data NKMH
            var dataNKMH = queryNKMH.OrderByDescending(x => x.CreateTime).Select(x => new ListNKMHResponse
            {
                NkmhId = x.GoodsReceiptId,
                //Plant
                Plant = x.PlantCode,
                //PO và POLine
                PurchaseOrderCode = x.PurchaseOrderDetail?.PurchaseOrder?.PurchaseOrderCode,
                POItem = x.PurchaseOrderDetail?.POLine,
                //Product
                Material = x.MaterialCodeInt.ToString(),
                MaterialName = product.FirstOrDefault(p => p.ProductCodeInt == long.Parse(x.MaterialCode))?.ProductName,
                //Ngày chứng từ
                DocumentDate = x.DocumentDate,
                //Mã kho
                SlocCode = x.PurchaseOrderDetail?.StorageLocation != null ? x.PurchaseOrderDetail?.StorageLocation : "",
                //Kho
                StorageLocation = x.PurchaseOrderDetail?.StorageLocation != null ? $"{x.PurchaseOrderDetail?.StorageLocation} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.PurchaseOrderDetail.StorageLocation).StorageLocationName}" : "",
                //Số lô
                Batch = x.Batch,
                //SL bao
                BagQuantity = x.BagQuantity,
                //Đơn trọng
                SingleWeight = x.SingleWeight,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode,
                //Trọng lượng cân
                Weight = x.Weight,
                //Confirm Qty
                ConfirmQty = x.ConfirmQty,
                //SL kèm bao bì
                QuantityWithPackaging = x.QuantityWithPackaging,
                //Số phương tiện
                VehicleCode = x.VehicleCode,
                //Số lần cân
                QuantityWeitght = x.QuantityWeitght,
                //Số phiếu cân
                WeightVote = x.WeitghtVote,
                //Total Quantity
                TotalQuantity = x.PurchaseOrderDetail?.OrderQuantity,
                //Open quantity
                OpenQuantity = x.PurchaseOrderDetail?.OpenQuantity,
                //Delivery Quantity
                DeliveredQuantity = x.PurchaseOrderDetail?.QuantityReceived,
                //Unit
                Unit = x.PurchaseOrderDetail?.Unit,
                //Số xe tải
                TruckQuantity = x.TruckQuantity,
                //Số cân đầu vào và ra
                InputWeight = x.InputWeight,
                OutputWeight = x.OutputWeight,
                //Thời gian bắt đầu cân
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                //Đánh dấu xóa
                isDelete = x.Status == "DEL" ? true : false,
                //Ghi chú 
                Description = x.Description,
                Status = nkmhStatus.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                CreateTime = x.CreateTime,
                CreateBy = user.FirstOrDefault(a => a.AccountId == x.CreateBy)?.FullName,
                LastEditTime = x.LastEditTime,
                LastEditBy = user.FirstOrDefault(a => a.AccountId == x.LastEditBy)?.FullName,
                ReverseDocument = x.ReverseDocument,
                MaterialDocument = x.MaterialDocument,
                VendorName = x.PurchaseOrderDetail == null ? null : vendor.FirstOrDefault(v => v.VendorCode == x.PurchaseOrderDetail.PurchaseOrder.VendorCode)?.VendorName,

            }).ToList();

            return dataNKMH;
        }

        public async Task<List<PuchaseOrderNKMHResponse>> GetPOAsync(GetNKMHCommand request)
        {
            #region Format Day

            //Ngày phát lệnh
            if (request.DocumentDateFrom.HasValue)
            {
                request.DocumentDateFrom = request.DocumentDateFrom.Value.Date;
            }
            if (request.DocumentDateTo.HasValue)
            {
                request.DocumentDateTo = request.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            var user = _userRep.GetQuery().AsNoTracking();

            //Product
            var product = await _prdRep.GetQuery().AsNoTracking().ToListAsync();

            //Dữ liệu đợt cân
            //var weightSs = await _weighSsRepo.GetQuery().AsNoTracking().ToListAsync();

            //Query PO
            var queryPO = await _poDetailRep.GetQuery(x => x.DeliveryCompleted != "X" &&
                                                           x.DeletionInd != "X")
                                            .Include(x => x.PurchaseOrder)
                                            .Where(x => x.PurchaseOrder.DeletionInd != "X")
                                            .AsNoTracking().ToListAsync();


            if (!string.IsNullOrEmpty(request.Plant))
            {
                queryPO = queryPO.Where(x => x.PurchaseOrder.Plant == request.Plant).ToList();
            }
            if (!string.IsNullOrEmpty(request.PurchasingOrgFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingOrgTo)) request.PurchasingOrgTo = request.PurchasingOrgFrom;
                queryPO = queryPO.Where(x => x.PurchaseOrder.PurchasingOrg == request.PurchasingOrgFrom).ToList();
            }
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                queryPO = queryPO.Where(x => !x.PurchaseOrder.VendorCode.IsNullOrEmpty() && 
                                             long.Parse(x.PurchaseOrder.VendorCode) >= long.Parse(request.VendorFrom) &&
                                             long.Parse(x.PurchaseOrder.VendorCode) <= long.Parse(request.VendorTo)).ToList();
            }   

            if (!string.IsNullOrEmpty(request.POType))
            {
                queryPO = queryPO.Where(x => x.PurchaseOrder.POType.Contains(request.POType)).ToList();
            }
            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                queryPO = queryPO.Where(x => long.Parse(x.ProductCode) >= long.Parse(request.MaterialFrom) &&
                                             long.Parse(x.ProductCode) <= long.Parse(request.MaterialTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.PurchasingGroupFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingGroupTo)) request.PurchasingGroupTo = request.PurchasingGroupFrom;
                queryPO = queryPO.Where(x => int.Parse(x.PurchaseOrder.PurchasingGroup) >= int.Parse(request.PurchasingGroupFrom) &&
                                             int.Parse(x.PurchaseOrder.PurchasingGroup) <= int.Parse(request.PurchasingGroupTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                queryPO = queryPO.Where(x => long.Parse(x.PurchaseOrder.PurchaseOrderCode) >= long.Parse(request.PurchaseOrderFrom) &&
                                             long.Parse(x.PurchaseOrder.PurchaseOrderCode) <= long.Parse(request.PurchaseOrderTo)).ToList();
            }

            //Data vendor
            var vendor = await _vendorRep.GetQuery().AsNoTracking().ToListAsync();

            //Data PO
            var dataPO = queryPO.Select(x => new PuchaseOrderNKMHResponse
            {
                PoDetailId = x.PurchaseOrderDetailId,
                StorageLocation = x.StorageLocation,
                //Plant
                Plant = x.PurchaseOrder?.Plant,
                //PO và POLine
                PurchaseOrderCode = x.PurchaseOrder?.PurchaseOrderCode,
                POItem = x.POLine,
                //Product
                Material = long.Parse(x.ProductCode).ToString(),
                MaterialName = product.FirstOrDefault(p => p.ProductCode == x.ProductCode)?.ProductName,
                //Unit
                Unit = x.Unit,
                //Vendor
                VendorCode = x.PurchaseOrder.VendorCode,
                VendorName = vendor.FirstOrDefault(v => v.VendorCode == x.PurchaseOrder.VendorCode)?.VendorName,
                //Total Quantity
                OrderQuantity = x.OrderQuantity,
                OpenQuantity = x.OpenQuantity,
                //Số lần cân
                //QuantityWeight = 
                //Batch
                Batch = x.Batch,
                //Số phương tiện
                VehicleCode = x.VehicleCode

            }).ToList();

            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                var material = await _prdRep.FindOneAsync(x => x.ProductCodeInt == long.Parse(request.MaterialFrom));

                dataPO.Add(new PuchaseOrderNKMHResponse
                {
                    Plant = request.Plant,
                    Material = material.ProductCodeInt.ToString(),
                    MaterialName = material?.ProductName,

                });
            }

            return dataPO;
        }

        public async Task<GetWeighNumResponse> GetWeighNum(string weightHeadCode)
        {
            //Lấy đầu cân
            var scale = await _scaleRepo.FindOneAsync(x => x.ScaleCode == weightHeadCode);


            //Lấy ra số cân của đầu cân có trạng thái đầu cân trong po
            var weighSs = _weighSsRepo.GetQuery(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN").FirstOrDefault();


            var result = new GetWeighNumResponse
            {
                Weight = weighSs != null ? weighSs.TotalWeight : 0,
                WeightQuantity = weighSs != null ? weighSs.TotalNumberOfWeigh : 0,
                StartTime = weighSs != null ? weighSs.StartTime : null,
                Status = weighSs != null ? weighSs.Status : ""
            };

            return result;
        }

        public async Task<GetDataByPoPoItemResponse> GetDataByPoAndPoItem(string po, string poItem)
        {
            //Lấy ra po
            var poDetails = await _poDetailRep.GetQuery().Include(x => x.PurchaseOrder)
                                              .FirstOrDefaultAsync(x => x.POLine == poItem && x.PurchaseOrder.PurchaseOrderCodeInt == long.Parse(po));

            //Danh sách product
            var prods = _prdRep.GetQuery().AsNoTracking();

            //Danh sách Vendor
            var vendors = _vendorRep.GetQuery().AsNoTracking();

            var response = new GetDataByPoPoItemResponse
            {
                //Material
                Material = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(poDetails.ProductCode)).ProductCodeInt.ToString(),
                //Material Desc
                MaterialName = prods.FirstOrDefault(p => p.ProductCodeInt == long.Parse(poDetails.ProductCode)).ProductName,
                //UoM
                UOM = !poDetails.Unit.IsNullOrEmpty() ? poDetails.Unit : "",
                //Batch
                Batch = poDetails.Batch,
                //Vendor Code
                VendorCode = poDetails.PurchaseOrder.VendorCode,
                //Vendor Name
                VendorName = poDetails.PurchaseOrder.VendorCode != null ? vendors.FirstOrDefault(v => v.VendorCode == poDetails.PurchaseOrder.VendorCode).VendorName : "",
                //Số phương tiện
                VehicleCode = poDetails.VehicleCode,
                //Total Quantity
                TotalQuantity = poDetails.OrderQuantity,
                //Open Quantity
                OpenQuantity = poDetails.OpenQuantity,
                //Delivery Quantity
                QuantityReceived = poDetails.QuantityReceived
            };

            return response;
        }
    }
}
