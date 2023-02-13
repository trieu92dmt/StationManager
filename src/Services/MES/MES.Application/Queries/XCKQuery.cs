using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.XCK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.OutboundDelivery;
using MES.Application.DTOs.MES.XCK;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

    public class XCKQuery : IXCKQuery
    {
        private readonly IRepository<WarehouseTransferModel> _xckRepo;
        private readonly IRepository<ReservationModel> _reserRepo;
        private readonly IRepository<DetailReservationModel> _detailReserRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public XCKQuery(IRepository<WarehouseTransferModel> xckRepo, IRepository<ReservationModel> reserRepo, IRepository<DetailReservationModel> detailReserRepo,
                        IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prodRepo)
        {
            _xckRepo = xckRepo;
            _reserRepo = reserRepo;
            _detailReserRepo = detailReserRepo;
            _plantRepo = plantRepo;
            _prodRepo = prodRepo;
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
                //8. Receving Sloc
                ReceivingSloc = x.Reservation.ReceivingSloc ?? "",
                //9. Batch
                Batch = x.Batch,
                //10. Total Quantity
                //TotalQty = x.
                //11. Delivered Quantity
                //12. Open Quantity
                //13. UoM

        }).ToListAsync();

            //Tính open quantity
            //foreach (var item in data)
            //{
            //    item.OpenQty = item.TotalQty - item.DeliveryQty;
            //}

            //if (!string.IsNullOrEmpty(command.MaterialFrom) && command.MaterialFrom == command.MaterialTo)
            //{
            //    data.Add(new OutboundDeliveryResponse
            //    {
            //        Plant = command.PlantCode,
            //        Material = long.Parse(command.MaterialFrom).ToString(),
            //        MaterialDesc = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).ProductName,
            //        Unit = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(command.MaterialFrom)).Unit
            //    });
            //}

            return data;
        }
    }
}
