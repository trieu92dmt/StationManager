using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.SalesOrgs
{
    public class DeleteSaleOrgCommand : IRequest<DeleteNSResponse>
    {
        public List<string> SaleOrgs { get; set; } = new List<string>();
    }

    public class DeleteSaleOrgCommandHandler : IRequestHandler<DeleteSaleOrgCommand, DeleteNSResponse>
    {
        private readonly IRepository<SaleOrgModel> _saleOrgRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSaleOrgCommandHandler(IRepository<SaleOrgModel> saleOrgRep, IUnitOfWork unitOfWork)
        {
            _saleOrgRep = saleOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteSaleOrgCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.SaleOrgs.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.SaleOrgs.Count();

            foreach (var saleOrgDelete in request.SaleOrgs)
            {
                try
                {
                    //Xóa Sale Org
                    var saleOrg = await _saleOrgRep.FindOneAsync(x => x.SaleOrgCode == saleOrgDelete);
                    if (saleOrg is not null)
                    {
                        _saleOrgRep.Remove(saleOrg);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(saleOrgDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(saleOrgDelete);
                }

            }
            return response;
        }
    }
}
