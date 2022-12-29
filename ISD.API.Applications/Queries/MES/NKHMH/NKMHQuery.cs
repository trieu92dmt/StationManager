using ISD.API.Applications.Commands.MES;
using ISD.API.Applications.DTOs.MES;
using ISD.API.Core.SeedWork;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.Applications.Queries.MES.NKHMH
{
    public interface INKMHQuery
    {
        Task<PagingResultSP<NKMHMesResponse>> GetNKMHAsync(GetNKMHCommand request);
    }
    public class NKMHQuery : INKMHQuery
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<ProductModel> _prdRep;

        public NKMHQuery(IRepository<GoodsReceiptModel> nkmhRep, IRepository<ProductModel> prdRep)
        {
            _nkmhRep = nkmhRep;
            _prdRep = prdRep;
        }

        public async Task<PagingResultSP<NKMHMesResponse>> GetNKMHAsync(GetNKMHCommand request)
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

            //Product
            var product = _prdRep.GetQuery().AsNoTracking();

            var query = _nkmhRep.GetQuery()
                                .Include(x => x.PurchaseOrderDetail).ThenInclude(x => x.PurchaseOrder)
                                .AsNoTracking();

            var data = query.Select(x => new NKMHMesResponse
            {
                //Plant
                Plant = x.PurchaseOrderDetail.PurchaseOrder.Plant,
                //PO và POLine
                PurchaseOrderCode = x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode,
                POItem = x.PurchaseOrderDetail.POLine,
                //Product
                Material = x.PurchaseOrderDetail.ProductCode,
                MaterialName = product.FirstOrDefault(p => p.ProductCode == x.PurchaseOrderDetail.ProductCode) == null ? null :
                               product.FirstOrDefault(p => p.ProductCode == x.PurchaseOrderDetail.ProductCode).ProductName,
                //Ngày chứng từ
                DocumentDate = x.DocumentDate,
                //Kho
                StorageLocation = x.PurchaseOrderDetail.StorageLocation,
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
                Unit = x.PurchaseOrderDetail.Unit,
                //Số xe tải
                TruckQuantity = x.TruckQuantity,
                //Số cân đầu vào và ra
                InputWeight = x.InputWeight,
                OutputWeight = x.OutputWeight,
                //Ghi chú 
                Description = x.Description,
                Status = x.Status
            });

            var totalRecords = await data.CountAsync();

            var responsePaginated = await PaginatedList<NKMHMesResponse>.CreateAsync(data, request.Paging.Offset, request.Paging.PageSize);
            var response = new PagingResultSP<NKMHMesResponse>(responsePaginated, totalRecords, request.Paging.PageIndex, request.Paging.PageSize);

            if (response.Data.Any())
            {
                int i = request.Paging.Offset;

                foreach (var item in response.Data)
                {
                    i++;
                    item.STT = i;
                }
            }

            return response;
        }
    }
}
