using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.CarCompany.CarManager
{
    public class RemoveCarCommand : IRequest<bool>
    {
        public Guid CarId { get; set; }
    }

    public class RemoveCarCommandHandler : IRequestHandler<RemoveCarCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarModel> _carRepo;

        public RemoveCarCommandHandler(IUnitOfWork unitOfWork, IRepository<CarModel> carRepo)
        {
            _unitOfWork = unitOfWork;
            _carRepo = carRepo;
        }

        public async Task<bool> Handle(RemoveCarCommand request, CancellationToken cancellationToken)
        {
            //Get car
            var car = await _carRepo.FindOneAsync(x => x.CarId == request.CarId);

            _carRepo.Remove(car);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
