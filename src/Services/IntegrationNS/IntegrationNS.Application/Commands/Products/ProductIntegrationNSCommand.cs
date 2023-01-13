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
        public string Material { get; set; }
        public string MaterialDescription { get; set; }
        public string MaterialGroup { get; set; }
        public string MaterialGroupDesc { get; set; }
        public string MaterialType { get; set; }
        public string MaterialTypeDescription { get; set; }
        public string BaseUnitofMeasure { get; set; }
        public string Status { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public string OldMaterialNum { get; set; }
        public string WeightUnit { get; set; }
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
                    var product = await _productRep.FindOneAsync(x => x.ProductCode == productGroupIntegration.Material && x.PlantCode == productGroupIntegration.Plant);

                    if (product is null)
                    {
                        _productRep.Add(new ProductModel
                        {
                            ProductId = Guid.NewGuid(),
                            PlantCode = productGroupIntegration.Plant,
                            ProductCode = productGroupIntegration.Material,
                            ProductCodeInt = int.Parse(productGroupIntegration.Material),
                            ProductName = productGroupIntegration.MaterialDescription,
                            ProductGroupCode = productGroupIntegration.MaterialGroup,
                            ProductGroupDesc = productGroupIntegration.MaterialGroupDesc,
                            ProductTypeCode = productGroupIntegration.MaterialType,
                            ProductTypeName = productGroupIntegration.MaterialTypeDescription,
                            Unit = productGroupIntegration.BaseUnitofMeasure,
                            Status = productGroupIntegration.Status,
                            GrossWeight = productGroupIntegration.GrossWeight,
                            NetWeight = productGroupIntegration.NetWeight,
                            OldMaterialNum = productGroupIntegration.OldMaterialNum,
                            WeightUnit = productGroupIntegration.WeightUnit,

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
                        product.Status = productGroupIntegration.Status;
                        product.GrossWeight = productGroupIntegration.GrossWeight;
                        product.NetWeight = productGroupIntegration.NetWeight;
                        product.OldMaterialNum = productGroupIntegration.OldMaterialNum;
                        product.WeightUnit = productGroupIntegration.WeightUnit;

                        //Common
                        product.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = productGroupIntegration.Material,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
