using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.CarCompany.EmployeeManager
{
    public class AddEmployeeCommand : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public string EmployeeName { get; set; }
        public string PositionCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }

    public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EmployeeModel> _eeRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;

        public AddEmployeeCommandHandler(IUnitOfWork unitOfWork, IRepository<EmployeeModel> eeRepo,
                                         IRepository<CarCompanyModel> carCompanyRepo)
        {
            _unitOfWork = unitOfWork;
            _eeRepo = eeRepo;
            _carCompanyRepo = carCompanyRepo;
        }

        public async Task<bool> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            //Get car company
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            //Tạo mới
            var ee = new EmployeeModel
            {
                EmpoyeeId = Guid.NewGuid(),
                EmployeeName = request.EmployeeName,
                Actived = true,
                CarCompanyId = carCompany.CarCompanyId,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Description = request.Description,
                Position = request.PositionCode
            };

            _eeRepo.Add(ee);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
