using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.RouteManager
{
    public class UpdateRouteCommand : IRequest<bool>
    {
        public Guid RouteId { get; set; }
        public string Description { get; set; }
        public decimal Distance { get; set; }
    }

    public class UpdateRouteCommandHandler : IRequestHandler<UpdateRouteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RouteModel> _routeRepo;

        public UpdateRouteCommandHandler(IUnitOfWork unitOfWork, IRepository<RouteModel> routeRepo)
        {
            _unitOfWork = unitOfWork;
            _routeRepo = routeRepo;
        }

        public async Task<bool> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _routeRepo.FindOneAsync(x => x.RouteId == request.RouteId);

            route.Description = request.Description;
            route.Distance = request.Distance;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
