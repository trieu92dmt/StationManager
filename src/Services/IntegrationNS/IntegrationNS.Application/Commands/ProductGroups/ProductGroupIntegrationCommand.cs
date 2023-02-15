using IntegrationNS.Application.DTOs;
using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.ProductGroups
{
    public class ProductGroupIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<ProductGroupIntegration> ProductGroups { get; set; } = new List<ProductGroupIntegration>();
    }
    public class ProductGroupIntegration
    {
        public string MaterialGroup { get; set; }
        public string MaterialGroupName { get; set; }
    }
    public class ProductGroupIntegrationCommandHandler : IRequestHandler<ProductGroupIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<ProductGroupModel> _productGroupRep;
        private readonly IUnitOfWork _unitOfWork;

        public ProductGroupIntegrationCommandHandler(IRepository<ProductGroupModel> productGroupRep, IUnitOfWork unitOfWork)
        {
            _productGroupRep = productGroupRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<IntegrationNSResponse> Handle(ProductGroupIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.ProductGroups.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.ProductGroups.Count();

            var productGroups = _productGroupRep.GetQuery();

            foreach (var productGroupIntegration in request.ProductGroups)
            {
                try
                {
                    //Check tồn tại
                    var productGroup = await _productGroupRep.FindOneAsync(x => x.ProductGroupCode == productGroupIntegration.MaterialGroup);

                    if (productGroup is null)
                    {
                        _productGroupRep.Add(new ProductGroupModel
                        {
                            ProductGroupId = Guid.NewGuid(),
                            ProductGroupCode = productGroupIntegration.MaterialGroup,
                            ProductGroupName = productGroupIntegration.MaterialGroupName,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        productGroup.ProductGroupName = productGroupIntegration.MaterialGroupName;

                        //Common
                        productGroup.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = productGroupIntegration.MaterialGroup,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
