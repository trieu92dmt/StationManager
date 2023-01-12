using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.CustmdSales
{
    public class DeleteCustmdSaleCommand : IRequest<bool>
    {
        public string CustomerNumber { get; set; }
        public string DivisionCode { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
    }
    public class DeleteCustmdSaleCommandHandler : IRequestHandler<DeleteCustmdSaleCommand, bool>
    {
        private readonly IRepository<CustmdSaleModel> _custmdSaleRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustmdSaleCommandHandler(IRepository<CustmdSaleModel> custmdSaleRep, IUnitOfWork unitOfWork)
        {
            _custmdSaleRep = custmdSaleRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteCustmdSaleCommand request, CancellationToken cancellationToken)
        {
            //Xóa CustmdSales
            var custmdSale = await _custmdSaleRep.FindOneAsync(x => x.CustomerNumber == request.CustomerNumber &&
                                                                    x.SaleOrgCode == request.SalesOrganization &&
                                                                    x.DistributionChannelCode == request.DistributionChannel &&
                                                                    x.DivisionCode == request.DivisionCode);
            if (custmdSale is null)
                throw new ISDException(CommonResource.Msg_NotFound, "CustmdSales");


            _custmdSaleRep.Remove(custmdSale);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
