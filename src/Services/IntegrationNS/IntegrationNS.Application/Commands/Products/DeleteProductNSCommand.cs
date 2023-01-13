using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Products
{
    public class DeleteProductNSCommand : IRequest<bool>
    {
        public string Product { get; set; }
        public string Plant { get; set; }
    }
    public class DeleteProductNSCommandHandler : IRequestHandler<DeleteProductNSCommand, bool>
    {
        private readonly IRepository<ProductModel> _productRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductNSCommandHandler(IRepository<ProductModel> productRep, IUnitOfWork unitOfWork)
        {
            _productRep = productRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteProductNSCommand request, CancellationToken cancellationToken)
        {
            //Xóa Product
            var product = await _productRep.FindOneAsync(x => x.ProductCode == request.Product && x.PlantCode == request.Plant);
            if (product is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Material {request.Product}");

            _productRep.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
