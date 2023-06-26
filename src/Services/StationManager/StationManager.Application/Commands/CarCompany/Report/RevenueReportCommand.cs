using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;

namespace StationManager.Application.Commands.CarCompany.Report
{
    public class RevenueReportCommand : IRequest<List<RevenueReportResponse>>
    {
        public Guid AccountId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Type { get; set; } //DAY/MONTH/YEAR
    }

    public class RevenueReportCommandHandler : IRequestHandler<RevenueReportCommand, List<RevenueReportResponse>>
    {
        private readonly IRepository<TicketModel> _ticketRepo;

        public RevenueReportCommandHandler(IRepository<TicketModel> ticketRepo)
        {
            _ticketRepo = ticketRepo;
        }

        public async Task<List<RevenueReportResponse>> Handle(RevenueReportCommand request, CancellationToken cancellationToken)
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

            //Ticket Query
            var tickets = await _ticketRepo.GetQuery().Include(x => x.Trip).ToListAsync();

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
                                         Value = x.Price
                                     }).ToList();
            var report = new List<RevenueReportResponse>();
            if (request.Type == "DAY")
            {
                report = data.GroupBy(x => x.Day)
                                 .Select(x => new RevenueReportResponse
                                 {
                                     Label = x.Key,
                                     Value = x.Sum(x => x.Value)
                                 }).ToList();
            }
            else if (request.Type == "MONTH")
            {
                report = data.GroupBy(x => x.Month)
                                 .Select(x => new RevenueReportResponse
                                 {
                                     Label = x.Key,
                                     Value = x.Sum(x => x.Value)
                                 }).ToList();
            }
            else if (request.Type == "YEAR")
            {
                report = data.GroupBy(x => x.Year)
                                 .Select(x => new RevenueReportResponse
                                 {
                                     Label = x.Key,
                                     Value = x.Sum(x => x.Value)
                                 }).ToList();
            }

            return report;
        }
    }
}
