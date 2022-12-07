using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class StorageLocationIntegrationCommand : IRequest<bool>
    {
        public string StorageLocation { get; set; }
        public string StorageLocationDescription { get; set; }
        public string Plant { get; set; }
        public string Division { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string Vendor { get; set; }
        public string Customer { get; set; }
    }
    public class StorageLocationIntegrationCommandHandler : IRequestHandler<StorageLocationIntegrationCommand, bool>
    {
        private readonly IGeneRepo<StorageLocationModel> _storageLocationRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public StorageLocationIntegrationCommandHandler(IGeneRepo<StorageLocationModel> storageLocationRep, IISDUnitOfWork unitOfWork)
        {
            _storageLocationRep = storageLocationRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(StorageLocationIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var storageLocation = await _storageLocationRep.FindOneAsync(x => x.StorageLocationCode == request.StorageLocation);

            if (storageLocation is null)
            {
                _storageLocationRep.Add(new StorageLocationModel
                {
                    StorageLocationId = Guid.NewGuid(),
                    StorageLocationCode = request.StorageLocation,
                    StorageLocationName = request.StorageLocationDescription,
                    PlantCode = request.Plant,
                    DivisionCode = request.Division,
                    SaleOrgCode = request.SalesOrganization,
                    DistributionChannelCode = request.DistributionChannel,
                    VendorCode = request.Vendor,
                    CustomerCode = request.Customer,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                storageLocation.StorageLocationName = request.StorageLocationDescription;
                storageLocation.PlantCode = request.Plant;
                storageLocation.DivisionCode = request.Division;
                storageLocation.PlantCode = request.Plant;
                storageLocation.SaleOrgCode = request.SalesOrganization;
                storageLocation.DistributionChannelCode = request.DistributionChannel;
                storageLocation.VendorCode = request.Vendor;
                storageLocation.CustomerCode = request.Customer;
                //Common
                storageLocation.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
