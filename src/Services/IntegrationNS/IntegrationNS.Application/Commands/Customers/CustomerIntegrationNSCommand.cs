using IntegrationNS.Application.DTOs;
using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.Customers
{
    public class CustomerIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<CustomerIntegration> Customers { get; set; } = new List<CustomerIntegration>();
    }

    public class CustomerIntegration
    {
        public string Customer { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
    public class CustomerIntegrationNSCommandHandler : IRequestHandler<CustomerIntegrationNSCommand, IntegrationNSResponse>
    {
        private readonly IRepository<CustomerModel> _customerRep;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerIntegrationNSCommandHandler(IRepository<CustomerModel> customerRep, IUnitOfWork unitOfWork)
        {
            _customerRep = customerRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(CustomerIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.Customers.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.Customers.Count();

            var customers = _customerRep.GetQuery();

            foreach (var customerIntegration in request.Customers)
            {
                try
                {
                    //Check tồn tại
                    var customer = await customers.FirstOrDefaultAsync(x => x.CustomerCode == customerIntegration.Customer);

                    if (customer is null)
                    {
                        _customerRep.Add(new CustomerModel
                        {
                            CustomerId = Guid.NewGuid(),
                            CustomerCode = customerIntegration.Customer,
                            CustomerName = customerIntegration.Name,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        customer.CustomerName = customerIntegration.Name;

                        //Common
                        customer.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = customerIntegration.Customer,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
