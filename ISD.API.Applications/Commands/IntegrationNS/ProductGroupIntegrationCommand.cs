using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class ProductGroupIntegrationCommand : IRequest<bool>
    {
        public string MaterialGroup { get; set; }
        public string MaterialGroupName { get; set; }
    }
    public class ProductGroupIntegrationCommandHandler : IRequestHandler<ProductGroupIntegrationCommand, bool>
    {
        private readonly IRepository<ProductGroupModel> _productGroupRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public ProductGroupIntegrationCommandHandler(IRepository<ProductGroupModel> productGroupRep, IISDUnitOfWork unitOfWork)
        {
            _productGroupRep = productGroupRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(ProductGroupIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var productGroup = await _productGroupRep.FindOneAsync(x => x.ProductGroupCode == request.MaterialGroup);

            if (productGroup is null)
            {
                _productGroupRep.Add(new ProductGroupModel
                {
                    ProductGroupId = Guid.NewGuid(),
                    ProductGroupCode = request.MaterialGroup,
                    ProductGroupName = request.MaterialGroupName,             

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                productGroup.ProductGroupName = request.MaterialGroupName;
                
                //Common
                productGroup.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
