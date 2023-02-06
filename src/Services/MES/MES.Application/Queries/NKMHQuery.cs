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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        private readonly IRepository<TruckInfoModel> _truckInfoRepo;

        public NKMHQuery(IRepository<GoodsReceiptModel> nkmhRep, IRepository<ProductModel> prdRep, IRepository<PurchaseOrderMasterModel> poRep,
                         IRepository<PurchaseOrderDetailModel> poDetailRep, IRepository<AccountModel> userRep, IRepository<VendorModel> vendorRep,
                         IRepository<WeighSessionModel> weighSsRepo, IRepository<ScaleModel> scaleRepo, IRepository<CatalogModel> cataRepo,
                         IRepository<StorageLocationModel> slocRepo, IRepository<TruckInfoModel> truckInfoRepo)
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
            _truckInfoRepo = truckInfoRepo;
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

            //Ngày phát lệnh
            if (request.WeightDateFrom.HasValue)
            {
                request.WeightDateFrom = request.WeightDateFrom.Value.Date;
            }
            if (request.WeightDateTo.HasValue)
            {
                request.WeightDateTo = request.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            var user = _userRep.GetQuery().AsNoTracking();

            //Danh sách sloc
            var slocs = _slocRepo.GetQuery(x => x.PlantCode == request.Plant).AsNoTracking();

            //Danh sách dữ liệu cân xe tải
            var truckInfo = _truckInfoRepo.GetQuery().AsNoTracking();

            //Product
            var product =  _prdRep.GetQuery().AsNoTracking();

            var queryNKMH = _nkmhRep.GetQuery()
                                    .Include(x => x.PurchaseOrderDetail)
                                    .ThenInclude(x => x.PurchaseOrder)
                                    .AsNoTracking();

            if (!string.IsNullOrEmpty(request.Plant))
            {
                queryNKMH = queryNKMH.Where(x => x.PlantCode == request.Plant);
            }
            if (!string.IsNullOrEmpty(request.PurchasingOrgFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingOrgTo)) request.PurchasingOrgTo = request.PurchasingOrgFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail.PurchaseOrder.PurchasingOrg == request.PurchasingOrgFrom);
            }
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
                                                                                    x.PurchaseOrderDetail.PurchaseOrder.VendorCode.CompareTo(request.VendorTo) <= 0 : false);
            }

            if (!string.IsNullOrEmpty(request.POType))
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.POType.Contains(request.POType) : false);
            }

            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                queryNKMH = queryNKMH.Where(x => x.MaterialCodeInt >= long.Parse(request.MaterialFrom) &&
                                                 x.MaterialCodeInt <= long.Parse(request.MaterialTo));
            }

            if (!string.IsNullOrEmpty(request.PurchasingGroupFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingGroupTo)) request.PurchasingGroupTo = request.PurchasingGroupFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupFrom) >=0 &&
                                                                                    x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupTo) <= 0 : false);
            }

            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderFrom) >= 0 &&
                                                                                    x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderFrom) <= 0 : false);
            }

            //Lọc document date
            if (request.DocumentDateFrom.HasValue)
            {
                if (!request.DocumentDateTo.HasValue)
                {
                    request.DocumentDateTo = request.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                queryNKMH = queryNKMH.Where(x => x.DocumentDate >= request.DocumentDateFrom &&
                                                 x.DocumentDate <= request.DocumentDateTo);
            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(request.WeightHeadCode))
            {
                queryNKMH = queryNKMH.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == request.WeightHeadCode.Trim().ToLower() : false);
            }


            if (request.WeightDateFrom.HasValue)
            {
                if (!request.WeightDateTo.HasValue) request.WeightDateTo = request.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);

                queryNKMH = queryNKMH.Where(x => x.WeighDate >= request.WeightDateFrom &&
                                         x.WeighDate <= request.WeightDateTo);
            }

            if (!request.WeightVotes.IsNullOrEmpty())
            {
                queryNKMH = queryNKMH.Where(x => request.WeightVotes.Contains(x.WeitghtVote));
            }

            if (request.CreateBy.HasValue)
            {
                queryNKMH = queryNKMH.Where(x => x.CreateBy == request.CreateBy);
            }    

            var vendor = _vendorRep.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var nkmhStatus = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            //Data NKMH
            var dataNKMH = await queryNKMH.OrderByDescending(x => x.CreateTime).Select(x => new ListNKMHResponse
            {
                //Id
                NkmhId = x.GoodsReceiptId,
                //Plant
                Plant = x.PlantCode,
                //PO và POLine
                PurchaseOrderCode = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode : "",
                POItem = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.POLine : "",
                //Product
                Material = x.MaterialCodeInt.ToString(),
                MaterialName = product.FirstOrDefault(p => p.ProductCodeInt == x.MaterialCodeInt).ProductName,
                //Ngày chứng từ
                DocumentDate = x.DocumentDate,
                //Mã kho
                SlocCode = x.SlocCode,
                //Kho
                StorageLocation = string.IsNullOrEmpty(x.SlocCode) ? "" : $"{x.SlocCode} | {x.SlocName}",
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
                TotalQuantity = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.OrderQuantity : 0,
                //Open quantity
                OpenQuantity = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.OpenQuantity : 0,
                //Delivery Quantity
                DeliveredQuantity = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.QuantityReceived : 0,
                //Unit
                Unit = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.Unit : "",
                //Id sô xe tải
                TruckInfoId = x.TruckInfoId,    
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
                //Hình ảnh
                Image = x.Img,
                Status = nkmhStatus.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                CreateTime = x.CreateTime,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                CreateById = x.CreateBy,
                LastEditTime = x.LastEditTime,
                LastEditBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                LastEditById = x.LastEditBy,
                ReverseDocument = x.ReverseDocument,
                MaterialDocument = x.MaterialDocument,
                VendorName = x.PurchaseOrderDetailId.HasValue ? vendor.FirstOrDefault(v => v.VendorCode == x.PurchaseOrderDetail.PurchaseOrder.VendorCode).VendorName : "",

            }).ToListAsync();

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
            var product = _prdRep.GetQuery().AsNoTracking();

            //Dữ liệu đợt cân
            //var weightSs = await _weighSsRepo.GetQuery().AsNoTracking().ToListAsync();

            //Query PO
            var queryPO = _poDetailRep.GetQuery()
                                            .Include(x => x.PurchaseOrder)
                                            .Where(x => x.DeliveryCompleted != "X" &&
                                                        x.DeletionInd != "X" &&
                                                        x.PurchaseOrder.DeletionInd != "X")
                                            .AsNoTracking();


            if (!string.IsNullOrEmpty(request.Plant))
            {
                queryPO = queryPO.Where(x => x.PurchaseOrder.Plant == request.Plant);
            }
            if (!string.IsNullOrEmpty(request.PurchasingOrgFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingOrgTo)) request.PurchasingOrgTo = request.PurchasingOrgFrom;
                queryPO = queryPO.Where(x => x.PurchaseOrder.PurchasingOrg == request.PurchasingOrgFrom);
            }
            //!x.PurchaseOrder.VendorCode.IsNullOrEmpty() &&
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                queryPO = queryPO.Where(x => 
                                             x.PurchaseOrder.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
                                             x.PurchaseOrder.VendorCode.CompareTo(request.VendorTo) <= 0);
            }

            if (!string.IsNullOrEmpty(request.POType))
            {
                queryPO = queryPO.Where(x => x.PurchaseOrder.POType.Contains(request.POType));
            }
            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                queryPO = queryPO.Where(x => x.ProductCode.CompareTo(request.MaterialFrom) >= 0 &&
                                             x.ProductCode.CompareTo(request.MaterialTo) <= 0);
            }

            if (!string.IsNullOrEmpty(request.PurchasingGroupFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingGroupTo)) request.PurchasingGroupTo = request.PurchasingGroupFrom;
                queryPO = queryPO.Where(x => x.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupFrom) >= 0 &&
                                             x.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupTo) <=0);
            }

            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                queryPO = queryPO.Where(x => x.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderFrom) >=0 &&
                                             x.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderTo) <=0);
            }

            //Data vendor
            var vendor = _vendorRep.GetQuery().AsNoTracking();

            //Data PO
            var dataPO = await queryPO.Select(x => new PuchaseOrderNKMHResponse
            {
                PoDetailId = x.PurchaseOrderDetailId,
                StorageLocation = x.StorageLocation,
                //Plant
                Plant = x.PurchaseOrder.Plant,
                //PO và POLine
                PurchaseOrderCode = x.PurchaseOrder.PurchaseOrderCode,
                POItem = x.POLine,
                //Product
                Material = long.Parse(x.ProductCode).ToString(),
                MaterialName = !string.IsNullOrEmpty(x.ProductCode) ? product.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName : "",
                //Unit
                Unit = x.Unit,
                //Vendor
                VendorCode = x.PurchaseOrder.VendorCode,
                VendorName = !string.IsNullOrEmpty(x.PurchaseOrder.VendorCode) ? vendor.FirstOrDefault(v => v.VendorCode == x.PurchaseOrder.VendorCode).VendorName : "",
                //Total Quantity
                OrderQuantity = x.OrderQuantity,
                OpenQuantity = x.OpenQuantity,
                DeliveredQuantity = x.QuantityReceived,
                //Số lần cân
                //QuantityWeight = 
                //Batch
                Batch = x.Batch,
                //Số phương tiện
                VehicleCode = x.VehicleCode

            }).OrderBy(x => x.PurchaseOrderCode).ThenBy(x => x.POItem).ToListAsync();

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
