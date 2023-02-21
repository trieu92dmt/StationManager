using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.XCK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XCK;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static System.Reflection.Metadata.BlobBuilder;

namespace MES.Application.Queries
{
    public interface IXCKQuery
    {
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchXCKCommand command);

        /// <summary>
        /// Lấy data xck
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchXCKResponse>> GetDataXCK(SearchXCKCommand command);

        /// <summary>
        /// Get data by reservation and reservation item
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="reservationItem"></param>
        /// <returns></returns>
        Task<GetDataByRsvAndRsvItemResponse> GetDataByRsvAndRsvItem(string reservation, string reservationItem);
    }

    public class XCKQuery : IXCKQuery
    {
        private readonly IRepository<WarehouseExportTransferModel> _xckRepo;
        private readonly IRepository<ReservationModel> _reserRepo;
        private readonly IRepository<DetailReservationModel> _detailReserRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public XCKQuery(IRepository<WarehouseExportTransferModel> xckRepo, IRepository<ReservationModel> reserRepo, IRepository<DetailReservationModel> detailReserRepo,
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


        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _xckRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }

        public async Task<List<GetInputDataResponse>> GetInputData(SearchXCKCommand command)
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
            #endregion

            //Tạo query
            var query = _detailReserRepo.GetQuery()
                                        .Include(x => x.Reservation)
                                        //Lấy các reservation có movement type là 311 313
                                        .Where(x => (x.MovementType == "311" || x.MovementType == "313") &&
                                                    //Loại trừ các reservation đã hoàn tất chuyển kho
                                                    (x.Reservation.FinalIssue != "X") &&
                                                    //Loại trừ các reservation đã đánh dấu xóa
                                                    (x.ItemDeleted != "X"))
                                        .AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Lọc điều kiện
            //Theo plant
            if (!string.IsNullOrEmpty(command.Plant))
            {
                query = query.Where(x => x.Reservation.Plant == command.Plant);
            }

            //Theo reservation
            if (!string.IsNullOrEmpty(command.ReservationFrom))
            {
                //Không có reservation to thì search 1
                if (string.IsNullOrEmpty(command.ReservationTo))
                    command.ReservationTo = command.ReservationFrom;

                query = query.Where(x => x.Reservation.ReservationCode.CompareTo(command.ReservationFrom) >= 0 &&
                                         x.Reservation.ReservationCode.CompareTo(command.ReservationTo) <= 0);
            }

            //Theo Receiving sloc
            if (!string.IsNullOrEmpty(command.RecevingSlocFrom))
            {
                //Không có reveiving sloc to thì search 1
                if (string.IsNullOrEmpty(command.RecevingSlocTo))
                    command.RecevingSlocTo = command.RecevingSlocFrom;

                query = query.Where(x => x.Reservation.ReceivingSloc.CompareTo(command.RecevingSlocFrom) >= 0 &&
                                         x.Reservation.ReceivingSloc.CompareTo(command.RecevingSlocTo) <= 0);
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
                query = query.Where(x => x.RequirementsDate >= command.DocumentDateFrom &&
                                         x.RequirementsDate <= command.DocumentDateTo);
            }


            //Data 
            var data = await query.Select(x => new GetInputDataResponse
            {
                //1. Plant
                Plant = x.Reservation.Plant,
                PlantName = plants.FirstOrDefault(p => p.PlantCode == x.Reservation.Plant).PlantName,
                //2. Reservation
                Reservation = x.Reservation.ReservationCodeInt.ToString(),
                //3. Reservation Item
                ReservationItem = x.ReservationItem,
                //4. Material
                Material = x.MaterialCodeInt.HasValue ? x.MaterialCodeInt.ToString() : "",
                //5. Material Desc
                MaterialDesc = !string.IsNullOrEmpty(x.Material) ? prods.FirstOrDefault(p => p.ProductCode == x.Material).ProductName : "",
                //6. Movement Type
                MovementType = x.MovementType ?? "",
                //7. Stor.Loc
                Sloc = x.Reservation.Sloc ?? "",
                SlocName = string.IsNullOrEmpty(x.Reservation.Sloc) ? "" : slocs.FirstOrDefault(s => s.StorageLocationCode == x.Reservation.Sloc).StorageLocationName,
                //8. Receving Sloc
                ReceivingSloc = x.Reservation.ReceivingSloc ?? "",
                ReceivingSlocName = string.IsNullOrEmpty(x.Reservation.ReceivingSloc) ? "" : slocs.FirstOrDefault(s => s.StorageLocationCode == x.Reservation.ReceivingSloc).StorageLocationName,
                //9. Batch
                Batch = x.Batch,
                //10. Total Quantity
                TotalQty = x.RequirementQty ?? 0,
                //11. Delivered Quantity
                DeliveredQty = x.QtyWithdrawn ?? 0,
                //13. UoM
                Unit = x.BaseUnit

            }).ToListAsync();

            var index = 1;
            //Tính open quantity
            foreach (var item in data)
            {
                item.OpenQty = item.TotalQty - item.DeliveredQty;
                item.IndexKey = index;
                index++;
            }

            if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
            {
                data.Add(new GetInputDataResponse
                {
                    Plant = command.Plant,
                    Material = long.Parse(command.MaterialFrom).ToString(),
                    MaterialDesc = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName,
                    Unit = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).Unit
                });
            }

            return data;
        }

        public async Task<List<SearchXCKResponse>> GetDataXCK(SearchXCKCommand command)
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

            //Theo Receiving sloc
            if (!string.IsNullOrEmpty(command.RecevingSlocFrom))
            {
                //Không có reveiving sloc to thì search 1
                if (string.IsNullOrEmpty(command.RecevingSlocTo))
                    command.RecevingSlocTo = command.RecevingSlocFrom;

                query = query.Where(x => x.DetailReservationId.HasValue ? x.DetailReservation.Reservation.ReceivingSloc.CompareTo(command.RecevingSlocFrom) >= 0 &&
                                                                          x.DetailReservation.Reservation.ReceivingSloc.CompareTo(command.RecevingSlocTo) <= 0 : false);
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
            if (!command.WeightVotes.IsNullOrEmpty() || command.WeightVotes.Any())
            {
                query = query.Where(x => command.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (command.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == command.CreateBy);
            }

            //Catalog Nhập kho mua hàng status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.WeightVote).OrderByDescending(x => x.CreateTime).Select(x => new SearchXCKResponse
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
                Material = long.Parse(x.MaterialCode).ToString(),
                //MaterialDesc
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCode == x.MaterialCode).ProductName,
                //MVT
                MovementType = x.DetailReservationId.HasValue ? x.DetailReservation.MovementType : "",
                //Stor.Sloc
                Sloc = x.SlocCode ?? "",
                SlocName = !string.IsNullOrEmpty(x.SlocCode) ? x.SlocName : "",
                //Receiving Stor.Sloc
                ReceivingSloc = x.ReceivingSlocCode ?? "",
                ReceivingSlocName = !string.IsNullOrEmpty(x.ReceivingSlocCode) ? x.ReceivingSlocName : "",
                //Batch
                Batch = x.Batch ?? "",
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
                TotalQty = x.DetailReservationId.HasValue ? x.DetailReservation.RequirementQty : 0,
                //Delivered Quantity
                DeliveredQty = x.DetailReservationId.HasValue ? x.DetailReservation.QtyWithdrawn : 0,
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
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true
                //isEdit = ((x.Status == "DEL") || (!string.IsNullOrEmpty(x.MaterialDocument))) ? false : true
            }).ToListAsync();

            //Tính open quantity
            foreach (var item in data)
            {
                item.OpenQty = item.TotalQty - item.DeliveredQty;
            }

            return data;
        }

        public async Task<GetDataByRsvAndRsvItemResponse> GetDataByRsvAndRsvItem(string reservation, string reservationItem)
        {
            //Lấy ra reservation detail
            var detailRes = await _detailReserRepo.GetQuery().Include(x => x.Reservation)
                                            .FirstOrDefaultAsync(x => x.Reservation.ReservationCodeInt == long.Parse(reservation) && x.ReservationItem == reservationItem);

            //Danh sách product
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Danh sách sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            var response = new GetDataByRsvAndRsvItemResponse
            {
                //Material
                Material = prods.FirstOrDefault(p => p.ProductCodeInt == detailRes.MaterialCodeInt).ProductCodeInt.ToString(),
                //Material Desc
                MaterialDesc = prods.FirstOrDefault(p => p.ProductCodeInt == detailRes.MaterialCodeInt).ProductName,
                //Movement type
                MovementType = detailRes.MovementType ?? "",
                //Rec Sloc
                ReceivingSloc = detailRes.Reservation.ReceivingSloc,
                ReceivingSlocName = string.IsNullOrEmpty(detailRes.Reservation.ReceivingSloc) ? "" : slocs.FirstOrDefault(s => s.StorageLocationCode == detailRes.Reservation.ReceivingSloc).StorageLocationName,
                //Batch
                Batch = detailRes.Batch ?? "",
                //Total Quantity
                TotalQty = detailRes.RequirementQty ?? 0,
                //Delivered Quantity
                DeliveredQty = detailRes.QtyWithdrawn ?? 0,
                //Unit
                Unit = detailRes.BaseUnit
            };

            return response;
        }
    }
}
