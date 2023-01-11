using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.Vendors
{
    public class VendorIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<VendorIntegration> Vendors { get; set; } = new List<VendorIntegration>();
    }

    public class VendorIntegration
    {
        public string Vendor { get; set; }
        public string VendorName { get; set; }
        public string Country { get; set; }
    }

    public class VendorIntegrationCommandHandler : IRequestHandler<VendorIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<VendorModel> _vendorRep;
        private readonly IUnitOfWork _unitOfWork;

        public VendorIntegrationCommandHandler(IRepository<VendorModel> vendorRep, IUnitOfWork unitOfWork)
        {
            _vendorRep = vendorRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(VendorIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.Vendors.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.Vendors.Count();

            var vendors = _vendorRep.GetQuery();

            foreach (var vendorIntegration in request.Vendors)
            {
                try
                {
                    //Check tồn tại
                    var vendor = await vendors.FirstOrDefaultAsync(x => x.VendorCode == vendorIntegration.Vendor);

                    if (vendor is null)
                    {
                        _vendorRep.Add(new VendorModel
                        {
                            VendorId = Guid.NewGuid(),
                            VendorCode = vendorIntegration.Vendor,
                            VendorName = vendorIntegration.VendorName,
                            Country = vendorIntegration.Country,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        vendor.VendorName = vendorIntegration.VendorName;
                        vendor.Country = vendorIntegration.Country;

                        //Common
                        vendor.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception ex)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add(new DetailIntegrationFailResponse
                    {
                        RecordFail = vendorIntegration.Vendor,
                        Msg = ex.Message
                    });
                }
            }

            return response;
        }
    }
}
