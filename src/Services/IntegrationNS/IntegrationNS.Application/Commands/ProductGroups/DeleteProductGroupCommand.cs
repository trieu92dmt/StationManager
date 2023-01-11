using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.ProductGroups
{
    public class DeleteProductGroupCommand : IRequest<bool>
    {
        public string ProductGroup { get; set; }
    }

    public class DeleteProductGroupCommandHandler : IRequestHandler<DeleteProductGroupCommand, bool>
    {
        private readonly IRepository<ProductGroupModel> _productGroupRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductGroupCommandHandler(IRepository<ProductGroupModel> productGroupRep, IUnitOfWork unitOfWork)
        {
            _productGroupRep = productGroupRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteProductGroupCommand request, CancellationToken cancellationToken)
        {
            //Xóa ProductGroup
            var productGroup = await _productGroupRep.FindOneAsync(x => x.ProductGroupCode == request.ProductGroup);
            if (productGroup is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Material Group {request.ProductGroup}");

            _productGroupRep.Remove(productGroup);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
