using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.CarCompany.EmployeeManager
{
    public class UpdateEmployeeCommand : IRequest<bool>
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PositionCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EmployeeModel> _eeRepo;

        public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork, IRepository<EmployeeModel> eeRepo)
        {
            _unitOfWork = unitOfWork;
            _eeRepo = eeRepo;
        }

        public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var ee = await _eeRepo.FindOneAsync(x => x.EmpoyeeId == request.EmployeeId);

            ee.EmployeeName = request.EmployeeName;
            ee.Position = request.PositionCode;
            ee.PhoneNumber = request.PhoneNumber;
            ee.Email = request.Email;
            ee.Description = request.Description;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
