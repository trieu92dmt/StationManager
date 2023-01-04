using ISD.API.Applications.Commands.MES;
using ISD.API.Applications.DTOs.MES;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.Applications.Queries.MES.NKHMH
{
    public interface INKMHQuery
    {
        Task<NKMHMesResponse> GetNKMHAsync(GetNKMHCommand request);
    }
    public class NKMHQuery : INKMHQuery
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<ProductModel> _prdRep;
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;
        private readonly IRepository<AccountModel> _userRep;
        private readonly IRepository<VendorModel> _vendorRep;

        public NKMHQuery(IRepository<GoodsReceiptModel> nkmhRep, IRepository<ProductModel> prdRep, IRepository<PurchaseOrderMasterModel> poRep, 
                         IRepository<PurchaseOrderDetailModel> poDetailRep, IRepository<AccountModel> userRep, IRepository<VendorModel> vendorRep)
        {
            _nkmhRep = nkmhRep;
            _prdRep = prdRep;
            _poRep = poRep;
            _poDetailRep = poDetailRep;
            _userRep = userRep;
            _vendorRep = vendorRep;
        }

        public async Task<NKMHMesResponse> GetNKMHAsync(GetNKMHCommand request)
        {
            var response = new NKMHMesResponse();

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
            var product = await  _prdRep.GetQuery().AsNoTracking().ToListAsync();

            var queryNKMH = await _nkmhRep.GetQuery()
                                    .Include(x => x.PurchaseOrderDetail)
                                    .ThenInclude(x => x.PurchaseOrder)
                                    .AsNoTracking().ToListAsync();
            if (!string.IsNullOrEmpty(request.Plant))
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.Plant.Contains(request.Plant)).ToList();
            }
            if (request.PurchasingOrgFrom.HasValue)
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCodeInt >= request.PurchaseOrderFrom &&
                                                 x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCodeInt <= request.PurchaseOrderTo).ToList();
            }
            if (request.VendorFrom.HasValue)
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.VendorCodeInt >= request.VendorFrom &&
                                                 x.PurchaseOrderDetail.PurchaseOrder.VendorCodeInt <= request.VendorTo).ToList();
            }

            if (!string.IsNullOrEmpty(request.POType))
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.POType.Contains(request.POType)).ToList();
            }
            if (request.MaterialFrom.HasValue)
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.ProductCodeInt >= request.MaterialFrom &&
                                                 x.PurchaseOrderDetail.PurchaseOrder.ProductCodeInt <= request.MaterialTo).ToList();
            }

            if (request.PurchasingGroupFrom.HasValue)
            {
                queryNKMH = queryNKMH.Where(x => x.PurchaseOrderDetail == null ? true : x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroupInt >= request.PurchasingGroupFrom &&
                                                 x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroupInt <= request.PurchasingGroupTo).ToList();
            }

            var vendor = await _vendorRep.GetQuery().AsNoTracking().ToListAsync();


            //Data NKMH
            var dataNKMH = queryNKMH.OrderByDescending(x => x.CreateTime).Select(x => new ListNKMHResponse
            {
                NkmhId = x.GoodsReceiptId,
                //Plant
                Plant = x.PurchaseOrderDetail?.PurchaseOrder?.Plant,
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
                TotalQuantity = x.TotalQuantity,
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

            });;

            response.ListNKMHs = dataNKMH.ToList();

            //Query PO
            var queryPO = await _poDetailRep.GetQuery()
                                            .Include(x => x.PurchaseOrder)
                                            .AsNoTracking().ToListAsync();

            if (!string.IsNullOrEmpty(request.Plant))
                queryPO = queryPO.Where(x => x.PurchaseOrder.Plant.Contains(request.Plant)).ToList();

            if (request.PurchasingOrgFrom.HasValue)
                queryPO = queryPO.Where(x => x.PurchaseOrder.PurchaseOrderCodeInt >= request.PurchaseOrderFrom &&
                             x.PurchaseOrder.PurchaseOrderCodeInt <= request.PurchaseOrderTo).ToList();

            if (request.VendorFrom.HasValue)
                queryPO = queryPO.Where(x => x.PurchaseOrder.VendorCodeInt >= request.VendorFrom &&
                                                 x.PurchaseOrder.VendorCodeInt <= request.VendorTo).ToList();

            if (!string.IsNullOrEmpty(request.POType))
                queryPO = queryPO.Where(x => x.PurchaseOrder.POType.Contains(request.POType)).ToList();

            if (request.MaterialFrom.HasValue)
            {
                queryPO = queryPO.Where(x => x.PurchaseOrder.ProductCodeInt >= request.MaterialFrom &&
                                             x.PurchaseOrder.ProductCodeInt <= request.MaterialTo).ToList();
            }

            if (request.PurchasingGroupFrom.HasValue)
                queryPO = queryPO.Where(x => x.PurchaseOrder.PurchasingGroupInt >= request.PurchasingGroupFrom &&
                                             x.PurchaseOrder.PurchasingGroupInt <= request.PurchasingGroupTo).ToList();


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

            if (request.MaterialFrom.HasValue)
            {
                var material = await _prdRep.FindOneAsync(x => x.ProductCodeInt == request.MaterialFrom);

                dataPO.Add(new PuchaseOrderNKMHResponse
                {
                    Material = request.MaterialFrom.ToString(),
                    MaterialName = material?.ProductName
                });
            }

            response.PuchaseOrderNKMHs = dataPO;

            return response;
        }
    }
}
