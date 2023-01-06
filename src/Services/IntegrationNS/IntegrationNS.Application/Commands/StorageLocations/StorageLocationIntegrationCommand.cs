using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.StorageLocations
{
    public class StorageLocationIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<StorageLocationIntegration> StorageLocations { get; set; } = new List<StorageLocationIntegration>();
    }

    public class StorageLocationIntegration
    {
        public string StorageLocation { get; set; }
        public string StorageLocationDescription { get; set; }
        public string Plant { get; set; }
        //public string division { get; set; }
        //public string salesorganization { get; set; }
        //public string distributionchannel { get; set; }
        //public string vendor { get; set; }
        //public string customer { get; set; }
    }
    public class StorageLocationIntegrationCommandHandler : IRequestHandler<StorageLocationIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<StorageLocationModel> _storageLocationRep;
        private readonly IUnitOfWork _unitOfWork;

        public StorageLocationIntegrationCommandHandler(IRepository<StorageLocationModel> storageLocationRep, IUnitOfWork unitOfWork)
        {
            _storageLocationRep = storageLocationRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(StorageLocationIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.StorageLocations.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.StorageLocations.Count();

            foreach (var storageLocationIntegration in request.StorageLocations)
            {
                try
                {
                    //Check tồn tại
                    var storageLocation = await _storageLocationRep.FindOneAsync(x => x.StorageLocationCode == storageLocationIntegration.StorageLocation);

                    if (storageLocation is null)
                    {
                        _storageLocationRep.Add(new StorageLocationModel
                        {
                            StorageLocationId = Guid.NewGuid(),
                            StorageLocationCode = storageLocationIntegration.StorageLocation,
                            StorageLocationName = storageLocationIntegration.StorageLocationDescription,
                            PlantCode = storageLocationIntegration.Plant,
                            //DivisionCode = storageLocationIntegration.Division,
                            //SaleOrgCode = storageLocationIntegration.SalesOrganization,
                            //DistributionChannelCode = storageLocationIntegration.DistributionChannel,
                            //VendorCode = storageLocationIntegration.Vendor,
                            //CustomerCode = storageLocationIntegration.Customer,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        storageLocation.StorageLocationName = storageLocationIntegration.StorageLocationDescription;
                        storageLocation.PlantCode = storageLocationIntegration.Plant;
                        //storageLocation.DivisionCode = storageLocationIntegration.Division;
                        //storageLocation.PlantCode = storageLocationIntegration.Plant;
                        //storageLocation.SaleOrgCode = storageLocationIntegration.SalesOrganization;
                        //storageLocation.DistributionChannelCode = storageLocationIntegration.DistributionChannel;
                        //storageLocation.VendorCode = storageLocationIntegration.Vendor;
                        //storageLocation.CustomerCode = storageLocationIntegration.Customer;
                        //Common
                        storageLocation.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();
                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{storageLocationIntegration.StorageLocation}");
                }
            }

            return response;
        }
    }
}
