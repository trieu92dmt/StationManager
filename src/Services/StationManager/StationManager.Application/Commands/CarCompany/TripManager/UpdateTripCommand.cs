using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.CarCompany.TripManager
{
    public class UpdateTripCommand : IRequest<bool>
    {
        public Guid TripId { get; set; }
        public string CarNumber { get; set; }
        public string Driver { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class UpdateTripCommandHandler : IRequestHandler<UpdateTripCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TripModel> _tripRepo;

        public UpdateTripCommandHandler(IUnitOfWork unitOfWork, IRepository<TripModel> tripRepo)
        {
            _unitOfWork = unitOfWork;
            _tripRepo = tripRepo;
        }

        public async Task<bool> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepo.FindOneAsync(x => x.TripId == request.TripId);

            trip.CarNumber = request.CarNumber;
            trip.Driver = request.Driver;
            trip.Description = request.Description;
            trip.StartDate = request.StartDate;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
