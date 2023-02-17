using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.XK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XK;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Queries
{
    public interface IXKQuery
    {
        /// <summary>
        /// Lấy dữ liệu đầu vào
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<GetInputDataResponse> GetInputData(SearchXKCommand command);

        /// <summary>
        /// Lấy dư liệu đã lưu xk
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<SearchXKResponse> GetDataXK(SearchXKCommand command);

        /// <summary>
        /// Drop down số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);
    }

    public class XKQuery : IXKQuery
    {
        private readonly IRepository<OrderExportModel> _xkRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<CustmdSaleModel> _custRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<DetailReservationModel> _dtResRepo;

        public XKQuery(IRepository<OrderExportModel> nkRepo, IRepository<PlantModel> plantRepo, IRepository<ProductModel> prdRepo, IRepository<CustmdSaleModel> custRepo,
                       IRepository<CatalogModel> cataRepo, IRepository<AccountModel> userRepo, IRepository<DetailReservationModel> dtResRepo)
        {
            _xkRepo = nkRepo;
            _plantRepo = plantRepo;
            _prdRepo = prdRepo;
            _custRepo = custRepo;
            _cataRepo = cataRepo;
            _userRepo = userRepo;
            _dtResRepo = dtResRepo;
        }
        public Task<SearchXKResponse> GetDataXK(SearchXKCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<GetInputDataResponse> GetInputData(SearchXKCommand command)
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

            ////Theo lệnh sản xuát
            //if (!string.IsNullOrEmpty(command.WorkorderFrom))
            //{
            //    //Nếu không có To thì search 1
            //    if (string.IsNullOrEmpty(command.WorkorderTo))
            //        command.WorkorderTo = command.WorkorderFrom;
            //    query = query.Where(x => x.WorkOrder.WorkOrderCodeInt >= long.Parse(command.WorkorderFrom) &&
            //                             x.WorkOrder.WorkOrderCodeInt <= long.Parse(command.WorkorderTo) &&
            //                             x.RequirementQuantiy <= 0);
            //}

            ////Theo Order Type
            //if (!string.IsNullOrEmpty(command.OrderType))
            //{
            //    query = query.Where(x => x.WorkOrder.OrderTypeCode == command.OrderType);
            //}

            ////Theo sale order
            //if (!string.IsNullOrEmpty(command.SalesOrderFrom))
            //{

            //    //Nếu không có To thì search 1
            //    if (string.IsNullOrEmpty(command.SalesOrderTo))
            //        command.SalesOrderTo = command.SalesOrderFrom;
            //    query = query.Where(x => x.WorkOrder.SalesOrder.CompareTo(command.SalesOrderFrom) >= 0 &&
            //                             x.WorkOrder.SalesOrder.CompareTo(command.SalesOrderTo) <= 0);
            //}


            ////Theo Scheduled Start
            //if (command.ScheduledStartFrom.HasValue)
            //{
            //    //Nếu không có To thì search 1
            //    if (!command.ScheduledStartTo.HasValue)
            //        command.ScheduledStartTo = command.ScheduledStartFrom.Value.Date.AddDays(1).AddSeconds(-1);
            //    query = query.Where(x => x.WorkOrder.ScheduledStartDate >= command.ScheduledStartFrom &&
            //                             x.WorkOrder.ScheduledStartDate <= command.ScheduledStartTo);
            //}

            ////Get query data material
            //var materials = _prdRepo.GetQuery().AsNoTracking();

            ////Get query data order type
            //var orderTypes = _orderTypeRepo.GetQuery().AsNoTracking();

            ////Get query data sloc
            //var slocs = _slocRepo.GetQuery().AsNoTracking();

            ////Get data
            //var data = await query.Select(x => new GetDataInputResponse
            //{
            //    //Plant
            //    Plant = x.WorkOrder.Plant ?? "",
            //    //Production Order
            //    WorkOrder = long.Parse(x.WorkOrder.WorkOrderCode).ToString() ?? "",
            //    //Material
            //    Material = long.Parse(x.WorkOrder.ProductCode).ToString() ?? "",
            //    //Material Desc
            //    MaterialDesc = materials.FirstOrDefault(m => m.ProductCode == x.WorkOrder.ProductCode).ProductName ?? "",
            //    //Item component
            //    ItemComponent = x.WorkOrderItem ?? "",
            //    //Component
            //    Component = long.Parse(x.ProductCode).ToString() ?? "",
            //    //Component Desc
            //    ComponentDesc = materials.FirstOrDefault(m => m.ProductCode == x.ProductCode).ProductName ?? "",
            //    //Order Type
            //    OrderType = !string.IsNullOrEmpty(x.WorkOrder.OrderTypeCode) ?
            //                $"{x.WorkOrder.OrderTypeCode} | {orderTypes.FirstOrDefault(o => o.OrderTypeCode == x.WorkOrder.OrderTypeCode).ShortText}" : "",
            //    //Sales Order
            //    SalesOrder = x.WorkOrder.SalesOrder ?? "",
            //    //Storage Location
            //    Sloc = x.StorageLocation ?? "",
            //    SlocName = string.IsNullOrEmpty(x.StorageLocation) ? "" : $"{x.StorageLocation} | {slocs.FirstOrDefault(s => s.StorageLocationCode == x.StorageLocation).StorageLocationName}",
            //    //Batch
            //    Batch = x.Batch ?? "",
            //    //UoM
            //    Unit = x.WorkOrder.Unit ?? "",
            //    //Schedule Start Time
            //    ScheduleStartTime = x.WorkOrder.ScheduledStartDate ?? null,
            //    //Schedule Finish Time
            //    ScheduleFinishTime = x.WorkOrder.ScheduledFinishDate ?? null,
            //    //ToTal Qty
            //    TotalQty = x.WorkOrder.TargetQuantity.HasValue ? Math.Abs(x.WorkOrder.TargetQuantity.Value) : 0,
            //    //Requirement Qty
            //    RequirementQty = x.RequirementQuantiy.HasValue ? Math.Abs(x.RequirementQuantiy.Value) : 0,
            //    //Withdraw Qty
            //    WithdrawQty = x.QuantityWithdrawn.HasValue ? Math.Abs(x.QuantityWithdrawn.Value) : 0,
            //}).ToListAsync();

            //var index = 1;
            //foreach (var item in data)
            //{
            //    item.IndexKey = index;
            //    index++;
            //}

            //return data;

            return null;
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
    }
}
