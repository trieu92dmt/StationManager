using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Customers
{
    public class DeleteCustomerNSCommand : IRequest<bool>
    {
        public string Customer { get; set; }
    }
    public class DeleteCustomerNSCommandHandler : IRequestHandler<DeleteCustomerNSCommand, bool>
    {
        private readonly IRepository<CustomerModel> _customerRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerNSCommandHandler(IRepository<CustomerModel> customerRep, IUnitOfWork unitOfWork)
        {
            _customerRep = customerRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCustomerNSCommand request, CancellationToken cancellationToken)
        {
            //Xóa customer
            var customer = await _customerRep.FindOneAsync(x => x.CustomerCode == request.Customer);
            if (customer is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Customer {request.Customer}");

            _customerRep.Remove(customer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
