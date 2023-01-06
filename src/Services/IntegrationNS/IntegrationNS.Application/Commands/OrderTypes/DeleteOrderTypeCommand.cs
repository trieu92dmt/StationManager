using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.OrderTypes
{
    public class DeleteOrderTypeCommand : IRequest<DeleteNSResponse>
    {
        public List<string> OrderTypes { get; set; } = new List<string>();

    }
    public class DeleteOrderTypeCommandHandler : IRequestHandler<DeleteOrderTypeCommand, DeleteNSResponse>
    {
        private readonly IRepository<OrderTypeModel> _orderTypeRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderTypeCommandHandler(IRepository<OrderTypeModel> orderTypeRep, IUnitOfWork unitOfWork)
        {
            _orderTypeRep = orderTypeRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteOrderTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.OrderTypes.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.OrderTypes.Count();

            foreach (var orderTypeDelete in request.OrderTypes)
            {
                try
                {
                    //Xóa Disivision
                    var orderType = await _orderTypeRep.FindOneAsync(x => x.OrderTypeCode == orderTypeDelete);
                    if (orderType is not null)
                    {
                        _orderTypeRep.Remove(orderType);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                        response.ListRecordDeleteFailed.Add(orderTypeDelete);

                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(orderTypeDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                }

            }
            return response;
        }
    }
}
