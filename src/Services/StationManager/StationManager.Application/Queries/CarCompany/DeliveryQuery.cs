using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;

namespace StationManager.Application.Queries.CarCompany
{
    public interface IDeliveryQuery
    {
        Task<List<DeliveryResponse>> GetListDelivery(GetListDeliveryRequest request);
    }

    public class DeliveryQuery : IDeliveryQuery
    {
        private readonly IRepository<DeliveryModel> _deliveryRepo;
        private readonly IRepository<provinces> _provinceRepo;

        public DeliveryQuery(IRepository<DeliveryModel> deliveryRepo, IRepository<provinces> provinceRepo)
        {
            _deliveryRepo = deliveryRepo;
            _provinceRepo = provinceRepo;
        }

        public Task<List<DeliveryResponse>> GetListDelivery(GetListDeliveryRequest request)
        {
            //province
            var province = _provinceRepo.GetQuery().AsNoTracking();

            var response = _deliveryRepo.GetQuery().Include(x => x.Trip).ThenInclude(x => x.Route)
                                        .Where(x => (request.RouteId.HasValue ? x.Trip.RouteId == request.RouteId : true) &&
                                        (!string.IsNullOrEmpty(request.Receiver) ? x.Receiver.Contains(request.Receiver) : true) &&
                                        (!string.IsNullOrEmpty(request.Email) ? x.Email == request.Email : true) &&
                                        (!string.IsNullOrEmpty(request.PhoneNumber) ? x.PhoneNumber.Contains(request.PhoneNumber) : true) &&
                                        (!string.IsNullOrEmpty(request.Status) ? x.Status == request.Status : x.Status == "InProgress"))
                                        .Select(x => new DeliveryResponse()
                                        {
                                            DeliveryId = x.DeliveryId,
                                            DeliveryCode = x.DeliveryCode.ToString(),
                                            Sender = x.Sender,
                                            Receiver = x.Receiver,
                                            PhoneNumber = x.PhoneNumber,
                                            Email = x.Email,
                                            Status = x.Status,
                                            isShipAtHome = x.isShipAtHome ?? false,
                                            Address = x.Address,
                                            Cost = x.Cost ?? 0,
                                            TripInfo = $"{province.FirstOrDefault(p => p.code == x.Trip.Route.StartPoint).name} - {province.FirstOrDefault(p => p.code == x.Trip.Route.EndPoint).name} - {x.Trip.StartDate.Value.ToString("HH:mm")}"
                                        }).ToListAsync();

            return response;
        }
    }
}
