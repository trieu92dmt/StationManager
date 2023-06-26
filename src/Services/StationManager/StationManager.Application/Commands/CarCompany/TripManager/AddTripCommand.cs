using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.CarCompany.TripManager
{
    public class AddTripCommand : IRequest<bool>
    {
        public Guid RouteId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTime StartDate { get; set; }
        public string CarNumber { get; set; }
        public string Driver { get; set; }
        public string Description { get; set; }
        public decimal TicketPrice { get; set; }
    }

    public class AddTripCommandHandler : IRequestHandler<AddTripCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TripModel> _tripRepo;
        private readonly IRepository<RouteModel> _routeRepo;

        public AddTripCommandHandler(IUnitOfWork unitOfWork, IRepository<TripModel> tripRepo, IRepository<RouteModel> routeRepo)
        {
            _unitOfWork = unitOfWork;
            _tripRepo = tripRepo;
            _routeRepo = routeRepo;
        }

        public async Task<bool> Handle(AddTripCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeRepo.FindOneAsync(x => x.RouteId == request.RouteId);

            var trip = new TripModel
            {
                TripId = Guid.NewGuid(),
                Actived = true,
                CarNumber = request.CarNumber,
                CreateTime = DateTime.Now,
                Description = request.Description,
                Driver = request.Driver,
                TicketPrice = request.TicketPrice,
                EndPoint = request.EndPoint,
                StartPoint = request.StartPoint,
                StartDate = request.StartDate,
                RouteId = request.RouteId,
                CarCompanyId = route.CarCompanyId
            };

            _tripRepo.Add(trip);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
