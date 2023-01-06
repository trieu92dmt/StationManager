using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Products
{
    public class ProductIntegrationNSCommand : IRequest<IntegrationNSResponse>
    {
        public List<ProductIntegration> Products { get; set; } = new List<ProductIntegration>();
    }
    public class ProductIntegration
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

    public class ProductIntegrationNSCommandHandler : IRequestHandler<ProductIntegrationNSCommand, IntegrationNSResponse>
    {
        private readonly IRepository<ProductModel> _productRep;
        private readonly IUnitOfWork _unitOfWork;

        public ProductIntegrationNSCommandHandler(IRepository<ProductModel> productRep, IUnitOfWork unitOfWork)
        {
            _productRep = productRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(ProductIntegrationNSCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.Products.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.Products.Count();

            foreach (var productGroupIntegration in request.Products)
            {
                try
                {
                    //Check tồn tại
                    var product = await _productRep.FindOneAsync(x => x.ProductCode == productGroupIntegration.Material);

                    if (product is null)
                    {
                        _productRep.Add(new ProductModel
                        {
                            ProductId = Guid.NewGuid(),
                            PlantCode = productGroupIntegration.Plant,
                            ProductCode = productGroupIntegration.Material,
                            //ProductCodeInt = int.Parse(request.Material),
                            ProductName = productGroupIntegration.MaterialDescription,
                            ProductGroupCode = productGroupIntegration.MaterialGroup,
                            ProductGroupDesc = productGroupIntegration.MaterialGroupDesc,
                            ProductTypeCode = productGroupIntegration.MaterialType,
                            ProductTypeName = productGroupIntegration.MaterialTypeDescription,
                            Unit = productGroupIntegration.BaseUnitofMeasure,
                            DivisionCode = productGroupIntegration.Division,
                            SaleOrgCode = productGroupIntegration.SalesOrganization,
                            DistributionChannelCode = productGroupIntegration.DistributionChannel,

                            //Common
                            CreateTime = DateTime.Now,
                            Activce = true
                        });
                    }
                    else
                    {
                        product.PlantCode = productGroupIntegration.Plant;
                        product.ProductName = productGroupIntegration.MaterialDescription;
                        product.ProductGroupCode = productGroupIntegration.MaterialGroup;
                        product.ProductGroupDesc = productGroupIntegration.MaterialGroupDesc;
                        product.ProductTypeCode = productGroupIntegration.MaterialType;
                        product.ProductTypeName = productGroupIntegration.MaterialTypeDescription;
                        product.Unit = productGroupIntegration.BaseUnitofMeasure;
                        product.DivisionCode = productGroupIntegration.Division;
                        product.SaleOrgCode = productGroupIntegration.SalesOrganization;
                        product.DistributionChannelCode = productGroupIntegration.DistributionChannel;

                        //Common
                        product.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{productGroupIntegration.MaterialGroup}");
                }
            }

            return response;
        }
    }
}
