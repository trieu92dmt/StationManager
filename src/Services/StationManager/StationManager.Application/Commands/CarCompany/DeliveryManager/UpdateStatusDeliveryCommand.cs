using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.CarCompany.DeliveryManager
{
    public class UpdateStatusDeliveryCommand : IRequest<bool>
    {
        public Guid DeliveryId { get; set; }
    }

    public class UpdateStatusDeliveryCommandHandler : IRequestHandler<UpdateStatusDeliveryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DeliveryModel> _deliveryRepo;

        public UpdateStatusDeliveryCommandHandler(IUnitOfWork unitOfWork, IRepository<DeliveryModel> deliveryRepo)
        {
            _unitOfWork = unitOfWork;
            _deliveryRepo = deliveryRepo;
        }

        public async Task<bool> Handle(UpdateStatusDeliveryCommand request, CancellationToken cancellationToken)
        {
            //Get delivery
            var delivery = await _deliveryRepo.FindOneAsync(x => x.DeliveryId == request.DeliveryId);

            delivery.Status = "Done";

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
