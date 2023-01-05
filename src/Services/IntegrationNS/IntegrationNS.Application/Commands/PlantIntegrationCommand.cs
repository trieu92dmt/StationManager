using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands
{
    public class PlantIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<PlantIntegration> Plants { get; set; } = new List<PlantIntegration>();
    }
    public class PlantIntegration
    {
        public string Plant { get; set; }
        public string PlantDescription { get; set; }
        public string CustomerNoPlant { get; set; }
        public string SupplierNoPlant { get; set; }
        public string ShippingPoint { get; set; }
        public string PurchOrganization { get; set; }
        public string SalesOrganization { get; set; }
    }
    public class PlantIntegrationCommandHandler : IRequestHandler<PlantIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<PlantModel> _plantRep;
        private readonly IUnitOfWork _unitOfWork;

        public PlantIntegrationCommandHandler(IRepository<PlantModel> plantRep, IUnitOfWork unitOfWork)
        {
            _plantRep = plantRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<IntegrationNSResponse> Handle(PlantIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.Plants.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.Plants.Count();

            var plants = _plantRep.GetQuery();

            foreach (var plantIntegration in request.Plants)
            {
                try
                {
                    //Check tồn tại
                    var plant = await _plantRep.FindOneAsync(x => x.PlantCode == plantIntegration.Plant);

                    if (plant is null)
                    {
                        _plantRep.Add(new PlantModel
                        {
                            PlantId = Guid.NewGuid(),
                            PlantCode = plantIntegration.Plant,
                            PlantName = plantIntegration.PlantDescription,
                            CustomerNoPlant = plantIntegration.CustomerNoPlant,
                            SupplierNoPlant = plantIntegration.SupplierNoPlant,
                            ShippingPoin = plantIntegration.ShippingPoint,
                            PurchasingOrgCode = plantIntegration.PurchOrganization,
                            SaleOrgCode = plantIntegration.SalesOrganization,
                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        plant.PlantName = plantIntegration.PlantDescription;
                        plant.CustomerNoPlant = plantIntegration.CustomerNoPlant;
                        plant.SupplierNoPlant = plantIntegration.SupplierNoPlant;
                        plant.ShippingPoin = plantIntegration.ShippingPoint;
                        plant.PurchasingOrgCode = plantIntegration.PurchOrganization;
                        plant.SaleOrgCode = plantIntegration.SalesOrganization;
                        //Common
                        plant.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{plantIntegration.Plant}");
                }              
            }

            return response;
        }
    }
}
