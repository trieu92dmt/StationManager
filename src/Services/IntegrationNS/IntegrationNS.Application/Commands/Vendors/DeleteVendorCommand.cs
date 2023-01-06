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

namespace IntegrationNS.Application.Commands.Vendors
{
    public class DeleteVendorCommand : IRequest<DeleteNSResponse>
    {
        public List<string> Vendors { get; set; } = new List<string>();
    }
    public class DeleteVendorCommandHandler : IRequestHandler<DeleteVendorCommand, DeleteNSResponse>
    {
        private readonly IRepository<VendorModel> _vendorRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVendorCommandHandler(IRepository<VendorModel> vendorRep, IUnitOfWork unitOfWork)
        {
            _vendorRep = vendorRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.Vendors.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.Vendors.Count();

            foreach (var vendorDelete in request.Vendors)
            {
                try
                {
                    //Xóa Storage Location
                    var storageLocation = await _vendorRep.FindOneAsync(x => x.VendorCode == vendorDelete);
                    if (storageLocation is not null)
                    {
                        _vendorRep.Remove(storageLocation);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(vendorDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(vendorDelete);
                }

            }
            return response;
        }
    }
}
