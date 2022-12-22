using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class CustomerIntegrationNSCommand : IRequest<bool>
    {
        public string Customer { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }

    public class CustomerIntegrationNSCommandHandler : IRequestHandler<CustomerIntegrationNSCommand, bool>
    {
        private readonly IRepository<CustomerModel> _customerRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public CustomerIntegrationNSCommandHandler(IRepository<CustomerModel> customerRep, IISDUnitOfWork unitOfWork)
        {
            _customerRep = customerRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CustomerIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var vendor = await _customerRep.FindOneAsync(x => x.CustomerCode == request.Customer);

            if (vendor is null)
            {
                _customerRep.Add(new CustomerModel
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerCode = request.Customer,
                    CustomerName = request.Name,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                vendor.CustomerName = request.Name;

                //Common
                vendor.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
