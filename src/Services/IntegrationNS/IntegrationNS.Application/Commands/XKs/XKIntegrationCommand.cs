using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.XKs
{
    public class XKIntegrationCommand : IRequest<List<XKResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Reservatopm
        public string ReservationFrom { get; set; }
        public string ReservationTo { get; set; }
        //Sloc
        public string SlocFrom { get; set; }
        public string SlocTo { get; set; }
        //Customer
        public string CustomerFrom { get; set; }
        public string CustomerTo { get; set; }
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
        //Ngày thực hiện cân
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        //CreateBy
        public Guid? CreateBy { get; set; }
        //Status
        public string Status { get; set; }
    }

    public class XKIntegrationCommandHandler : IRequestHandler<XKIntegrationCommand, List<XKResponse>>
    {
        private readonly IRepository<OtherExportModel> _xkRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<CustmdSaleModel> _custRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<DetailReservationModel> _dtResRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        public XKIntegrationCommandHandler(IRepository<OtherExportModel> nkRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prdRepo, IRepository<CustmdSaleModel> custRepo,
                                           IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo, IRepository<DetailReservationModel> dtResRepo,
                                           IRepository<StorageLocationModel> slocRepo)
        {
            _xkRepo = nkRepo;
            _plantRepo = plantRepo;
            _prdRepo = prdRepo;
            _custRepo = custRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _dtResRepo = dtResRepo;
            _slocRepo = slocRepo;
        }

        public async Task<List<XKResponse>> Handle(XKIntegrationCommand command, CancellationToken cancellationToken)
        {
            #region Format Day
            //Ngày thực hiện cân
            if (command.WeightDateFrom.HasValue)
            {
                command.WeightDateFrom = command.WeightDateFrom.Value.Date;
            }
            if (command.WeightDateTo.HasValue)
            {
                command.WeightDateTo = command.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
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

            //Tạo query
            var query = _xkRepo.GetQuery().Include(x => x.DetailReservation).ThenInclude(x => x.Reservation)
                               .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo Reservation
            if (!string.IsNullOrEmpty(command.ReservationFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.ReservationTo))
                    command.ReservationTo = command.ReservationFrom;
                query = query.Where(x => x.DetailReservationId.HasValue ? x.DetailReservation.Reservation.ReservationCodeInt >= long.Parse(command.ReservationFrom) &&
                                                                          x.DetailReservation.Reservation.ReservationCodeInt <= long.Parse(command.ReservationTo) : false);
            }

            //Theo Sloc
            if (!string.IsNullOrEmpty(command.SlocFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.SlocTo))
                    command.SlocTo = command.SlocFrom;
                query = query.Where(x => x.SlocCode.CompareTo(command.SlocFrom) >= 0 &&
                                         x.SlocCode.CompareTo(command.SlocTo) <= 0);
            }

            //Theo Customer
            if (!string.IsNullOrEmpty(command.CustomerFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.CustomerFrom))
                    command.CustomerTo = command.CustomerFrom;
                query = query.Where(x => x.DetailReservationId.HasValue ? x.DetailReservation.Reservation.Customer.CompareTo(command.CustomerFrom) >= 0 &&
                                                                          x.DetailReservation.Reservation.Customer.CompareTo(command.ReservationTo) <= 0 : false);
            }

            //Theo Material
            if (!string.IsNullOrEmpty(command.MaterialFrom))
            {
                //Nếu không có To thì search 1
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
                query = query.Where(x => x.DetailReservationId.HasValue ? x.DetailReservation.RequirementsDate >= command.DocumentDateFrom &&
                                                                          x.DetailReservation.RequirementsDate <= command.DocumentDateTo : false);
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

            //Query Material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Query Customer
            var customers = _custRepo.GetQuery().AsNoTracking();

            //User Query
            var user = _userRepo.GetQuery().AsNoTracking();

            //Sloc query
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Catalog status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();


            //Get data
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new XKResponse
            {
                //ID NK
                XKId = x.OtherExportId,
                //7 Plant
                Plant = x.PlantCode ?? "",
                //Reservation
                Reservation = x.DetailReservationId.HasValue ? x.DetailReservation.Reservation.ReservationCodeInt.ToString() : "",
                //Reservation item
                ReservationItem = x.DetailReservationId.HasValue ? x.DetailReservation.ReservationItem : "",
                //9 Material
                Material = x.MaterialCodeInt.ToString() ?? "",
                //10 Material Desc
                MaterialDesc = !string.IsNullOrEmpty(x.MaterialCode) ? materials.FirstOrDefault(m => m.ProductCode == x.MaterialCode).ProductName : "",
                //13 Stor.Loc
                Sloc = x.SlocCode ?? "",
                SlocFmt = string.IsNullOrEmpty(x.SlocCode) ? "" : $"{x.SlocCode} | {x.SlocName}",
                //Receiving sloc
                ReceivingSloc = x.DetailReservationId.HasValue ? x.DetailReservation.Reservation.ReceivingSloc : "",
                ReceivingSlocFmt = x.DetailReservationId.HasValue ?
                                   $"{x.DetailReservation.Reservation.ReceivingSloc} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.DetailReservation.Reservation.ReceivingSloc).StorageLocationName}" : "",
                //14 Batch
                Batch = x.Batch ?? "",
                //Special Stock
                SpecialStock = x.SpecialStock ?? "",
                //15 SL bao
                BagQuantity = x.BagQuantity ?? 0,
                //16 Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //17 Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //18 Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Customer
                Customer = x.Customer ?? "",
                //Customer name
                CustomerName = !string.IsNullOrEmpty(x.Customer) ? customers.FirstOrDefault(c => c.CustomerNumber == x.Customer).CustomerName : "",
                //19 Confirm Quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //20 SL kèm bao bì
                QtyWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //21 Số lần cân
                QtyWeight = x.QuantityWeight ?? 0,
                //Total qty
                TotalQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.TotalQuantity : x.DetailReservationId.HasValue ? x.DetailReservation.RequirementQty : 0,
                //Delivered qty
                DeliveryQty = !string.IsNullOrEmpty(x.MaterialDocument) ? x.DeliveredQuantity : x.DetailReservationId.HasValue ? x.DetailReservation.QtyWithdrawn : 0,
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //24 UOM
                Unit = x.UOM ?? "",
                //Document date
                DocumentDate = x.DetailReservationId.HasValue ? x.DetailReservation.RequirementsDate : null,
                //Số xe tải
                TruckInfoId = x.TruckInfoId ?? null,
                TruckNumber = x.TruckNumber ?? "",
                //25 Ghi chú
                Description = x.Description ?? "",
                //26 Hình ảnh
                Image = string.IsNullOrEmpty(x.Image) ? "" : $"https://itp-mes.isdcorp.vn/{x.Image}",
                //27 Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //28 Số phiếu cân
                WeightVote = x.WeightVote ?? "",
                //29 Thời gian bắt đầu
                StartTime = x.StartTime ?? null,
                //30 Thời gian kết thúc
                EndTime = x.EndTime ?? null,
                //31 Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //32 Crete On
                CreateOn = x.CreateTime ?? null,
                //33 Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                //34 Material Doc
                MatDoc = x.MaterialDocument ?? "",
                //Mat doc item
                MatDocItem = x.MaterialDocumentItem ?? "",
                //35 Reverse Doc
                RevDoc = x.ReverseDocument ?? "",
                isDelete = x.Status == "DEL" ? true : false
            }).ToListAsync();

            return data;
        }
    }
}
