using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.CustmdSales
{
    public class DeleteCustmdSaleCommand : IRequest<DeleteNSResponse>
    {
        public List<string> CustmdSales { get; set; } = new List<string>();
    }
    public class DeleteCustmdSaleCommandHandler : IRequestHandler<DeleteCustmdSaleCommand, DeleteNSResponse>
    {
        private readonly IRepository<CustmdSaleModel> _custmdSaleRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustmdSaleCommandHandler(IRepository<CustmdSaleModel> custmdSaleRep, IUnitOfWork unitOfWork)
        {
            _custmdSaleRep = custmdSaleRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteCustmdSaleCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.CustmdSales.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.CustmdSales.Count();

            foreach (var custmdSaleDelete in request.CustmdSales)
            {
                try
                {
                    //Xóa CustmdSales
                    var custmdSale = await _custmdSaleRep.FindOneAsync(x => x.SalesOffice == custmdSaleDelete);
                    if (custmdSale is not null)
                    {
                        _custmdSaleRep.Remove(custmdSale);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(custmdSaleDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(custmdSaleDelete);
                }

            }
            return response;
        }
    }
}
