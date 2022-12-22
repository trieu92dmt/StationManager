using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class ProductIntegrationNSCommand : IRequest<bool>
    {
        public string Plant { get; set; }
        public string PlantDescription { get; set; }
        public string Material { get; set; }
        public string MaterialDescription { get; set; }
        public string MaterialGroup { get; set; }
        public string MaterialGroupDesc { get; set; }
        public string MaterialType { get; set; }
        public string MaterialTypeDescription { get; set; }
        public string BaseUnitofMeasure { get; set; }
        public string Division { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
    }
    public class ProductIntegrationNSCommandHandler : IRequestHandler<ProductIntegrationNSCommand, bool>
    {
        private readonly IRepository<ProductModel> _productRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public ProductIntegrationNSCommandHandler(IRepository<ProductModel> productRep, IISDUnitOfWork unitOfWork)
        {
            _productRep = productRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(ProductIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var product = await _productRep.FindOneAsync(x => x.ProductCode == request.Material);

            if (product is null)
            {
                _productRep.Add(new ProductModel
                {
                    ProductId = Guid.NewGuid(),
                    PlantCode = request.Plant,
                    ProductCode = request.Material,
                    ProductName = request.MaterialDescription,
                    ProductGroupCode = request.MaterialGroup,
                    ProductGroupDesc = request.MaterialGroupDesc,
                    ProductTypeCode = request.MaterialType,
                    ProductTypeName = request.MaterialTypeDescription,
                    Unit = request.BaseUnitofMeasure,
                    DivisionCode = request.Division,
                    SaleOrgCode = request.SalesOrganization,
                    DistributionChannelCode = request.DistributionChannel,

                    //Common
                    CreateTime = DateTime.Now,
                    Activce = true
                });
            }
            else
            {
                product.PlantCode = request.Plant;
                product.ProductName = request.MaterialDescription;
                product.ProductGroupCode = request.MaterialGroup;
                product.ProductGroupDesc = request.MaterialGroupDesc;
                product.ProductTypeCode = request.MaterialType;
                product.ProductTypeName = request.MaterialTypeDescription;
                product.Unit = request.BaseUnitofMeasure;
                product.DivisionCode = request.Division;
                product.SaleOrgCode = request.SalesOrganization;
                product.DistributionChannelCode = request.DistributionChannel;

                //Common
                product.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
