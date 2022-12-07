using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class OrderTypeIntegrationCommand : IRequest<bool>
    {
        public string PlanningPlant { get; set; }
        public string Name { get; set; }
        public string OrderType { get; set; }
        public string ShortText { get; set; }
    }

    public class OrderTypeIntegrationCommandHandler : IRequestHandler<OrderTypeIntegrationCommand, bool>
    {
        private readonly IGeneRepo<OrderTypeModel> _orderTypeRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public OrderTypeIntegrationCommandHandler(IGeneRepo<OrderTypeModel> orderTypeRep, IISDUnitOfWork unitOfWork)
        {
            _orderTypeRep = orderTypeRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(OrderTypeIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var orderType = await _orderTypeRep.FindOneAsync(x => x.OrderTypeCode == request.OrderType);

            //Chưa tồn tại tạo mới
            if (orderType is null)
            {
                _orderTypeRep.Add(new OrderTypeModel
                {
                    OrderTypeId = Guid.NewGuid(),
                    PlanningPlant = request.PlanningPlant,
                    OrderTypeCode = request.OrderType,
                    OrderTypeName = request.Name,
                    ShortText = request.ShortText,
                    CreateTime = DateTime.Now,
                    Actived = true,
                });
            }
            else
            {
                orderType.PlanningPlant = request.PlanningPlant;
                orderType.OrderTypeName = request.Name;
                orderType.ShortText = request.ShortText;
                orderType.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
