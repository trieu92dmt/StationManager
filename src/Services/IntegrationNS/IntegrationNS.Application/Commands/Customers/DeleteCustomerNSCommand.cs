using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Customers
{
    public class DeleteCustomerNSCommand : IRequest<DeleteNSResponse>
    {
        public List<string> Customers { get; set; } = new List<string>();
    }
    public class DeleteCustomerNSCommandHandler : IRequestHandler<DeleteCustomerNSCommand, DeleteNSResponse>
    {
        private readonly IRepository<CustomerModel> _customerRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerNSCommandHandler(IRepository<CustomerModel> customerRep, IUnitOfWork unitOfWork)
        {
            _customerRep = customerRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteNSResponse> Handle(DeleteCustomerNSCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.Customers.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.Customers.Count();

            foreach (var customerDelete in request.Customers)
            {
                try
                {
                    //Xóa Disivision
                    var customer = await _customerRep.FindOneAsync(x => x.CustomerCode == customerDelete);
                    if (customer is not null)
                    {
                        _customerRep.Remove(customer);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
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
