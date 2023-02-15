using IntegrationNS.Application.DTOs;
using Core.Exceptions;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Queries
{
    public interface INKMHQuery
    {
        /// <summary>
        /// GET phiếu nhập kho mua hàng
        /// </summary>
        /// <param name="nkmhId"></param>
        /// <returns></returns>
        Task<NKMHResponse> GetNKMHAsync(Guid nkmhId);

        /// <summary>
        /// GET Purchase Order
        /// </summary>
        /// <param name="purchaseOrder"></param>
        /// <returns></returns>
        Task<PuchaseOrderNKMHResponse> GetPOAsync(string purchaseOrder);

    }

    public class NKMHQuery : INKMHQuery
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<AccountModel> _userRep;
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;

        public NKMHQuery(IRepository<GoodsReceiptModel> nkmhRep, IRepository<AccountModel> userRep, IRepository<PurchaseOrderMasterModel> poRep,
                         IRepository<PurchaseOrderDetailModel> poDetailRep)
        {
            _nkmhRep = nkmhRep;
            _userRep = userRep;
            _poRep = poRep;
            _poDetailRep = poDetailRep;
        }

        #region GET phiếu nhập kho mua hàng
        /// <summary>
        /// GET phiếu nhập kho mua hàng
        /// </summary>
        /// <param name="nkmhId"></param>
        /// <returns></returns>
        public async Task<NKMHResponse> GetNKMHAsync(Guid nkmhId)
        {
            var users = _userRep.GetQuery().AsNoTracking();

            var nkmh = await _nkmhRep.GetQuery(x => x.GoodsReceiptId == nkmhId)
                                      .Include(x => x.PurchaseOrderDetail)
                                      .ThenInclude(x => x.PurchaseOrder)
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync();

            if (nkmh is null)
                throw new ISDException(CommonResource.Msg_NotFound, "Phiếu NKMH");


            var response = new NKMHResponse
            {
                NkmhId = nkmh.GoodsReceiptId,
                //ID đợt cân
                WeightId = nkmh.WeightId,
                WeightVote = nkmh.WeitghtVote,
                //Đơn trọng
                SingleWeight = nkmh.SingleWeight,
                //Số lượng bao
                BagQuantity = nkmh.BagQuantity,
                //Mã đầu cân
                WeightHeadCode = nkmh.WeightHeadCode,
                //Trọng lượng cân
                Weight = nkmh.Weight,
                //Confirm Quantity
                ConfirmQty = nkmh.ConfirmQty,
                //Số lượng kèm bao bì
                QuantityWithPackaging = nkmh.QuantityWithPackaging,
                //Số lần cân
                QuantityWeitght = nkmh.QuantityWeitght,
                //Total Quantity
                TotalQuantity = nkmh.PurchaseOrderDetail?.OrderQuantity,
                //Delivered Quantity
                DeliveredQuantity = nkmh.PurchaseOrderDetail?.QuantityReceived,
                //Open Quantity
                OpenQuantity = nkmh.PurchaseOrderDetail?.OpenQuantity,
                //Số xe tải
                TruckQuantity = nkmh.TruckQuantity,
                //Số cân đầu vào
                InputWeight = nkmh.InputWeight,
                //Số cân đầu ra
                OutputWeight = nkmh.OutputWeight,
                //Ghi chú
                Description = nkmh.Description,
                //Hình ảnh
                Image = nkmh.Img,
                //Trạng thái
                Status = nkmh.Status,
                //Thời gian bắt đầu và kết thúc
                StartTime = nkmh.StartTime,
                EndTime = nkmh.EndTime,
                //Common
                CreateBy = users.FirstOrDefault(a => a.AccountId == nkmh.CreateBy)?.UserName,
                CreateTime = nkmh.CreateTime,
                LastEditBy = users.FirstOrDefault(a => a.AccountId == nkmh.LastEditBy)?.UserName,
                LastEditTime = nkmh.LastEditTime,
                //Palne 
                Plant = nkmh.PurchaseOrderDetail?.PurchaseOrder?.Plant,
                //PO
                PurchaseOrderCode = nkmh.PurchaseOrderDetail?.PurchaseOrder?.PurchaseOrderCode,
                POItem = nkmh.PurchaseOrderDetail?.POLine,
                POType = nkmh.PurchaseOrderDetail?.PurchaseOrder?.POType,
                //PurchasingOrg
                PurchasingOrg = nkmh.PurchaseOrderDetail?.PurchaseOrder?.PurchasingOrg,
                PurchasingGroup = nkmh.PurchaseOrderDetail?.PurchaseOrder?.PurchasingGroup,
                //NCC
                VendorCode = nkmh.PurchaseOrderDetail?.PurchaseOrder?.VendorCode,
                //Material
                Material = nkmh.PurchaseOrderDetail?.PurchaseOrder?.ProductCode,
                //DocumentDate
                DocumentDate = nkmh.PurchaseOrderDetail?.PurchaseOrder?.DocumentDate,
                //Storage Location
                StorageLocation = nkmh.PurchaseOrderDetail?.StorageLocation,
                //Số lô
                Batch = nkmh.PurchaseOrderDetail?.Batch,
                //Số phương tiện
                VehicleCode = nkmh.VehicleCode,

            };


            return response;
        }
        #endregion

        #region GET Purchase Order
        /// <summary>
        /// GET Purchase Order
        /// </summary>
        /// <param name="purchaseOrder"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PuchaseOrderNKMHResponse> GetPOAsync(string purchaseOrder)
        {
            //Purchase Order
            var po = await _poRep.GetQuery(x => x.PurchaseOrderCode == purchaseOrder)
                                 .Include(x => x.PurchaseOrderDetailModel)
                                 .FirstOrDefaultAsync();

            if (po == null)
                throw new ISDException(CommonResource.Msg_NotFound, $"PO {purchaseOrder}");

            var response = new PuchaseOrderNKMHResponse
            {
                //PO
                PurchaseOrderCode = po.PurchaseOrderCode,
                //PO Type
                POType = po.POType,
                //Plant
                Plant = po.Plant,
                //Purchasing Org
                PurchasingOrg = po.PurchasingOrg,
                //Purchasing Group
                PurchasingGroup = po.PurchasingGroup,
                //Material 
                Material = po.ProductCode,
                //Vendor
                Vendor = po.VendorCode,
                //Document Date
                DocumentDate = po.DocumentDate,
                //Common
                CreateOn = po.CreateTime,
                ChangeOn = po.LastEditTime,
                PODetails = po.PurchaseOrderDetailModel.Select(x => new DetailPuchaseOrderNKMHResponse
                {
                    //PO Line
                    POLine = x.POLine,
                    //Storage Location
                    StorageLocation = x.StorageLocation,
                    //Số lô
                    Batch = x.Batch,
                    //Open Quantity
                    OpenQuantity = x.OpenQuantity,
                    //Order Quantity
                    OrderQuantity = x.OpenQuantity
                }).ToList()
            };

            return response;
        }
        #endregion
    }
}
