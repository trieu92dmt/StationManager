using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.MES;
using MES.Application.DTOs.MES;
using MES.Application.DTOs.MES.NKMH;
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
        //Task<List<GetWeighNumResponse>> GetWeighNum(List<string> weightHead);
    }
    public class NKMHQuery : INKMHQuery
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<ProductModel> _prdRep;
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;
        private readonly IRepository<AccountModel> _userRep;
        private readonly IRepository<VendorModel> _vendorRep;
        private readonly IRepository<WeighingSessionModel> _weighSsRepo;

        public NKMHQuery(IRepository<GoodsReceiptModel> nkmhRep, IRepository<ProductModel> prdRep, IRepository<PurchaseOrderMasterModel> poRep,
                         IRepository<PurchaseOrderDetailModel> poDetailRep, IRepository<AccountModel> userRep, IRepository<VendorModel> vendorRep,
                         IRepository<WeighingSessionModel> weighSsRepo)
        {
            _nkmhRep = nkmhRep;
            _prdRep = prdRep;
            _poRep = poRep;
            _poDetailRep = poDetailRep;
            _userRep = userRep;
            _vendorRep = vendorRep;
            _weighSsRepo = weighSsRepo;
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

            //Product
            var product = await _prdRep.GetQuery().AsNoTracking().ToListAsync();

            var queryNKMH = await _nkmhRep.GetQuery()
                                    .Include(x => x.PurchaseOrderDetail)
                                    .ThenInclude(x => x.PurchaseOrder)
                                    .AsNoTracking().ToListAsync();

            if (!string.IsNullOrEmpty(request.Plant))
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.Plant.Contains(request.Plant)).ToList();
            }
            if (!string.IsNullOrEmpty(request.PurchasingOrgFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingOrgTo)) request.PurchasingOrgTo = request.PurchasingOrgFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail.PurchaseOrder.PurchasingOrg == request.PurchasingOrgFrom).ToList();
            }
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : int.Parse(x.PurchaseOrderDetail.PurchaseOrder.VendorCode) >= int.Parse(request.VendorFrom) &&
                                                                                        int.Parse(x.PurchaseOrderDetail.PurchaseOrder.VendorCode) <= int.Parse(request.VendorTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.POType))
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.POType.Contains(request.POType)).ToList();
            }

            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                queryNKMH = queryNKMH.Where(x => !x.PurchaseOrderDetailId.HasValue ? true : long.Parse(x?.PurchaseOrderDetail?.ProductCode) >= long.Parse(request.MaterialFrom) &&
                                                                                            long.Parse(x?.PurchaseOrderDetail?.ProductCode) <= long.Parse(request.MaterialTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.PurchasingGroupFrom))
            {
                if (string.IsNullOrEmpty(request.PurchasingGroupTo)) request.PurchasingGroupTo = request.PurchasingGroupFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : int.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup) >= int.Parse(request.PurchasingGroupFrom) &&
                                                                                        int.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup) <= int.Parse(request.PurchasingGroupTo)).ToList();
            }

            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : int.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode) >= int.Parse(request.PurchaseOrderFrom) &&
                                                                                        int.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode) <= int.Parse(request.PurchaseOrderTo)).ToList();
            }

            var vendor = await _vendorRep.GetQuery().AsNoTracking().ToListAsync();


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
                Material = x.PurchaseOrderDetail?.ProductCode,
                MaterialName = x.PurchaseOrderDetail == null ? null :
                               product.FirstOrDefault(p => p.ProductCode == x.PurchaseOrderDetail.ProductCode)?.ProductName,
                //Ngày chứng từ
                DocumentDate = x.DocumentDate,
                //Kho
                StorageLocation = x.PurchaseOrderDetail?.StorageLocation,
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
                //Ghi chú 
                Description = x.Description,
                Status = x.Status,
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

            //Query PO
            var queryPO = await _poDetailRep.GetQuery()
                                            .Include(x => x.PurchaseOrder)
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
                Material = x.ProductCode,
                MaterialName = product.FirstOrDefault(p => p.ProductCode == x.ProductCode)?.ProductName,
                //Unit
                Unit = x.Unit,
                //Vendor
                VendorCode = x.PurchaseOrder.VendorCode,
                VendorName = vendor.FirstOrDefault(v => v.VendorCode == x.PurchaseOrder.VendorCode)?.VendorName,
                OrderQuantity = x.OrderQuantity,
                OpenQuantity = x.OpenQuantity

            }).ToList();

            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                var material = await _prdRep.FindOneAsync(x => x.ProductCode == request.MaterialFrom);

                dataPO.Add(new PuchaseOrderNKMHResponse
                {
                    Plant = request.Plant,
                    Material = request.MaterialFrom.ToString(),
                    MaterialName = material?.ProductName
                });
            }

            return dataPO;
        }
    }
}
