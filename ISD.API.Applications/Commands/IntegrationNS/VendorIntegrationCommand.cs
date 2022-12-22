using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class VendorIntegrationCommand : IRequest<bool>
    {
        public string Vendor { get; set; }
        public string VendorName { get; set; }
        public string Country { get; set; }
    }
    public class VendorIntegrationCommandHandler : IRequestHandler<VendorIntegrationCommand, bool>
    {
        private readonly IRepository<VendorModel> _vendorRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public VendorIntegrationCommandHandler(IRepository<VendorModel> vendorRep, IISDUnitOfWork unitOfWork)
        {
            _vendorRep = vendorRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(VendorIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var vendor = await _vendorRep.FindOneAsync(x => x.VendorCode == request.Vendor);

            if (vendor is null)
            {
                _vendorRep.Add(new VendorModel
                {
                    VendorId = Guid.NewGuid(),
                    VendorCode = request.Vendor,
                    VendorName = request.VendorName,
                    Country = request.Country,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                vendor.VendorName = request.VendorName;
                vendor.Country = request.Country;

                //Common
                vendor.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
