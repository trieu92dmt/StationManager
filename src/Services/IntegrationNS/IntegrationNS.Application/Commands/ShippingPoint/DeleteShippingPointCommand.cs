using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.ShippingPoint
{
    public class DeleteShippingPointCommand : IRequest<bool>
    {
        public string ShippingPointCode { get; set; }
    }

    public class DeleteShippingPointCommandHandler : IRequestHandler<DeleteShippingPointCommand, bool>
    {
        private readonly IRepository<ShippingPointModel> _shipRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteShippingPointCommandHandler(IRepository<ShippingPointModel> shipRepo, IUnitOfWork unitOfWork)
        {
            _shipRepo = shipRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteShippingPointCommand request, CancellationToken cancellationToken)
        {
            //Xóa Shipping Point
            var plant = await _shipRepo.FindOneAsync(x => x.ShippingPointCode == request.ShippingPointCode);
            if (plant is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Shipping Point {request.ShippingPointCode}");

            _shipRepo.Remove(plant);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
