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
    public class RemoveRouteCommand : IRequest<bool>
    {
        public Guid RouteId { get; set; }
    }

    public class RemoveRouteCommandHandler : IRequestHandler<RemoveRouteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RouteModel> _routeRepo;

        public RemoveRouteCommandHandler(IUnitOfWork unitOfWork, IRepository<RouteModel> routeRepo)
        {
            _unitOfWork = unitOfWork;
            _routeRepo = routeRepo;
        }

        public async Task<bool> Handle(RemoveRouteCommand request, CancellationToken cancellationToken)
        {
            //Get route
            var route = await _routeRepo.FindOneAsync(x => x.RouteId == request.RouteId);

            _routeRepo.Remove(route);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
