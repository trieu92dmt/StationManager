using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.CarCompany.CarManager
{
    public class AddCarCommand : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public string CarNumber { get; set; }
        public string CarType { get; set; }  
        public string Description { get; set; }
    }

    public class AddCarCommandHandler : IRequestHandler<AddCarCommand, bool>
    {
        private readonly IRepository<CarModel> _carRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<CarTypeModel> _carTypeRepo;

        public AddCarCommandHandler(IRepository<CarModel> carRepo, IUnitOfWork unitOfWork, IRepository<CarCompanyModel> carCompanyRepo,
                                    IRepository<CarTypeModel> carTypeRepo)
        {
            _carRepo = carRepo;
            _unitOfWork = unitOfWork;
            _carCompanyRepo = carCompanyRepo;
            _carTypeRepo = carTypeRepo;
        }

        public async Task<bool> Handle(AddCarCommand request, CancellationToken cancellationToken)
        {
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            var carType = await _carTypeRepo.FindOneAsync(x => x.CarTypeCode == request.CarType);

            _carRepo.Add(new CarModel
            {
                CarId = Guid.NewGuid(),
                CarTypeId = carType.CarTypeId,
                CarNumber = request.CarNumber,
                CarCompanyId = carCompany.CarCompanyId,
                CreateTime = DateTime.Now,
                Description = request.Description,
                Actived = true,
            });

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
