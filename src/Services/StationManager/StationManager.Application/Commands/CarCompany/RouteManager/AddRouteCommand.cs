using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.RouteManager
{
    public class AddRouteCommand : IRequest<ApiResponse>
    {
        public Guid AccountId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal Distance { get; set; }
        public string Description { get; set; }
    }

    public class AddRouteCommandHandler : IRequestHandler<AddRouteCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RouteModel> _routeRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<districts> _districtRepo;
        private readonly IRepository<provinces> _provinceRepo;

        public AddRouteCommandHandler(IUnitOfWork unitOfWork, IRepository<RouteModel> routeRepo,
                                      IRepository<CarCompanyModel> carCompanyRepo, IRepository<districts> districtRepo,
                                      IRepository<provinces> provinceRepo)
        {
            _unitOfWork = unitOfWork;
            _routeRepo = routeRepo;
            _carCompanyRepo = carCompanyRepo;
            _districtRepo = districtRepo;
            _provinceRepo = provinceRepo;
        }

        public async Task<ApiResponse> Handle(AddRouteCommand request, CancellationToken cancellationToken)
        {
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            var checkRoute = await _routeRepo.GetQuery().Include(x => x.CarCompany)
                                             .FirstOrDefaultAsync(x => x.StartPoint == request.StartPoint && x.EndPoint == request.EndPoint &&
                                                                       x.CarCompany.AccountId == request.AccountId);
            if (checkRoute != null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = $"Tuyến đi từ {request.StartPoint} đến {request.EndPoint} đã tồn tại"
                };
            }


            //Get district
            var startPoint = await _provinceRepo.FindOneAsync(x => x.code == request.StartPoint);
            var endPoint = await _provinceRepo.FindOneAsync(x => x.code == request.EndPoint);

            var route = new RouteModel
            {
                Actived = true,
                RouteId = Guid.NewGuid(),
                CarCompanyId = carCompany.CarCompanyId,
                CreateTime = DateTime.Now,
                Description = request.Description,
                Distance = request.Distance,
                EndPoint = endPoint.code,
                StartPoint = startPoint.code,
                RouteCode = $"TX{startPoint.code}-{endPoint.code}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}",
            };
            _routeRepo.Add(route);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse
            {
                IsSuccess = true,
                Message = $"Thêm mới thành công!"
            };
        }
    }
}
