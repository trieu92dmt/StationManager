using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.OrderTypes
{
    public class OrderTypeIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<OrderTypeIntegration> OrderTypes { get; set; } = new List<OrderTypeIntegration>();
    }
    public class OrderTypeIntegration
    {
        public string PlanningPlant { get; set; }
        public string Name { get; set; }
        public string OrderType { get; set; }
        public string ShortText { get; set; }
    }
    public class OrderTypeIntegrationCommandHandler : IRequestHandler<OrderTypeIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<OrderTypeModel> _orderTypeRep;
        private readonly IUnitOfWork _unitOfWork;

        public OrderTypeIntegrationCommandHandler(IRepository<OrderTypeModel> orderTypeRep, IUnitOfWork unitOfWork)
        {
            _orderTypeRep = orderTypeRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(OrderTypeIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.OrderTypes.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.OrderTypes.Count();

            foreach (var orderTypeIntegration in request.OrderTypes)
            {
                try
                {
                    //Check tồn tại
                    var orderType = await _orderTypeRep.FindOneAsync(x => x.OrderTypeCode == orderTypeIntegration.OrderType);

                    //Chưa tồn tại tạo mới
                    if (orderType is null)
                    {
                        _orderTypeRep.Add(new OrderTypeModel
                        {
                            OrderTypeId = Guid.NewGuid(),
                            PlanningPlant = orderTypeIntegration.PlanningPlant,
                            OrderTypeCode = orderTypeIntegration.OrderType,
                            OrderTypeName = orderTypeIntegration.Name,
                            ShortText = orderTypeIntegration.ShortText,
                            CreateTime = DateTime.Now,
                            Actived = true,
                        });
                    }
                    else
                    {
                        orderType.PlanningPlant = orderTypeIntegration.PlanningPlant;
                        orderType.OrderTypeName = orderTypeIntegration.Name;
                        orderType.ShortText = orderTypeIntegration.ShortText;
                        orderType.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{orderTypeIntegration.OrderType}");
                }
            }

            return response;
        }
    }
}
