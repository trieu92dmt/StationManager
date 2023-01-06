using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Products
{
    public class DeleteProductNSCommand : IRequest<DeleteNSResponse>
    {
        public List<string> Products { get; set; } = new List<string>();
    }
    public class DeleteProductNSCommandHandler : IRequestHandler<DeleteProductNSCommand, DeleteNSResponse>
    {
        private readonly IRepository<ProductModel> _productRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductNSCommandHandler(IRepository<ProductModel> productRep, IUnitOfWork unitOfWork)
        {
            _productRep = productRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteProductNSCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.Products.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.Products.Count();

            foreach (var productDelete in request.Products)
            {
                try
                {
                    //Xóa Disivision
                    var product = await _productRep.FindOneAsync(x => x.ProductGroupCode == productDelete);
                    if (product is not null)
                    {
                        _productRep.Remove(product);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(productDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(productDelete);
                }

            }
            return response;
        }
    }
}
