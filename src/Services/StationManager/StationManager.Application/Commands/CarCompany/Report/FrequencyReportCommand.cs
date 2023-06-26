using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;

namespace StationManager.Application.Commands.CarCompany.Report
{
    public class FrequencyReportCommand : IRequest<List<FrequencyReportResponse>>
    {
        public Guid AccountId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class FrequencyReportCommandHandler : IRequestHandler<FrequencyReportCommand, List<FrequencyReportResponse>>
    {
        private readonly IRepository<TicketModel> _ticketRepo;
        private readonly IRepository<provinces> _provinceRepo;

        public FrequencyReportCommandHandler(IRepository<TicketModel> ticketRepo, IRepository<provinces> provinceRepo)
        {
            _ticketRepo = ticketRepo;
            _provinceRepo = provinceRepo;
        }

        public async Task<List<FrequencyReportResponse>> Handle(FrequencyReportCommand request, CancellationToken cancellationToken)
        {
            var crrYear = DateTime.Now.Year;
            if (!request.DateFrom.HasValue)
            {
                request.DateFrom = new DateTime(crrYear, 1, 1);
            }
            if (!request.DateTo.HasValue)
            {
                request.DateTo = new DateTime(crrYear, 12, 31);
            }

            //Province Query
            var provinces = _provinceRepo.GetQuery().AsNoTracking();

            //Ticket Query
            var tickets = await _ticketRepo.GetQuery().Include(x => x.Trip).ThenInclude(x => x.Route).ToListAsync();

            var data = tickets.Where(x => (!string.IsNullOrEmpty(request.StartPoint) ? x.Trip.StartPoint == request.StartPoint : true) &&
                                                 (!string.IsNullOrEmpty(request.EndPoint) ? x.Trip.EndPoint == request.EndPoint : true) &&
                                                 (request.DateFrom.HasValue ? x.Trip.StartDate.Value.Date >= request.DateFrom.Value.Date : true) &&
                                                 (request.DateTo.HasValue ? x.Trip.StartDate.Value.Date <= request.DateTo.Value.Date : true))
                                     .Select(x => new
                                     {
                                         Date = x.Trip.StartDate,
                                         Day = x.Trip.StartDate.Value.ToString("dd/MM/yyyy"),
                                         Month = x.Trip.StartDate.Value.ToString("MM/yyyy"),
                                         Year = x.Trip.StartDate.Value.Year.ToString(),
                                         Route = $"{provinces.FirstOrDefault(p => p.code == x.Trip.Route.StartPoint).name}-{provinces.FirstOrDefault(p => p.code == x.Trip.Route.EndPoint).name}", 
                                         Value = x.Price
                                     }).ToList();
            var report = new List<FrequencyReportResponse>();

            report = data.GroupBy(x => x.Route)
                                 .Select(x => new FrequencyReportResponse
                                 {
                                     Label = x.Key,
                                     Value = x.Count()
                                 }).ToList();


            return report;
        }
    }
}
