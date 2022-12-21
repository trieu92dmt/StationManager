using ISD.API.Applications.DTOs.IntegrationNS;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class NKMHIntegrationCommand : IRequest<List<NKMHResponse>>
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
    public class NKMHIntegrationCommandHandler : IRequestHandler<NKMHIntegrationCommand, List<NKMHResponse>>
    {
        private readonly IGeneRepo<GoodsReceiptModel> _nkmhRep;
        private readonly IGeneRepo<AccountModel> _userRep;

        public NKMHIntegrationCommandHandler(IGeneRepo<GoodsReceiptModel> nkmhRep, IGeneRepo<AccountModel> userRep)
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

            var query = _nkmhRep.GetQuery(x => x.CreateTime >= request.FromTime && x.CreateTime <= request.ToTime)
                                .Include(x => x.PurchaseOrderDetail).ThenInclude(x => x.PurchaseOrder).AsNoTracking();

            var data = query.AsEnumerable()
                            .Select(x => new NKMHResponse
                            {
                                SingleWeight = x.SingleWeight,
                                BagQuantity = x.BagQuantity,
                                WeightHeadCode = x.WeightHeadCode,
                                Weight = x.Weight,
                                ConfirmQty = x.ConfirmQty,
                                QuantityWithPackaging = x.QuantityWithPackaging,
                                QuantityWeitght = x.QuantityWeitght,
                                TotalQuantity = x.TotalQuantity,
                                DeliveredQuantity = x.DeliveredQuantity,
                                TruckQuantity = x.TruckQuantity,
                                InputWeight = x.InputWeight,
                                OutputWeight = x.OutputWeight,
                                Description = x.Description,
                                Image = x.Image,
                                Status = x.Status,
                                QuantityWeitghtVote = x.QuantityWeitghtVote,
                                StartTime = x.StartTime,
                                EndTime = x.EndTime,
                                CreateBy = users.FirstOrDefault(a => a.AccountId == x.CreateBy)?.FullName,
                                CreateTime = x.CreateTime,
                                LastEditBy = users.FirstOrDefault(a => a.AccountId == x.LastEditBy)?.FullName,
                                LastEditTime = x.LastEditTime,
                                Plant = x.PurchaseOrderDetail?.PurchaseOrder?.Plant,
                                PurchaseOrderCode = x.PurchaseOrderDetail?.PurchaseOrder?.PurchaseOrderCode,
                                POType = x.PurchaseOrderDetail?.PurchaseOrder?.POType,
                                PurchasingOrg = x.PurchaseOrderDetail?.PurchaseOrder?.PurchasingOrg,
                                PurchasingGroup = x.PurchaseOrderDetail?.PurchaseOrder?.PurchasingGroup,
                                VendorCode = x.PurchaseOrderDetail?.PurchaseOrder?.VendorCode,
                                ProductCode = x.PurchaseOrderDetail?.PurchaseOrder?.ProductCode,
                                DocumentDate = x.PurchaseOrderDetail?.PurchaseOrder?.DocumentDate,
                                POLine = x.PurchaseOrderDetail?.POLine,
                                StorageLocation = x.PurchaseOrderDetail?.StorageLocation,
                                Batch = x.PurchaseOrderDetail?.Batch,
                                VehicleCode = x.PurchaseOrderDetail?.VehicleCode,
                                OrderQuantity = x.PurchaseOrderDetail?.OrderQuantity,
                                OpenQuantity = x.PurchaseOrderDetail?.OpenQuantity
                            }).ToList();
            return data;
        }
    }
}
