using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IRepository<DetailWorkOrderModel> _detailWORepo;

        public DeleteWorkOrderCommandHandler(IRepository<WorkOrderModel> woRepo, IUnitOfWork unitOfWork, IRepository<DetailWorkOrderModel> detailWORepo)
        {
            _unitOfWork = unitOfWork;
            _detailWORepo = detailWORepo;
            _woRepo = woRepo;
        }

        public async Task<bool> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
        {
            //Xóa WorkOrder
            var workOrder = await _woRepo.GetQuery(x => x.WorkOrderCode == request.WorkOrderCode).Include(x => x.DetailWorkOrderModel).FirstOrDefaultAsync();
            if (workOrder is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"WorkOrder {request.WorkOrderCode}");

            //Xóa WO Detail
            _detailWORepo.RemoveRange(workOrder.DetailWorkOrderModel);

            _woRepo.Remove(workOrder);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
