using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.NKMHs
{
    public class NKMHIntegrationCommand : IRequest<List<NKMHResponse>>
    {
        public string Plant { get; set; }
        public string PurchasingOrg { get; set; }

        public string PurchasingGroupFrom { get; set; }
        public string PurchasingGroupTo { get; set; }

        public string VendorFrom { get; set; }
        public string VendorTo { get; set; }
        public string POType { get; set; }
        public string PurchaseOrderFrom { get; set; }
        public string PurchaseOrderTo { get; set; }
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }

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

            var query = _nkmhRep.GetQuery()
                                    .Include(x => x.PurchaseOrderDetail)
                                    .ThenInclude(x => x.PurchaseOrder)
                                    .AsNoTracking();


            //WeightSS
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Lọc theo plant
            if (!string.IsNullOrEmpty(request.Plant))
            {
                query = query.Where(x => x.PlantCode == request.Plant);
            }

            //Lọc theo purchasing organization
            if (!string.IsNullOrEmpty(request.PurchasingOrg))
            {
                //Nếu ko search to thì search 1
                if (string.IsNullOrEmpty(request.PurchasingOrg)) request.PurchasingOrg = request.PurchasingOrg;
                query = query.Where(x => x.PurchaseOrderDetail.PurchaseOrder.PurchasingOrg == request.PurchasingOrg);
            }
            //Lọc theo vendor
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
                                                                                    x.PurchaseOrderDetail.PurchaseOrder.VendorCode.CompareTo(request.VendorTo) <= 0 : false);
            }
            //Lọc PO Type
            if (!string.IsNullOrEmpty(request.POType))
            {
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.POType.Contains(request.POType) : false);
            }
            //Lọc theo material
            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                query = query.Where(x => x.MaterialCodeInt >= long.Parse(request.MaterialFrom) &&
                                                 x.MaterialCodeInt <= long.Parse(request.MaterialTo));
            }
            //Lọc theo Purchasing Group
            if (!string.IsNullOrEmpty(request.PurchasingGroupFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(request.PurchasingGroupTo)) request.PurchasingGroupTo = request.PurchasingGroupFrom;
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupFrom) >= 0 &&
                                                                                    x.PurchaseOrderDetail.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupTo) <= 0 : false);
            }
            //Lọc theo PurchaseOrder
            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderFrom) >= 0 &&
                                                                                    x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderTo) <= 0 : false);
            }

            //Lọc document date
            if (request.WeightDateFrom.HasValue)
            {
                if (!request.WeightDateTo.HasValue)
                {
                    request.WeightDateTo = request.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.DocumentDate >= request.WeightDateFrom &&
                                                 x.DocumentDate <= request.WeightDateTo);
            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(request.WeightHead))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == request.WeightHead.Trim().ToLower() : false);
            }

            //Search ngày cân
            if (request.WeightDateFrom.HasValue)
            {
                //Nếu không có WeightDateTo thì lọc theo ngày WeightDateFrom
                if (!request.WeightDateTo.HasValue) request.WeightDateTo = request.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);

                query = query.Where(x => x.WeighDate >= request.WeightDateFrom &&
                x.WeighDate <= request.WeightDateTo);
            }
            //Loc theo số phiếu cân
            if (request.WeightVotes != null && request.WeightVotes.Any())
            {
                query = query.Where(x => request.WeightVotes.Contains(x.WeitghtVote));
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
                query = query.Where(x => x.Status == request.Status && x.ReverseDocument == null);
            }

            //Lấy ra dòng không có reverseDoc
            query = query.Where(x => x.ReverseDocument == null);

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
                                Batch = string.IsNullOrEmpty(x.MaterialDocument) ? 
                                                        x.PurchaseOrderDetailId.HasValue ?
                                                        x.PurchaseOrderDetail.Batch : "" : x.Batch ?? "",

                                //Số phương tiện
                                VehicleCode = x.VehicleCode,
                                //Đơn vị vận tải
                                TransportUnit = x.TransportUnit ?? "",
                                //Số lượng đặt hàng 
                                OrderQuantity = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : x.PurchaseOrderDetail?.OrderQuantity,
                                OpenQuantity = !string.IsNullOrEmpty(x.MaterialDocument) ? x.OpenQuantity : x.PurchaseOrderDetail?.OpenQuantity,
                                //Mat Doc
                                MaterialDocument = x.MaterialDocument,
                                //Mat doc item
                                MaterialDocumentItem = x.MaterialDocumentItem,
                                //Reverse Doc
                                ReverseDocument = x.ReverseDocument,
                                
                            }).ToList();
            return data;
        }
    }
}
