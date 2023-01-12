using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.WorkOrder
{
    public class DeleteWorkOrderCommand : IRequest<bool>
    {
        public string WorkOrderCode { get; set; }
    }
    public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand, bool>
    {
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWorkOrderCommandHandler(IRepository<WorkOrderModel> woRepo, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _woRepo = woRepo;
        }

        public async Task<bool> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
        {
            //Xóa WorkOrder
            var workOrder = await _woRepo.FindOneAsync(x => x.WorkOrderCode == request.WorkOrderCode);
            if (workOrder is not null)
                throw new ISDException(CommonResource.Msg_NotFound, $"WorkOrder {request.WorkOrderCode}");

            _woRepo.Remove(workOrder);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
