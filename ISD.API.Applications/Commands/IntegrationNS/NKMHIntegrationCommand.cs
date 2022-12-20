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
                                .Include(x => x.PurchaseOrder).ThenInclude(x => x.PurchaseOrderDetailModel).AsNoTracking();

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
                                PurchaseOrder = new PurchaseOrderResponse
                                {
                                    Plant = x.PurchaseOrder?.Plant,
                                    PurchaseOrderCode = x.PurchaseOrder?.PurchaseOrderCode,
                                    POType = x.PurchaseOrder?.POType,
                                    PurchasingOrg = x.PurchaseOrder?.PurchasingOrg,
                                    PurchasingGroup = x.PurchaseOrder?.PurchasingGroup,
                                    VendorCode = x.PurchaseOrder?.VendorCode,
                                    ProductCode = x.PurchaseOrder?.ProductCode,
                                    DocumentDate = x.PurchaseOrder?.DocumentDate,
                                    DetailResponses = x.PurchaseOrder?.PurchaseOrderDetailModel?.Select(e => new PurchaseOrderDetailResponse
                                    {
                                        POLine = e.POLine,
                                        StorageLocation = e.StorageLocation,
                                        Batch = e.Batch,
                                        VehicleCode = e.VehicleCode,
                                        OrderQuantity = e.OrderQuantity,
                                        OpenQuantity = e.OpenQuantity
                                    }).ToList()
                                }
                            }).ToList();


            return data;
        }
    }
}
