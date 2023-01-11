using IntegrationNS.Application.DTOs;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.NKMHs
{
    public class NKMHIntegrationCommand : IRequest<List<NKMHResponse>>
    {
        public string Plant { get; set; }
        public int? PurchasingOrgFrom { get; set; }
        public int? PurchasingOrgTo { get; set; }

        public int? PurchasingGroupFrom { get; set; }
        public int? PurchasingGroupTo { get; set; }

        public int? VendorFrom { get; set; }
        public int? VendorTo { get; set; }
        public string POType { get; set; }
        public int? PurchaseOrderFrom { get; set; }
        public int? PurchaseOrderTo { get; set; }
        public int? MaterialFrom { get; set; }
        public int? MaterialTo { get; set; }
        public List<int?> Materials { get; set; } = new List<int?>();

        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
    public class NKMHIntegrationCommandHandler : IRequestHandler<NKMHIntegrationCommand, List<NKMHResponse>>
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<AccountModel> _userRep;

        public NKMHIntegrationCommandHandler(IRepository<GoodsReceiptModel> nkmhRep, IRepository<AccountModel> userRep)
        {
            _nkmhRep = nkmhRep;
            _userRep = userRep;
        }
        public async Task<List<NKMHResponse>> Handle(NKMHIntegrationCommand request, CancellationToken cancellationToken)
        {

            #region Format day and Search by day

            if (request.ToTime.HasValue)
            {
                request.ToTime = request.ToTime.Value.Date.AddDays(1).AddSeconds(-1);
            }
            else
            {
                request.ToTime = DateTime.Now;

            }

            if (request.FromTime.HasValue)
            {
                request.FromTime = request.FromTime.Value.Date;
            }
            else
            {
                request.FromTime = DateTime.Now;

            }
            #endregion

            var users = _userRep.GetQuery().AsNoTracking();

            var query = await _nkmhRep.GetQuery(x => x.DocumentDate >= request.FromTime && x.DocumentDate <= request.ToTime)
                                      .Include(x => x.PurchaseOrderDetail)
                                      .ThenInclude(x => x.PurchaseOrder)
                                      .AsNoTracking()
                                      .ToListAsync();

            if (!string.IsNullOrEmpty(request.Plant))
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.Plant.Contains(request.Plant)).ToList();
            }

            if (request.PurchasingOrgFrom.HasValue)
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCodeInt >= request.PurchaseOrderFrom &&
                                         x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCodeInt <= request.PurchaseOrderTo).ToList();
            }

            if (request.VendorFrom.HasValue)
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.VendorCodeInt >= request.VendorFrom &&
                                         x.PurchaseOrderDetail.PurchaseOrder.VendorCodeInt <= request.VendorTo).ToList();
            }

            if (!string.IsNullOrEmpty(request.POType))
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.POType.Contains(request.POType)).ToList();
            }

            if (request.MaterialFrom.HasValue)
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.ProductCodeInt >= request.MaterialFrom &&
                                         x.PurchaseOrderDetail.PurchaseOrder.ProductCodeInt <= request.MaterialTo).ToList();
            }

            if (request.PurchasingGroupFrom.HasValue)
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroupInt >= request.PurchasingGroupFrom &&
                                         x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroupInt <= request.PurchasingGroupTo).ToList();
            }

            if (request.Materials.Any())
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         request.Materials.Contains(x.PurchaseOrderDetail.PurchaseOrder.ProductCodeInt)).ToList();
            }

            var data = query.AsEnumerable()
                            .Select(x => new NKMHResponse
                            {
                                NkmhId = x.GoodsReceiptId,
                                //ID đợt cân
                                WeightId = x.WeightId,
                                WeightVote = x.WeitghtVote,
                                //Đơn trọng
                                SingleWeight = x.SingleWeight,
                                //Số lượng bao
                                BagQuantity = x.BagQuantity,
                                //Mã đầu cân
                                WeightHeadCode = x.WeightHeadCode,
                                //Trọng lượng cân
                                Weight = x.Weight,
                                //Confirm Quantity
                                ConfirmQty = x.ConfirmQty,
                                //Số lượng kèm bao bì
                                QuantityWithPackaging = x.QuantityWithPackaging,
                                //Số lần cân
                                QuantityWeitght = x.QuantityWeitght,
                                //Total Quantity
                                TotalQuantity = x.PurchaseOrderDetail.OrderQuantity,
                                //Delivered Quantity
                                DeliveredQuantity = x.PurchaseOrderDetail.QuantityReceived,
                                //Số xe tải
                                TruckQuantity = x.TruckQuantity,
                                //Số cân đầu vào
                                InputWeight = x.InputWeight,
                                //Số cân đầu ra
                                OutputWeight = x.OutputWeight,
                                //Ghi chú
                                Description = x.Description,
                                //Hình ảnh
                                Image = x.Image,
                                //Trạng thái
                                Status = x.Status,
                                //Thời gian bắt đầu và kết thúc
                                StartTime = x.StartTime,
                                EndTime = x.EndTime,
                                //Common
                                CreateBy = users.FirstOrDefault(a => a.AccountId == x.CreateBy)?.UserName,
                                CreateTime = x.CreateTime,
                                LastEditBy = users.FirstOrDefault(a => a.AccountId == x.LastEditBy)?.UserName,
                                LastEditTime = x.LastEditTime,
                                //Palne 
                                Plant = x.PurchaseOrderDetail?.PurchaseOrder?.Plant,
                                //PO
                                PurchaseOrderCode = x.PurchaseOrderDetail?.PurchaseOrder?.PurchaseOrderCode,
                                POItem = x.PurchaseOrderDetail?.POLine,
                                POType = x.PurchaseOrderDetail?.PurchaseOrder?.POType,
                                //PurchasingOrg
                                PurchasingOrg = x.PurchaseOrderDetail?.PurchaseOrder?.PurchasingOrg,
                                PurchasingGroup = x.PurchaseOrderDetail?.PurchaseOrder?.PurchasingGroup,
                                //NCC
                                VendorCode = x.PurchaseOrderDetail?.PurchaseOrder?.VendorCode,
                                //Material
                                Material = x.PurchaseOrderDetail?.PurchaseOrder?.ProductCode,
                                //DocumentDate
                                DocumentDate = x.PurchaseOrderDetail?.PurchaseOrder?.DocumentDate,
                                //Storage Location
                                StorageLocation = x.PurchaseOrderDetail?.StorageLocation,
                                //Số lô
                                Batch = x.PurchaseOrderDetail?.Batch,
                                //Số phương tiện
                                VehicleCode = x.VehicleCode,

                                //Số lượng đặt hàng
                                OrderQuantity = x.PurchaseOrderDetail?.OrderQuantity,
                                OpenQuantity = x.PurchaseOrderDetail?.OpenQuantity
                            }).ToList();
            return data;
        }
    }
}
