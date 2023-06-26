using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.Admin
{
    public class ChangeStatusCarCompanyCommand : IRequest<bool>
    {
        public Guid CarCompanyId { get; set; }
        public string Status { get; set; }
    }

    public class ChangeStatusCarCompanyCommandHandler : IRequestHandler<ChangeStatusCarCompanyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<AccountModel> _accRepo;
        private readonly IRepository<CarCompany_Package_MappingModel> _mappingRepo;
        public ChangeStatusCarCompanyCommandHandler(IUnitOfWork unitOfWork, IRepository<CarCompanyModel> carCompanyRepo,
                                                     IRepository<AccountModel> accRepo,
                                                     IRepository<CarCompany_Package_MappingModel> mappingRepo)
        {
            _unitOfWork = unitOfWork;
            _carCompanyRepo = carCompanyRepo;
            _accRepo = accRepo;
            _mappingRepo = mappingRepo;
        }

        public async Task<bool> Handle(ChangeStatusCarCompanyCommand request, CancellationToken cancellationToken)
        {
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.CarCompanyId == request.CarCompanyId);

            carCompany.Actived = request.Status == "lock" ? false : true;
            carCompany.Status = request.Status;

            var account = await _accRepo.FindOneAsync(x => x.AccountId == carCompany.AccountId);

            account.Actived = request.Status == "lock" ? false : true;

            var mapping = await _mappingRepo.FindOneAsync(x => x.CarCompanyId == carCompany.CarCompanyId);

            mapping.Actived = request.Status == "lock" ? false : true;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
