using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.OutboundDelivery
{
    public class DeleteOutboundDeliveryIntegrationCommand : IRequest<bool>
    {
        public string DeliveryCode { get; set; }
    }

    public class DeleteOutboundDeliveryIntegrationCommandHandler : IRequestHandler<DeleteOutboundDeliveryIntegrationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OutboundDeliveryModel> _deliveryRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _deliDetailRepo;

        public DeleteOutboundDeliveryIntegrationCommandHandler(IUnitOfWork unitOfWork, IRepository<OutboundDeliveryModel> deliveryRepo, IRepository<DetailOutboundDeliveryModel> deliDetailRepo)
        {
            _unitOfWork = unitOfWork;
            _deliveryRepo = deliveryRepo;
            _deliDetailRepo = deliDetailRepo;
        }

        public async Task<bool> Handle(DeleteOutboundDeliveryIntegrationCommand request, CancellationToken cancellationToken)
        {
            //OutboundDelivery
            var outboundDelivery = await _deliveryRepo.GetQuery(x => x.DeliveryCode == request.DeliveryCode)
                                      .Include(x => x.DetailOutboundDeliveryModel)
                                      .FirstOrDefaultAsync();
            if (outboundDelivery is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"OutboundDelivery {request.DeliveryCode}");

            //Xóa outbound delivery detail
            _deliDetailRepo.RemoveRange(outboundDelivery.DetailOutboundDeliveryModel);

            //Xóa PO
            _deliveryRepo.Remove(outboundDelivery);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
