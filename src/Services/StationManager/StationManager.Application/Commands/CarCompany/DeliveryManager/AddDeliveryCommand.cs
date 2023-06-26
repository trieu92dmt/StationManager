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
    public class AddDeliveryCommand : IRequest<bool>
    {
        public Guid AccountId { get; set; }
        public Guid TripId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool isShipAtHome { get; set; }
        public decimal Cost { get; set; }
    }

    public class AddDeliveryCommandHandler : IRequestHandler<AddDeliveryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DeliveryModel> _deliveryRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;

        public AddDeliveryCommandHandler(IUnitOfWork unitOfWork, IRepository<DeliveryModel> deliveryRepo,
                                         IRepository<CarCompanyModel> carCompanyRepo)
        {
            _unitOfWork = unitOfWork;
            _deliveryRepo = deliveryRepo;
            _carCompanyRepo = carCompanyRepo;
        }

        public async Task<bool> Handle(AddDeliveryCommand request, CancellationToken cancellationToken)
        {
            //Get Car company
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            _deliveryRepo.Add(new DeliveryModel
            {
                DeliveryId = Guid.NewGuid(),
                CarCompanyId = carCompany.CarCompanyId,
                TripId = request.TripId,
                Sender = request.Sender,
                Receiver = request.Receiver,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                isShipAtHome = request.isShipAtHome,
                Status = "InProgress",
                Cost = request.Cost,
                Actived = true,
                CreatedTime = DateTime.Now
            });

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
