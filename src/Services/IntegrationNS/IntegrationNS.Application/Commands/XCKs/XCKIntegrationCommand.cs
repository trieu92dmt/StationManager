using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs.MES.XCK;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.XCKs
{
    public class SearchXCKCommand : IRequest<List<SearchXCKResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Sloc
        public string SlocFrom { get; set; }
        public string SlocTo { get; set; }
        //Receving Sloc
        public string RecevingSlocFrom { get; set; }
        public string RecevingSlocTo { get; set; }
        //Reservation
        public string ReservationFrom { get; set; }
        public string ReservationTo { get; set; }
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

    public class SearchXCKCommandHandler : IRequestHandler<SearchXCKCommand, List<SearchXCKResponse>>
    {
        private readonly IRepository<WarehouseExportTransferModel> _xckRepo;
        private readonly IRepository<ReservationModel> _reserRepo;
        private readonly IRepository<DetailReservationModel> _detailReserRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public SearchXCKCommandHandler(IRepository<WarehouseExportTransferModel> xckRepo, IRepository<ReservationModel> reserRepo, IRepository<DetailReservationModel> detailReserRepo,
                        IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prodRepo,
                        IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo)
        {
            _xckRepo = xckRepo;
            _reserRepo = reserRepo;
            _detailReserRepo = detailReserRepo;
            _plantRepo = plantRepo;
            _prodRepo = prodRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
            _slocRepo = slocRepo;
        }
        public async Task<List<SearchXCKResponse>> Handle(SearchXCKCommand command, CancellationToken cancellationToken)
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

            //Get query xck
            var query = _xckRepo.GetQuery().Include(x => x.DetailReservation).ThenInclude(x => x.Reservation).AsNoTracking();

            //Lọc theo điều kiện
            //Lọc theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.PlantCode == command.Plant);
            }

            //Theo reservation
            if (!string.IsNullOrEmpty(command.ReservationFrom))
            {
                //Không có reservation to thì search 1
                if (string.IsNullOrEmpty(command.ReservationTo))
                    command.ReservationTo = command.ReservationFrom;

                query = query.Where(x => x.DetailReservationId.HasValue ? x.DetailReservation.Reservation.ReservationCode.CompareTo(command.ReservationFrom) >= 0 &&
                                                                          x.DetailReservation.Reservation.ReservationCode.CompareTo(command.ReservationTo) <= 0 : false);
            }

            //Theo sloc
            if (!string.IsNullOrEmpty(command.SlocFrom))
            {
                //Không có reveiving sloc to thì search 1
                if (string.IsNullOrEmpty(command.SlocTo))
                    command.SlocTo = command.SlocFrom;

                query = query.Where(x => x.DetailReservationId.HasValue ? x.SlocCode.CompareTo(command.SlocFrom) >= 0 &&
                                                                          x.SlocCode.CompareTo(command.SlocTo) <= 0 : false);
            }

            //Theo Receiving sloc
            if (!string.IsNullOrEmpty(command.RecevingSlocFrom))
            {
                //Không có reveiving sloc to thì search 1
                if (string.IsNullOrEmpty(command.RecevingSlocTo))
                    command.RecevingSlocTo = command.RecevingSlocFrom;

                query = query.Where(x => x.DetailReservationId.HasValue ? x.ReceivingSlocCode.CompareTo(command.RecevingSlocFrom) >= 0 &&
                                                                          x.ReceivingSlocCode.CompareTo(command.RecevingSlocTo) <= 0 : false);
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
                query = query.Where(x => x.Status == command.Status);
            }
            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.CreateTime).Select(x => new SearchXCKResponse
            {
                //Id
                XCKId = x.WarehouseTransferId,
                //Plant
                Plant = x.PlantCode,
                //Reservation
                Reservation = x.DetailReservationId.HasValue ? x.DetailReservation.Reservation.ReservationCodeInt.ToString() : "",
                //Reservation Item
                ReservationItem = x.DetailReservationId.HasValue ? x.DetailReservation.ReservationItem.ToString() : "",
                //Material
                Material = x.MaterialCode,
                //MaterialDesc
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName,
                //MVT
                MovementType = x.DetailReservationId.HasValue ? x.DetailReservation.MovementType : "",
                //Stor.Sloc
                Sloc = x.SlocCode ?? "",
                SlocName = x.SlocName ?? "",
                //Receiving Stor.Sloc
                ReceivingSloc = x.ReceivingSlocCode ?? "",
                ReceivingSlocName = x.ReceivingSlocName ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //DocumentDate
                DocumentDate = x.DetailReservationId.HasValue ? x.DetailReservation.RequirementsDate : null,
                //Sl bao
                BagQuantity = x.BagQuantity.HasValue ? x.BagQuantity : 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight.HasValue ? x.SingleWeight : 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                //Trọng lượng cân
                Weight = x.Weight.HasValue ? x.Weight : 0,
                //Confirm Qty
                ConfirmQty = x.ConfirmQty.HasValue ? x.ConfirmQty : 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging.HasValue ? x.QuantityWithPackaging.Value : 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeitght.HasValue ? x.QuantityWeitght : 0,
                //Total Quantity
                TotalQty = x.DetailReservationId.HasValue ? x.DetailReservation.RequirementQty ?? 0 : 0,
                //Delivered Quantity
                DeliveredQty = x.DetailReservationId.HasValue ? x.DetailReservation.QtyWithdrawn ?? 0 : 0,
                //UoM
                Unit = prods.FirstOrDefault(x => x.ProductCode == x.ProductCode).Unit,
                //Số xe tải
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight.HasValue ? x.InputWeight : 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight.HasValue ? x.OutputWeight : 0,
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"https://itp-mes.isdcorp.vn/{x.Image}" : "",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote,
                //Thời gian bắt đầu
                StartTime = x.StartTime,
                //Thời gian kết thúc
                EndTime = x.EndTime,
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
                //Được chỉnh sửa
                isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true
            }).ToListAsync();

            //Tính open quantity
            foreach (var item in data)
            {
                item.OpenQty = item.TotalQty - item.DeliveredQty;
            }

            return data;
        }
    }
}
