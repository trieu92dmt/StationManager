using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;

namespace StationManager.Application.Queries.CarCompany
{
    public interface ITripQuery
    {
        Task<List<TripResponse>> GetListTrip(GetListTripRequest request);

        Task<TripDetailResponse> GetTripDetail(Guid tripId);
        Task<TripDataResponse> GetTripData(Guid tripId);
        Task<PagingResultSP<TripSearchResponse>> SearchTrip(SearchTripRequest request);
    }

    public class TripQuery : ITripQuery
    {
        private readonly IRepository<TripModel> _tripRepo;
        private readonly IRepository<EmployeeModel> _eeRepo;
        private readonly IRepository<districts> _districtRepo;
        private readonly IRepository<provinces> _provinceRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<CarModel> _carRepo;
        private readonly IRepository<SeatModel> _seatRepo;
        private readonly IRepository<Ticket_Seat_Mapping> _tkSeatRepo;
        private readonly IRepository<TicketModel> _ticketRepo;
        private readonly IRepository<RateModel> _rateRepo;

        public TripQuery(IRepository<TripModel> tripRepo, IRepository<EmployeeModel> eeRepo,
                         IRepository<districts> districtRepo, IRepository<provinces> provinceRepo,
                         IRepository<CarCompanyModel> carCompanyRepo, IRepository<CarModel> carRepo,
                         IRepository<SeatModel> seatRepo, IRepository<Ticket_Seat_Mapping> tkSeatRepo,
                         IRepository<TicketModel> ticketRepo,
                         IRepository<RateModel> rateRepo)
        {
            _tripRepo = tripRepo;
            _eeRepo = eeRepo;
            _districtRepo = districtRepo;
            _provinceRepo = provinceRepo;
            _carCompanyRepo = carCompanyRepo;
            _carRepo = carRepo;
            _seatRepo = seatRepo;
            _tkSeatRepo = tkSeatRepo;
            _ticketRepo = ticketRepo;
            _rateRepo = rateRepo;
        }

        public async Task<List<TripResponse>> GetListTrip(GetListTripRequest request)
        {
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            var provinces = _provinceRepo.GetQuery().AsNoTracking();

            var districts = _districtRepo.GetQuery()
                                         .Select(x => new
                                         {
                                             Code = x.code,
                                             Name = x.full_name,
                                             ProvinceCode = x.province_code,
                                             Province = provinces.FirstOrDefault(p => p.code == x.province_code).name
                                         });

            var cars = _carRepo.GetQuery().Include(x => x.CarType).AsNoTracking();

            var drivers = _eeRepo.GetQuery(x => x.Position == "TAIXE").AsNoTracking();

            var ticketSeats = _tkSeatRepo.GetQuery().Include(x => x.Ticket)
                                                    .Include(x => x.Seat).AsNoTracking();

            var response = await _tripRepo.GetQuery(x => x.CarCompanyId == carCompany.CarCompanyId &&
                                             (!string.IsNullOrEmpty(request.StartPoint) ? x.StartPoint == request.StartPoint : true) &&
                                             (!string.IsNullOrEmpty(request.EndPoint) ? x.EndPoint == request.EndPoint : true) &&
                                             (request.StartDate.HasValue ? x.StartDate.Value.Date == request.StartDate.Value.Date : true) &&
                                             (!string.IsNullOrEmpty(request.CarNumber) ? x.CarNumber == request.CarNumber : true) &&
                                             (!string.IsNullOrEmpty(request.Driver) ? x.Driver == request.Driver : true) &&
                                             (!string.IsNullOrEmpty(request.CarType) ?
                                             cars.FirstOrDefault(c => c.CarNumber == x.CarNumber).CarType.CarTypeCode == request.CarType : true))
                                    .OrderByDescending(x => x.TripCode)
                                    .Select(x => new TripResponse
                                    {
                                        TripId = x.TripId,
                                        TripCode = x.TripCode.ToString(),
                                        CarNumber = x.CarNumber,
                                        Description = x.Description,
                                        Driver = drivers.FirstOrDefault(d => d.EmployeeCode.ToString() == x.Driver).EmployeeName,
                                        EndPoint = districts.FirstOrDefault(d => d.Code == x.EndPoint) != null ?
                                                   $"{districts.FirstOrDefault(d => d.Code == x.EndPoint).Name} - {districts.FirstOrDefault(d => d.Code == x.EndPoint).Province}" : districts.FirstOrDefault(d => d.ProvinceCode == x.EndPoint).Province,
                                        StartPoint = districts.FirstOrDefault(d => d.Code == x.StartPoint) != null ?
                                                    $"{districts.FirstOrDefault(d => d.Code == x.StartPoint).Name} - {districts.FirstOrDefault(d => d.Code == x.StartPoint).Province}" : districts.FirstOrDefault(d => d.ProvinceCode == x.StartPoint).Province,
                                        StartDate = x.StartDate ?? null,
                                        CarType = cars.FirstOrDefault(c => c.CarNumber == x.CarNumber).CarType.CarTypeName,
                                        SeatQuantity = cars.FirstOrDefault(c => c.CarNumber == x.CarNumber).CarType.SeatQuantity ?? 0,
                                        SeatUsing = ticketSeats.Where(t => t.Ticket.TripId == x.TripId).Count(),
                                    }).ToListAsync();
            return response;
        }

        public async Task<TripDataResponse> GetTripData(Guid tripId)
        {
            var trip = await _tripRepo.FindOneAsync(x => x.TripId == tripId);

            var car = _carRepo.GetQuery(c => c.CarNumber == trip.CarNumber).Include(x => x.CarType).FirstOrDefault();

            var seats = _seatRepo.GetQuery(x => x.CarTypeId == car.CarTypeId).Include(x => x.CarType).AsNoTracking();

            var ticketSeats = _tkSeatRepo.GetQuery().Include(x => x.Ticket)
                                                    .Include(x => x.Seat).AsNoTracking();

            var tickets = _ticketRepo.GetQuery(x => x.TripId == tripId).AsNoTracking();

            var response = new TripDataResponse()
            {
                SeatDiagram = new SeatDiagramResponse
                {
                    Levels = car.CarType.FloorNumber,
                    Columns = car.CarType.ColNumber,
                    Rows = car.CarType.RowNumber,
                    Seats = seats.Where(s => s.CarType.CarTypeCode ==
                                         car.CarType.CarTypeCode)
                             .Select(s => new SeatResponse
                             {
                                 SeatId = s.SeatId,
                                 Columns = s.Col ?? 0,
                                 Rows = s.Row ?? 0,
                                 Levels = s.Floor ?? 0,
                                 Actived = s.Actived ?? false,
                                 SeatNumber = s.SeatNumber,
                                 Status = ticketSeats.FirstOrDefault(t => t.Ticket.TripId == trip.TripId &&
                                                                          t.SeatId == s.SeatId) != null ?
                                          ticketSeats.FirstOrDefault(t => t.Ticket.TripId == trip.TripId &&
                                                                          t.SeatId == s.SeatId).Ticket.Status : s.Status,
                             }).ToList()
                },
                TicketDatas = tickets.Select(x => new TicketData
                {
                    TicketCode = x.TicketCode,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    Price = x.Price,
                    Status = x.Status,
                    CreateTime = x.CreatedTime,
                    Seats = string.Join(',',ticketSeats.Where(t => t.TicketId == x.TicketId)
                                                       .Select(t => t.Seat.SeatNumber).ToList())
                }).OrderByDescending(x => x.CreateTime).ToList()
            };

            return response;
        }

        public async Task<TripDetailResponse> GetTripDetail(Guid tripId)
        {
            var provinces = _provinceRepo.GetQuery().AsNoTracking();

            var districts = _districtRepo.GetQuery()
                                         .Select(x => new
                                         {
                                             Code = x.code,
                                             Name = x.full_name,
                                             ProvinceCode = x.province_code,
                                             Province = provinces.FirstOrDefault(p => p.code == x.province_code).name
                                         });


            var drivers = _eeRepo.GetQuery(x => x.Position == "TAIXE").AsNoTracking();

            var cars = _carRepo.GetQuery().Include(x => x.CarType).AsNoTracking();

            var seats = _seatRepo.GetQuery().Include(x => x.CarType).AsNoTracking();

            var trip = await _tripRepo.FindOneAsync(x => x.TripId == tripId);

            var ticketSeats = _tkSeatRepo.GetQuery().Include(x => x.Ticket)
                                                    .Include(x => x.Seat).AsNoTracking();

            var CarType = cars.FirstOrDefault(c => c.CarNumber == trip.CarNumber).CarType.CarTypeName;

            var routeStartPoint = districts.FirstOrDefault(d => d.Code == trip.StartPoint) != null ? districts.FirstOrDefault(d => d.Code == trip.StartPoint).Province : districts.FirstOrDefault(d => d.ProvinceCode == trip.StartPoint).Province;

            var routeEndPoint = districts.FirstOrDefault(d => d.Code == trip.EndPoint) != null ? districts.FirstOrDefault(d => d.Code == trip.EndPoint).Province : districts.FirstOrDefault(d => d.ProvinceCode == trip.EndPoint).Province;

            var response = new TripDetailResponse()
            {
                TripId = trip.TripId,
                TripCode = trip.TripCode.ToString(),
                CarNumber = trip.CarNumber,
                TicketPrice = trip.TicketPrice ?? 0,
                Description = trip.Description,
                CarType = cars.FirstOrDefault(c => c.CarNumber == trip.CarNumber).CarType.CarTypeName,
                Driver = trip.Driver,
                DriverName = drivers.FirstOrDefault(d => d.EmployeeCode.ToString() == trip.Driver).EmployeeName,
                Route = $"{routeStartPoint} - {routeEndPoint}",
                EndPoint = districts.FirstOrDefault(d => d.Code == trip.EndPoint) != null ?
                                                   $"{districts.FirstOrDefault(d => d.Code == trip.EndPoint).Name} - {districts.FirstOrDefault(d => d.Code == trip.EndPoint).Province}" : districts.FirstOrDefault(d => d.ProvinceCode == trip.EndPoint).Province,
                StartPoint = districts.FirstOrDefault(d => d.Code == trip.StartPoint) != null ?
                                                    $"{districts.FirstOrDefault(d => d.Code == trip.StartPoint).Name} - {districts.FirstOrDefault(d => d.Code == trip.StartPoint).Province}" : districts.FirstOrDefault(d => d.ProvinceCode == trip.StartPoint).Province,
                StartDate = trip.StartDate ?? null,
                CarDetail = new CarDetail
                {
                    Columns = cars.FirstOrDefault(c => c.CarNumber == trip.CarNumber).CarType.ColNumber,
                    Rows = cars.FirstOrDefault(c => c.CarNumber == trip.CarNumber).CarType.RowNumber,
                    Levels = cars.FirstOrDefault(c => c.CarNumber == trip.CarNumber).CarType.FloorNumber,
                },
                Seats = seats.Where(s => s.CarType.CarTypeCode ==
                                         cars.FirstOrDefault(c => c.CarNumber == trip.CarNumber).CarType.CarTypeCode)
                             .Select(s => new SeatResponse
                             {
                                 SeatId = s.SeatId,
                                 Columns = s.Col ?? 0,
                                 Rows = s.Row ?? 0,
                                 Levels = s.Floor ?? 0,
                                 Actived = s.Actived ?? false,
                                 SeatNumber = s.SeatNumber,
                                 Status = ticketSeats.FirstOrDefault(t => t.Ticket.TripId == trip.TripId &&
                                                                          t.SeatId == s.SeatId) != null ?
                                          ticketSeats.FirstOrDefault(t => t.Ticket.TripId == trip.TripId &&
                                                                          t.SeatId == s.SeatId).Ticket.Status : s.Status,
                             }).ToList()
            };

            return response;
        }

        public async Task<PagingResultSP<TripSearchResponse>> SearchTrip(SearchTripRequest request)
        {
            if (!request.StartDate.HasValue)
            {
                request.StartDate = DateTime.Now;
            }

            var startPointProvince = await _districtRepo.FindOneAsync(x => x.code == request.StartPoint);
            var endPointProvince = await _districtRepo.FindOneAsync(x => x.code == request.EndPoint);

            var provinces = _provinceRepo.GetQuery().AsNoTracking();

            var districts = _districtRepo.GetQuery()
                                         .Select(x => new
                                         {
                                             Code = x.code,
                                             Name = x.full_name,
                                             ProvinceCode = x.province_code,
                                             Province = provinces.FirstOrDefault(p => p.code == x.province_code).name
                                         });

            //Get query car
            var cars = _carRepo.GetQuery().Include(x => x.CarType).ThenInclude(x => x.SeatModel).AsNoTracking();

            //Get query ticket
            var tickets = _tkSeatRepo.GetQuery().Include(x => x.Ticket).AsNoTracking();

            //Get query Rate
            var rates = _rateRepo.GetQuery().AsNoTracking();

            var query = _tripRepo.GetQuery().Include(x => x.Route).Include(x => x.CarCompany)
                                 .Where(x => ((x.StartPoint == request.StartPoint || x.Route.StartPoint == request.StartPoint) && 
                                             (x.EndPoint == request.EndPoint || x.Route.EndPoint == request.EndPoint)) &&
                                             (x.StartDate.Value.Date == request.StartDate.Value.Date) &&
                                             (x.TicketPrice >= request.PriceFrom && x.TicketPrice <= request.PriceTo) &&
                                             (request.ListCarCompany.Any() ? request.ListCarCompany.Contains(x.CarCompany.CarCompanyCode) : true) &&
                                             x.StartDate.Value > DateTime.Now)
                                 .Select(x => new TripSearchResponse
                                 {
                                     TripId = x.TripId,
                                     CompanyId = x.CarCompanyId,
                                     CompanyName = x.CarCompany.CarCompanyName,
                                     CarType = cars.FirstOrDefault(c => c.CarNumber == x.CarNumber).CarType.CarTypeName,
                                     EndPoint = districts.FirstOrDefault(d => d.Code == x.EndPoint) != null ?
                                                   $"{districts.FirstOrDefault(d => d.Code == x.EndPoint).Name} - {districts.FirstOrDefault(d => d.Code == x.EndPoint).Province}" : districts.FirstOrDefault(d => d.ProvinceCode == x.EndPoint).Province,
                                     StartPoint = districts.FirstOrDefault(d => d.Code == x.StartPoint) != null ?
                                                    $"{districts.FirstOrDefault(d => d.Code == x.StartPoint).Name} - {districts.FirstOrDefault(d => d.Code == x.StartPoint).Province}" : districts.FirstOrDefault(d => d.ProvinceCode == x.StartPoint).Province,
                                     TicketPrice = x.TicketPrice ?? 0,
                                     StartDate = x.StartDate,
                                     StartTime = x.StartDate.Value.ToString("HH:mm"),
                                     EndTime = "",
                                     SeatTotal = cars.FirstOrDefault(c => c.CarNumber == x.CarNumber).CarType.SeatQuantity ?? 0,
                                     RateCount = rates.Where(r => r.CarCompanyId == x.CarCompanyId).Count(),
                                     RatePoint = rates.Where(r => r.CarCompanyId == x.CarCompanyId).Count() > 0 ?
                                                 Math.Round((decimal)(rates.Where(r => r.CarCompanyId == x.CarCompanyId).Sum(r => r.Rate ?? 0)/ rates.Where(r => r.CarCompanyId == x.CarCompanyId).Count()),1) : 0,
                                     Image = x.CarCompany.Image,
                                     EmptySeat = cars.FirstOrDefault(c => c.CarNumber == x.CarNumber).CarType.SeatQuantity.Value - 
                                                 tickets.Where(t => t.Ticket.TripId == x.TripId).Count() -
                                                 cars.FirstOrDefault(c => c.CarNumber == x.CarNumber).CarType.SeatModel.Where(s => s.Status == "unavailable").Count()
                                 }).OrderBy(x => x.StartDate).ThenBy(x => x.RatePoint);

            var data = query.Where(x => (request.RatePointFrom.HasValue ? x.RatePoint >= request.RatePointFrom : true) &&
                                        (request.EmptySeat.HasValue ? x.EmptySeat >= request.EmptySeat : true));

            #region Phân trang
            var totalRecords = data.Count();

            //Sorting
            var dataSorting = PagingSorting.Sorting(request.Paging, data);
            //Phân trang
            var responsePaginated = PaginatedList<TripSearchResponse>.Create(dataSorting, request.Paging.Offset, request.Paging.PageSize);
            var res = new PagingResultSP<TripSearchResponse>(responsePaginated, totalRecords, request.Paging.PageIndex, request.Paging.PageSize);

            //Đánh số thứ tự
            if (res.Data.Any())
            {
                int i = request.Paging.Offset;
                foreach (var item in res.Data)
                {
                    i++;
                    item.STT = i;
                }
            }
            #endregion

            return await Task.FromResult(res);
        }
    }
}
