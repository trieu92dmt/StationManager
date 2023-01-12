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
