using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.XKLXH
{
    public class XKLXHIntegrationCommand : IRequest<List<XKLXHResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Delivery Type
        public string DeliveryType { get; set; }
        //Purchase order
        public string PurchaseOrderFrom { get; set; }
        public string PurchaseOrderTo { get; set; }
        //Sales Order
        public string SalesOrderFrom { get; set; }
        public string SalesOrderTo { get; set; }
        //Ship to party
        public string ShipToPartyFrom { get; set; }
        public string ShipToPartyTo { get; set; }
        //Outbound Delivery
        public string OutboundDeliveryFrom { get; set; }
        public string OutboundDeliveryTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Document date
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        //CreateBy
        public Guid? CreateBy { get; set; }
        //Status
        public string Status { get; set; }
    }

    public class XKLXHIntegrationCommandHandler : IRequestHandler<XKLXHIntegrationCommand, List<XKLXHResponse>>
    {
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        public XKLXHIntegrationCommandHandler(IRepository<ProductModel> prodRepo,
                                              IRepository<StorageLocationModel> slocRepo, IRepository<ExportByCommandModel> xklxhRepo,
                                              IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo)
        {
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _xklxhRepo = xklxhRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
        }

        public async Task<List<XKLXHResponse>> Handle(XKLXHIntegrationCommand command, CancellationToken cancellationToken)
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

            //Ngày cân
            if (command.WeightDateFrom.HasValue)
            {
                command.WeightDateFrom = command.WeightDateFrom.Value.Date;
            }
            if (command.WeightDateTo.HasValue)
            {
                command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            var user = _userRepo.GetQuery().AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query xklxh
            var query = _xklxhRepo.GetQuery()
                                  .Include(x => x.DetailOD).ThenInclude(x => x.OutboundDelivery).AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo delivery type
            if (!string.IsNullOrEmpty(command.DeliveryType))
            {
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryType == command.DeliveryType : false);
            }

            //Theo purchase order
            if (!string.IsNullOrEmpty(command.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(command.PurchaseOrderTo))
                    command.PurchaseOrderTo = command.PurchaseOrderFrom;

                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderFrom) >= 0 &&
                                                                 x.DetailOD.ReferenceDocument1.CompareTo(command.PurchaseOrderTo) <= 0 : false);
            }

            //Theo sale order
            if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            {
                if (string.IsNullOrEmpty(command.SalesOrderTo))
                    command.SalesOrderTo = command.SalesOrderFrom;

                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.ReferenceDocument2.CompareTo(command.SalesOrderFrom) >= 0 &&
                                                                 x.DetailOD.ReferenceDocument2.CompareTo(command.SalesOrderTo) <= 0 : false);
            }

            //Theo outbound deliver
            if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
            {
                if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
                    command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
                                                                 x.DetailOD.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0 : false);
            }

            //Theo ship to party
            if (!string.IsNullOrEmpty(command.ShipToPartyFrom))
            {
                if (string.IsNullOrEmpty(command.ShipToPartyTo))
                    command.ShipToPartyTo = command.ShipToPartyFrom;
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyFrom) >= 0 &&
                                                                 x.DetailOD.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyTo) <= 0 : false);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                if (string.IsNullOrEmpty(command.MaterialTo))
                    command.MaterialTo = command.MaterialFrom;

                query = query.Where(x => x.MaterialCodeInt >= long.Parse(command.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(command.MaterialTo));
            }

            //Theo document date
            if (command.DocumentDateFrom.HasValue)
            {
                if (!command.DocumentDateTo.HasValue)
                {
                    command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
                                                                 x.DetailOD.OutboundDelivery.DocumentDate <= command.DocumentDateTo : false);
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

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.CreateTime).Select(x => new XKLXHResponse
            {
                //Id
                XKLXHId = x.ExportByCommandId,
                //Plant
                Plant = x.PlantCode,
                //Ship to party name
                ShipToPartyName = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.ShiptoPartyName : "",
                //Od
                OutboundDelivery = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DeliveryCode : "",
                //Od item
                OutboundDeliveryItem = x.DetailODId.HasValue ? x.DetailOD.OutboundDeliveryItem : "",
                //Material
                Material = x.MaterialCodeInt.ToString(),
                //Material desc
                MaterialDesc = !string.IsNullOrEmpty(x.MaterialCode) ? prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName : "",
                //Sloc
                Sloc = x.SlocCode ?? "",
                SlocName = x.SlocName ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //Sl bao
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
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeight ?? 0,
                //Total quantity
                TotalQty = x.DetailODId.HasValue ? x.DetailOD.DeliveryQuantity : 0,
                //Delivered quantity
                DeliveredQty = x.DetailODId.HasValue ? x.DetailOD.PickedQuantityPUoM : 0,
                //UoM
                Unit = x.UOM ?? "",
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"https://itp-mes.isdcorp.vn/{x.Image}" : "",
                //Trạng thái
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote,
                //Thời gian bắt đầu
                StartTime = x.StartTime,
                //Thời gian kết thúc
                EndTime = x.EndTime,
                //Document date
                DocumentDate = x.DetailODId.HasValue ? x.DetailOD.OutboundDelivery.DocumentDate : null,
                //Số xe tải
                TruckInfoId = x.TruckInfoId ?? null,
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //Trọng lượng hàng hóa
                GoodsWeight = x.GoodsWeight ?? 0,
                //số lượng bao 2
                BagQuantity2 = x.BagQuantity2 ?? 0,
                //Đơn trọng 2
                SingleWeight2 = x.SingleWeight2 ?? 0,
                //Thời gian ghi nhận 2
                RecordTime2 = x.RecordTime2,
                //Thời gian ghi nhận 3
                RecordTime3 = x.RecordTime3,
                //Create by
                CreateById = x.CreateBy,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //Create on
                CreateOn = x.CreateTime,
                //Change by
                ChangeById = x.LastEditBy,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //Material doc
                MatDoc = x.MaterialDocument,
                //Reverse doc
                RevDoc = x.ReverseDocument,
                //Đánh dấu xóa
                isDelete = x.Status == "DEL" ? true : false,

            }).ToListAsync();

            return data;
        }
    }
}
