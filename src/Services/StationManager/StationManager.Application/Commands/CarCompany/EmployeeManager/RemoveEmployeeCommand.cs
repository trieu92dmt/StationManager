using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace StationManager.Application.Commands.CarCompany.EmployeeManager
{
    public class RemoveEmployeeCommand : IRequest<bool>
    {
        public Guid EmployeeId { get; set; }
    }

    public class RemoveEmployeeCommandHandler : IRequestHandler<RemoveEmployeeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EmployeeModel> _eeRepo;

        public RemoveEmployeeCommandHandler(IUnitOfWork unitOfWork, IRepository<EmployeeModel> eeRepo)
        {
            _unitOfWork = unitOfWork;
            _eeRepo = eeRepo;
        }

        public async Task<bool> Handle(RemoveEmployeeCommand request, CancellationToken cancellationToken)
        {
            //Get ee
            var ee = await _eeRepo.FindOneAsync(x => x.EmpoyeeId == request.EmployeeId);

            _eeRepo.Remove(ee);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
