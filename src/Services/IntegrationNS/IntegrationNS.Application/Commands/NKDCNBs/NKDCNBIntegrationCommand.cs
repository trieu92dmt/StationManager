using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs.MES.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationNS.Application.Commands.NKDCNBs
{
    public class SearchNKDCNBCommand : IRequest<List<SearchNKDCNBResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Shipping Point
        public string ShippingPoint { get; set; }
        //Purchase Order
        public string PurchaseOrderFrom { get; set; }
        public string PurchaseOrderTo { get; set; }
        //Outbound Delivery
        public string OutboundDeliveryFrom { get; set; }
        public string OutboundDeliveryTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Document Date
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân 
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân from
        public DateTime? WeightDateFrom { get; set; }
        //Ngày thực hiện cân to
        public DateTime? WeightDateTo { get; set; }
        //Create by
        public Guid? CreateBy { get; set; }

        //Status
        public string Status { get; set; }
    }

    public class SearchNKDCNBCommandHandler : IRequestHandler<SearchNKDCNBCommand, List<SearchNKDCNBResponse>>
    {
        private readonly IRepository<InhouseTransferModel> _nkdcnbRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailOdRepo;
        private readonly IRepository<PurchaseOrderDetailModel> _detailPoRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<OutboundDeliveryModel> _odRepo;

        public SearchNKDCNBCommandHandler(IRepository<InhouseTransferModel> nkdcnbRepo, IRepository<DetailOutboundDeliveryModel> detailOdRepo,
                           IRepository<PurchaseOrderDetailModel> detailPoRepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                           IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo, IRepository<OutboundDeliveryModel> odRepo)
        {
            _nkdcnbRepo = nkdcnbRepo;
            _detailOdRepo = detailOdRepo;
            _detailPoRepo = detailPoRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _odRepo = odRepo;
        }
        public async Task<List<SearchNKDCNBResponse>> Handle(SearchNKDCNBCommand command, CancellationToken cancellationToken)
        {
            #region Format Day

            if (command.DocumentDateFrom.HasValue)
            {
                command.DocumentDateFrom = command.DocumentDateFrom.Value.Date;
            }
            if (command.DocumentDateTo.HasValue)
            {
                command.DocumentDateTo = command.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }

            //Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                command.WeightDateFrom = command.WeightDateFrom.Value.Date;
            }
            if (command.WeightDateTo.HasValue)
            {
                command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Tạo query detail po
            var poQuery = _detailPoRepo.GetQuery().Include(x => x.PurchaseOrder).AsNoTracking();

            //Tạo query detail od
            var query = _nkdcnbRepo.GetQuery()
                                        .Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery)
                                        //Lọc delivery type
                                        .Where(x => (x.DetailOD.OutboundDelivery.DeliveryType == "ZNLC" || x.DetailOD.OutboundDelivery.DeliveryType == "ZNLN") &&
                                                    //Lấy delivery đã hoàn tất giao dịch
                                                    x.DetailOD.OutboundDelivery.GoodsMovementSts == "C" &&
                                                    x.DetailOD.GoodsMovementSts == "C")
                                        .AsNoTracking();

            //Check điều kiện 3
            //Loại trừ các delivery có po đã hoàn tất nhập kho
            query = query.Where(x => poQuery.FirstOrDefault(p => p.POLine == x.DetailOD.ReferenceItem && p.PurchaseOrder.PurchaseOrderCode == x.DetailOD.ReferenceDocument1).DeliveryCompleted != "X" &&
                                     //Loại trừ các delivery đã đánh dấu xóa
                                     poQuery.FirstOrDefault(p => p.POLine == x.DetailOD.ReferenceItem && p.PurchaseOrder.PurchaseOrderCode == x.DetailOD.ReferenceDocument1).DeletionInd != "L");


            //Products
            var prods = _prdRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Plant
            //var plants = _plantRepo.GetQuery().AsNoTracking();

            //NKDCNB
            var nkdcnbs = _nkdcnbRepo.GetQuery().AsNoTracking();

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var user = _userRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo shipping point
            if (!string.IsNullOrEmpty(command.ShippingPoint))
            {
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShippingPoint == command.ShippingPoint : false);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;

                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo Purchase order
            if (!string.IsNullOrEmpty(command.PurchaseOrderFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.PurchaseOrderTo))
                    command.PurchaseOrderTo = command.PurchaseOrderFrom;

                query = query.Where(x => x.DetailODId.HasValue ? (x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderFrom) >= 0 &&
                                                                 x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderTo) <= 0) : false);
            }

            //Theo outbound deliver
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

                query = query.Where(x => x.DetailODId.HasValue ? (x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
                                                                 x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0) : false);
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                //Không có to thì search 1
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.DetailODId.HasValue ? (x.DetailOD.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                                                 x.DetailOD.OutboundDelivery.DocumentDate <= command.DocumentDateTo) : false);

            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(command.WeightHeadCode))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == command.WeightHeadCode.Trim().ToLower() : false);
            }

            //Check Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                if (!command.WeightDateTo.HasValue) command.WeightDateTo = command.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.CreateTime >= command.WeightDateFrom &&
                                         x.CreateTime <= command.WeightDateTo);
            }

            //Check số phiếu cân
            if (command.WeightVotes != null && command.WeightVotes.Any())
            {
                query = query.Where(x => command.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (command.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == command.CreateBy);
            }

            //Search Status
            if (!string.IsNullOrEmpty(command.Status))
            {
                query = query.Where(x => x.Status == command.Status && x.ReverseDocument == null);
            }

            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchNKDCNBResponse
            {
                //Id
                NKDCNBId = x.InhouseTransferId,
                //Plant
                Plant = x.PlantCode ?? "",
                //Shipping point
                ShippingPoint = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShippingPoint : "",
                //OD
                OutboundDelivery = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCode : "",
                //OD item
                OutboundDeliveryItem = x.DetailODId.HasValue ? x.DetailOD.OutboundDeliveryItem : "",
                //Material
                Material = x.MaterialCode ?? "",
                //Material Desc
                MaterialDesc = prods.FirstOrDefault(m => m.ProductCode == x.MaterialCode).ProductName ?? "",
                //Storage Location
                Sloc = x.SlocCode ?? "",
                SlocDesc = x.SlocName ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //Số lượng bao
                BagQuantity = x.BagQuantity ?? 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QtyWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Đơn vị vận chuyển
                TransportUnit = x.TransportUnit ?? "",
                //Số lần cân
                QtyWeight = x.QuantityWeitght ?? 0,
                //Total quantity
                TotalQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : x.DetailODId.HasValue ? x.DetailOD.DeliveryQuantity : 0,
                //Delivery Quantity
                DeliveryQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.DeliveredQuantity : nkdcnbs.Where(n => n.DetailODId == x.DetailODId).Sum(n => n.ConfirmQty) ?? 0,
                //UoM
                Unit = x.UOM ?? "",
                //Purchase Order
                PurchaseOrder = x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument1 : "",
                //Số xe tải
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = string.IsNullOrEmpty(x.Image) ? "" : $"https://itp-mes.isdcorp.vn/{x.Image}",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote ?? "",
                //Thời gian bắt đầu
                StartTime = x.StartTime,
                //Thời gian kết thúc
                EndTime = x.EndTime,
                //Document Date
                DocumentDate = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate : null,
                //Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //Crete On
                CreateOn = x.CreateTime ?? null,
                //Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //Material Doc
                MatDoc = x.MaterialDocument ?? "",
                //MatDocItem
                MatDocItem = x.MaterialDocumentItem ?? "",
                //Reverse Doc
                RevDoc = x.ReverseDocument ?? "",
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true
            }).ToListAsync();

            return data;
        }
    }
}
