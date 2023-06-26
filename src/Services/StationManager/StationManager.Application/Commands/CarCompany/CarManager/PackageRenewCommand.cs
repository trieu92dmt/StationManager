using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.CarCompany.CarManager
{
    public class PackageRenewCommand : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public string PackageCode { get; set; }
    }

    public class PackageRenewCommandHandler : IRequestHandler<PackageRenewCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarCompany_Package_MappingModel> _mappingRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<PackageModel> _packageRepo;

        public PackageRenewCommandHandler(IUnitOfWork unitOfWork, IRepository<CarCompany_Package_MappingModel> mappingRepo,
                                          IRepository<CarCompanyModel> carCompanyRepo, IRepository<PackageModel> packageRepo)
        {
            _unitOfWork = unitOfWork;
            _mappingRepo = mappingRepo;
            _carCompanyRepo = carCompanyRepo;
            _packageRepo = packageRepo;
        }

        public async Task<bool> Handle(PackageRenewCommand request, CancellationToken cancellationToken)
        {
            //Get car company
            var company = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            //Get Package
            var package = await _packageRepo.FindOneAsync(x => x.PackageCode == request.PackageCode);

            //Create Mapping
            var newMapping = new CarCompany_Package_MappingModel
            {
                CarCompany_Package_MappingId = Guid.NewGuid(),
                CarCompanyId = company.CarCompanyId,
                PackageId = package.PackageId,
                CreatedTime = DateTime.Now,
                Actived = true,
                ExpireTime = DateTime.Now.AddMonths(package.Duration ?? 0),
            };

            _mappingRepo.Add(newMapping);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
