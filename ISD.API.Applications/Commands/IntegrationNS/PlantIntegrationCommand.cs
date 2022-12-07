using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class PlantIntegrationCommand : IRequest<bool>
    {
        public string Plant { get; set; }
        public string PlantDescription { get; set; }
        public string CustomerNoPlant { get; set; }
        public string SupplierNoPlant { get; set; }
        public string ShippingPoint { get; set; }
        public string PurchOrganization { get; set; }
        public string SalesOrganization { get; set; }
    }

    public class PlantIntegrationCommandHandler : IRequestHandler<PlantIntegrationCommand, bool>
    {
        private readonly IGeneRepo<PlantModel> _plantRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public PlantIntegrationCommandHandler(IGeneRepo<PlantModel> plantRep, IISDUnitOfWork unitOfWork)
        {
            _plantRep = plantRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(PlantIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var plant = await _plantRep.FindOneAsync(x => x.PlantCode == request.Plant);

            if (plant is null)
            {
                _plantRep.Add(new PlantModel
                {
                    PlantId = Guid.NewGuid(),
                    PlantCode = request.Plant,
                    PlantName = request.PlantDescription,
                    CustomerNoPlant = request.CustomerNoPlant,
                    SupplierNoPlant = request.SupplierNoPlant,
                    ShippingPoin = request.ShippingPoint,
                    PurchasingOrgCode = request.PurchOrganization,
                    SaleOrgCode = request.SalesOrganization,
                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                plant.PlantName = request.PlantDescription;
                plant.CustomerNoPlant = request.CustomerNoPlant;
                plant.SupplierNoPlant = request.SupplierNoPlant;
                plant.ShippingPoin = request.ShippingPoint;
                plant.PurchasingOrgCode = request.PurchOrganization;
                plant.SaleOrgCode = request.SalesOrganization;
                //Common
                plant.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
