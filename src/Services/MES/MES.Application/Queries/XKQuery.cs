using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.XK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XK;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MES.Application.Queries
{
    public interface IXKQuery
    {
        /// <summary>
        /// Lấy dữ liệu đầu vào
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchXKCommand command);

        /// <summary>
        /// Lấy dư liệu đã lưu xk
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchXKResponse>> GetDataXK(SearchXKCommand command);

        /// <summary>
        /// Drop down số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Get data by reservation and reservation item
        /// </summary>
        /// <param name="resCode"></param>
        /// <param name="resItem"></param>
        /// <returns></returns>
        Task<GetDataByRsvAndRsvItemResponse> GetDataByResAndResItem(string resCode, string resItem);
    }

    public class XKQuery : IXKQuery
    {
        private readonly IRepository<OtherExportModel> _xkRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<CustmdSaleModel> _custRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<DetailReservationModel> _dtResRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public XKQuery(IRepository<OtherExportModel> nkRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prdRepo, IRepository<CustmdSaleModel> custRepo,
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
        public async Task<List<SearchXKResponse>> GetDataXK(SearchXKCommand command)
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
            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchXKResponse
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
                ReceivingSloc = x.ReceivingSlocCode ?? "",
                ReceivingSlocFmt = string.IsNullOrEmpty(x.ReceivingSlocCode) ? "" : $"{x.ReceivingSlocCode} | {x.ReceivingSlocName}",
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
                //Số xe tải
                TruckInfoId = x.TruckInfoId ?? null,
                TruckNumber = x.TruckNumber ?? "",
                //25 Ghi chú
                Description = x.Description ?? "",
                //26 Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? x.Image : "",
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
                ChangeOn = x.LastEditTime,
                //34 Material Doc
                MatDoc = x.MaterialDocument ?? null,
                //35 Reverse Doc
                RevDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
                //isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true
            }).ToListAsync();

            return data;
        }

        public async Task<List<GetInputDataResponse>> GetInputData(SearchXKCommand command)
        {
            #region Format Day

            //Scheduled Start
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

            //Movement type xk
            var movementType = new List<string> { "Z42", "Z44", "Z46", "201" };

            //Tạo query
            var query = _dtResRepo.GetQuery()
                                     .Include(x => x.Reservation)
                                     .Where(x => movementType.Contains(x.MovementType) &&
                                                 x.Reservation.FinalIssue != "X" &&
                                                 x.ItemDeleted != "X")
                                     .AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.Reservation.Plant == command.Plant);
            }

            //Theo Reservation
            if (!string.IsNullOrEmpty(command.ReservationFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.ReservationTo))
                    command.ReservationTo = command.ReservationFrom;
                query = query.Where(x => x.Reservation.ReservationCodeInt >= long.Parse(command.ReservationFrom) &&
                                         x.Reservation.ReservationCodeInt <= long.Parse(command.ReservationTo));
            }

            //Theo Customer
            if (!string.IsNullOrEmpty(command.CustomerFrom))
            {
                //Nếu không có To thì search 1
                if (string.IsNullOrEmpty(command.CustomerFrom))
                    command.CustomerTo = command.CustomerFrom;
                query = query.Where(x => x.Reservation.Customer.CompareTo(command.CustomerFrom) >= 0 &&
                                         x.Reservation.Customer.CompareTo(command.ReservationTo) <= 0);
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
                query = query.Where(x => x.RequirementsDate >= command.DocumentDateFrom &&
                                         x.RequirementsDate <= command.DocumentDateTo);
            }

            //Get query data material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            //Get query data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Query Customer
            var customers = _custRepo.GetQuery().AsNoTracking();

            //Get data
            var data = await query.Select(x => new GetInputDataResponse
            {
                //Plant
                Plant = x.Reservation.Plant ?? "",
                //Reservatiom
                Reservation = x.Reservation.ReservationCodeInt.ToString() ?? "",
                //Reservatiom item
                ReservationItem = x.ReservationItem ?? "",
                //Material
                Material = x.MaterialCodeInt.ToString() ?? "",
                //Material Desc
                MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.Material).ProductName ?? "",
                //Movement Type
                MovementType = x.MovementType ?? "",
                //Customer
                Customer = x.Reservation.Customer ?? "",
                //CustomerName
                CustomerName = !string.IsNullOrEmpty(x.Reservation.Customer) ? customers.FirstOrDefault(c => c.CustomerNumber == x.Reservation.Customer).CustomerName : "",
                //Special Stock
                SpecialStock = x.SpecialStock ?? "",
                //Storage Location
                Sloc = x.Reservation.Sloc ?? "",
                SlocFmt = string.IsNullOrEmpty(x.Reservation.Sloc) ? "" : $"{x.Reservation.Sloc} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.Reservation.Sloc).StorageLocationName}",
                //Batch
                Batch = x.Batch ?? "",
                //UoM
                Unit = x.BaseUnit ?? "",
                //ToTal Qty
                TotalQuantity = Math.Abs(x.RequirementQty),
                //Delivered Qty
                DeliveredQuantity =Math.Abs(x.QtyWithdrawn)
            }).ToListAsync();

            var index = 1;
            foreach (var item in data)
            {
                item.IndexKey = index;
                index++;
            }

            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
            {
                data.Add(new GetInputDataResponse
                {
                    Plant = command.Plant,
                    Material = long.Parse(command.MaterialFrom).ToString(),
                    MaterialDesc = materials.FirstOrDefault(m => m.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName ?? "",
                    Unit = materials.FirstOrDefault(m => m.ProductCodeInt == long.Parse(command.MaterialFrom)).Unit ?? ""
                });
            }

            return data;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _xkRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }

        public async Task<GetDataByRsvAndRsvItemResponse> GetDataByResAndResItem(string resCode, string resItem)
        {
            //Lấy ra reservation detail
            var detailRes = await _dtResRepo.GetQuery().Include(x => x.Reservation)
                                            .FirstOrDefaultAsync(x => x.Reservation.ReservationCodeInt == long.Parse(resCode) && x.ReservationItem == resItem);

            //Danh sách product
            var prods = _prdRepo.GetQuery().AsNoTracking();

            var response = new GetDataByRsvAndRsvItemResponse
            {
                //Material
                Material = prods.FirstOrDefault(p => p.ProductCodeInt == detailRes.MaterialCodeInt).ProductCodeInt.ToString(),
                //Material Desc
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCodeInt == detailRes.MaterialCodeInt).ProductName,
                //Movement type
                MovementType = detailRes.MovementType ?? "",
                //Batch
                Batch = detailRes.Batch ?? "",
                //Total Quantity
                TotalQty = detailRes.RequirementQty,
                //Delivered Quantity
                DeliveryQty = detailRes.QtyWithdrawn,
                //Unit
                Unit = detailRes.BaseUnit
            };

            return response;
        }
    }
}
