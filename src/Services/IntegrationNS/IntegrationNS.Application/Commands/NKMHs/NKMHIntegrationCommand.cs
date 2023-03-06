using IntegrationNS.Application.DTOs;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IntegrationNS.Application.Commands.NKMHs
{
    public class NKMHIntegrationCommand : IRequest<List<NKMHResponse>>
    {
        public string Plant { get; set; }
        public string PurchasingOrg { get; set; }

        public long? PurchasingGroupFrom { get; set; }
        public long? PurchasingGroupTo { get; set; }

        public int? VendorFrom { get; set; }
        public int? VendorTo { get; set; }
        public string POType { get; set; }
        public long? PurchaseOrderFrom { get; set; }
        public long? PurchaseOrderTo { get; set; }
        public long? MaterialFrom { get; set; }
        public long? MaterialTo { get; set; }

        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        public string WeightHead { get; set; }
        public List<string> WeightVotes { get; set; } = new List<string>();
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        //public bool? IsReverse { get; set; }
        public string Status { get; set; }

    }
    public class NKMHIntegrationCommandHandler : IRequestHandler<NKMHIntegrationCommand, List<NKMHResponse>>
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IRepository<AccountModel> _userRep;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;

        public NKMHIntegrationCommandHandler(IRepository<GoodsReceiptModel> nkmhRep, IRepository<AccountModel> userRep, IRepository<WeighSessionModel> weightSsRepo)
        {
            _nkmhRep = nkmhRep;
            _userRep = userRep;
            _weightSsRepo = weightSsRepo;
        }
        public async Task<List<NKMHResponse>> Handle(NKMHIntegrationCommand request, CancellationToken cancellationToken)
        {

            #region Format day and Search by day

            if (request.ToTime.HasValue)
            {
                request.ToTime = request.ToTime.Value.Date.AddDays(1).AddSeconds(-1);
            }

            if (request.FromTime.HasValue)
            {
                request.FromTime = request.FromTime.Value.Date;
            }

            if (request.WeightDateTo.HasValue)
            {
                request.WeightDateTo = request.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }

            if (request.WeightDateFrom.HasValue)
            {
                request.WeightDateFrom = request.WeightDateFrom.Value.Date;
            }
            #endregion

            var users = _userRep.GetQuery().AsNoTracking();

            var query = await _nkmhRep.GetQuery(x => request.FromTime.HasValue ? x.DocumentDate >= request.FromTime && x.DocumentDate <= request.ToTime : true)
                                      .Include(x => x.PurchaseOrderDetail)
                                      .ThenInclude(x => x.PurchaseOrder)
                                      .AsNoTracking()
                                      .ToListAsync();

            //WeightSS
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Search Plant
            if (!string.IsNullOrEmpty(request.Plant))
            {
                query = query.Where(x => x.PlantCode == request.Plant).ToList();
            }

            //Search PurchasingOrg
            if (!string.IsNullOrEmpty(request.PurchasingOrg))
            {
                query = query.Where(x => x.PurchaseOrderDetail is null ? false :
                                         x.PurchaseOrderDetail.PurchaseOrder.PurchasingOrg == request.PurchasingOrg).ToList();
            }

            //Search Purchasing Group
            if (request.PurchasingGroupFrom.HasValue)
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         long.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup) >= request.PurchasingGroupFrom &&
                                         long.Parse(x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup) <= request.PurchasingGroupTo).ToList();
            }

            //Search Vendor
            if (request.VendorFrom.HasValue)
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.VendorCodeInt >= request.VendorFrom &&
                                         x.PurchaseOrderDetail.PurchaseOrder.VendorCodeInt <= request.VendorTo).ToList();
            }

            //Search PO TYPE
            if (!string.IsNullOrEmpty(request.POType))
            {
                query = query.Where(x => x.PurchaseOrderDetail == null ? true :
                                         x.PurchaseOrderDetail.PurchaseOrder.POType.Contains(request.POType)).ToList();
            }


            //Search PO
            if (request.PurchaseOrderFrom.HasValue)
            {
                query = query.Where(x => x.PurchaseOrderDetail is null ? false :
                                         x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCodeInt >= request.PurchaseOrderFrom &&
                                         x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCodeInt <= request.PurchaseOrderTo).ToList();
            }

            //Search Material
            if (request.MaterialFrom.HasValue)
            {
                if (!request.MaterialTo.HasValue) request.MaterialTo = request.MaterialFrom;

                query = query.Where(x => x.MaterialCodeInt >= request.MaterialFrom &&
                                         x.MaterialCodeInt <= request.MaterialTo).ToList();
            }


            //Search WeightHead
            if (!string.IsNullOrEmpty(request.WeightHead))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == request.WeightHead.Trim().ToLower() : false).ToList();
            }

            //Search WeightDate
            if (request.WeightDateFrom.HasValue)
            {
                if (!request.WeightDateTo.HasValue) request.WeightDateTo = request.WeightDateFrom;

                query = query.Where(x => x.WeighDate >= request.WeightDateFrom &&
                                         x.WeighDate <= request.WeightDateTo).ToList();
            }

            //Search WeightVotes
            if (request.WeightVotes != null && request.WeightVotes.Any())
            {
                query = query.Where(x => request.WeightVotes.Contains(x.WeitghtVote)).ToList();
            }

            //if (request.IsReverse == true)
            //{
            //    query = query.Where(x => !string.IsNullOrEmpty(x.MaterialDocument) && !string.IsNullOrEmpty(x.ReverseDocument)).ToList();
            //}

            //if (request.IsReverse == false)
            //{
            //    query = query.Where(x => string.IsNullOrEmpty(x.MaterialDocument) && string.IsNullOrEmpty(x.ReverseDocument)).ToList();
            //}

            //Search Status
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(x => x.Status == request.Status && x.ReverseDocument == null).ToList();
            }

            //Lấy ra dòng không có reverseDoc
            query = query.Where(x => x.ReverseDocument is null).ToList();

            var data = query.AsEnumerable()
                            .Select(x => new NKMHResponse
                            {
                                //Ngày thực hiện cân
                                WeightDate = x.WeighDate,

                                NkmhId = x.GoodsReceiptId,
                                //ID đợt cân
                                WeightId = x.WeightId.HasValue ? weightSs.FirstOrDefault(w => w.WeighSessionID == x.WeightId).WeighSessionCode : "",
                                //Số đầu cân
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
                                TotalQuantity = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : x.PurchaseOrderDetail?.OrderQuantity,
                                //Delivered Quantity
                                DeliveredQuantity = !string.IsNullOrEmpty(x.MaterialDocument) ? x.DeliveryQuantity : x.PurchaseOrderDetail?.QuantityReceived,
                                //Số xe tải
                                TruckQuantity = x.TruckQuantity,
                                //Số cân đầu vào
                                InputWeight = x.InputWeight,
                                //Số cân đầu ra
                                OutputWeight = x.OutputWeight,
                                //Ghi chú
                                Description = x.Description,
                                //Hình ảnh
                                Image = x.Img,
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
                                Material = x.MaterialCode,
                                //DocumentDate
                                DocumentDate = x.PurchaseOrderDetail?.PurchaseOrder?.DocumentDate,
                                //Storage Location
                                StorageLocation = x.SlocCode,
                                //Số lô
                                Batch = x.Batch,
                                //Số phương tiện
                                VehicleCode = x.VehicleCode,

                                //Số lượng đặt hàng
                                OrderQuantity = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : x.PurchaseOrderDetail?.OrderQuantity,
                                OpenQuantity = !string.IsNullOrEmpty(x.MaterialDocument) ? x.OpenQuantity : x.PurchaseOrderDetail?.OpenQuantity,
                                //Mat Doc
                                MaterialDocument = x.MaterialDocument,
                                //Reverse Doc
                                ReverseDocument = x.ReverseDocument,
                                
                            }).ToList();
            return data;
        }
    }
}
