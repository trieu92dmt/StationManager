using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.ProductGroups
{
    public class DeleteProductGroupCommand : IRequest<DeleteNSResponse>
    {
        public List<string> ProductGroups { get; set; } = new List<string>();
    }

    public class DeleteProductGroupCommandHandler : IRequestHandler<DeleteProductGroupCommand, DeleteNSResponse>
    {
        private readonly IRepository<ProductGroupModel> _productGroupRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductGroupCommandHandler(IRepository<ProductGroupModel> productGroupRep, IUnitOfWork unitOfWork)
        {
            _productGroupRep = productGroupRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteProductGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.ProductGroups.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.ProductGroups.Count();

            foreach (var productGroupDelete in request.ProductGroups)
            {
                try
                {
                    //Xóa Disivision
                    var productGroup = await _productGroupRep.FindOneAsync(x => x.ProductGroupCode == productGroupDelete);
                    if (productGroup is not null)
                    {
                        _productGroupRep.Remove(productGroup);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(productGroupDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(productGroupDelete);
                }

            }
            return response;
        }
    }
}
