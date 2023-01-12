using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.OrderTypes
{
    public class DeleteOrderTypeCommand : IRequest<bool>
    {
        public string Category { get; set; }
        public string OrderType { get; set; }
        public string Plant { get; set; }

    }
    public class DeleteOrderTypeCommandHandler : IRequestHandler<DeleteOrderTypeCommand, bool>
    {
        private readonly IRepository<OrderTypeModel> _orderTypeRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderTypeCommandHandler(IRepository<OrderTypeModel> orderTypeRep, IUnitOfWork unitOfWork)
        {
            _orderTypeRep = orderTypeRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteOrderTypeCommand request, CancellationToken cancellationToken)
        {
            //Xóa Order Type
            var orderType = await _orderTypeRep.FindOneAsync(x => x.OrderTypeCode == request.OrderType &&
                                                                  x.Plant == request.Plant);
            if (orderType is null)
                throw new ISDException(CommonResource.Msg_NotFound, "Order Type");

            _orderTypeRep.Remove(orderType);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
