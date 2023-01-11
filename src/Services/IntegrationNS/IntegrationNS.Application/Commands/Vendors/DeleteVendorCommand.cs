using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.Vendors
{
    public class DeleteVendorCommand : IRequest<bool>
    {
        public string Vendor { get; set; } 
    }
    public class DeleteVendorCommandHandler : IRequestHandler<DeleteVendorCommand, bool>
    {
        private readonly IRepository<VendorModel> _vendorRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVendorCommandHandler(IRepository<VendorModel> vendorRep, IUnitOfWork unitOfWork)
        {
            _vendorRep = vendorRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
        {
            //Xóa Storage Location
            var vendor = await _vendorRep.FindOneAsync(x => x.VendorCode == request.Vendor);
            if (vendor is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Vendor {request.Vendor}");


            _vendorRep.Remove(vendor);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
