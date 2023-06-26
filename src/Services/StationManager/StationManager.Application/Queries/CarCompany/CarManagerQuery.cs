using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Queries.CarCompany
{
    public interface ICarManagerQuery 
    {
        Task<List<CarResponse>> GetListCar(GetListCarRequest request);

        Task<DetailCarResponse> GetDetailCar(Guid carId);

        Task<SeatDiagramResponse> GetSeatByCarNumber(string carNumber);
    }

    public class CarManagerQuery : ICarManagerQuery
    {
        private readonly IRepository<CarModel> _carRepo;
        private readonly IRepository<CarTypeModel> _carTypeRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<SeatModel> _seatRepo;

        public CarManagerQuery(IRepository<CarModel> carRepo, IRepository<CarTypeModel> carTypeRepo, 
                               IRepository<CarCompanyModel> carCompanyRepo, IRepository<SeatModel> seatRepo)
        {
            _carRepo = carRepo;
            _carTypeRepo = carTypeRepo;
            _carCompanyRepo = carCompanyRepo;
            _seatRepo = seatRepo;
        }

        public async Task<DetailCarResponse> GetDetailCar(Guid carId)
        {
            var car = await _carRepo.GetQuery().Include(x => x.CarType).ThenInclude(x => x.SeatModel)
                                    .FirstOrDefaultAsync(x => x.CarId == carId);

            return new DetailCarResponse
            {
                CarId = car.CarId,
                CarNumber = car.CarNumber,
                CarTypeCode = car.CarType.CarTypeCode,
                CarTypeName = car.CarType.CarTypeName,
                Description = car.Description,
                Levels = car.CarType.FloorNumber ?? 0,
                Columns = car.CarType.ColNumber ?? 0,
                Rows = car.CarType.RowNumber ?? 0,
                Seats = car.CarType.SeatModel.Select(x => new SeatResponse
                {
                    Levels = x.Floor ?? 0,
                    Columns = x.Col ?? 0,
                    Rows = x.Row ?? 0,
                    SeatNumber = x.SeatNumber,
                    Actived = x.Actived ?? false,
                }).ToList()
            };
        }

        public async Task<List<CarResponse>> GetListCar(GetListCarRequest request)
        {
            //Get CarCompany by account
            var company = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            //Get List Car
            var response = await _carRepo.GetQuery().Include(x => x.CarType)
                           .Where(x => (!string.IsNullOrEmpty(request.CarNumber) ? x.CarNumber == request.CarNumber : true) &&
                                       (!string.IsNullOrEmpty(request.CarTypeCode) ? x.CarType.CarTypeCode == request.CarTypeCode: true) &&
                                       x.CarCompanyId == company.CarCompanyId)
                           .OrderByDescending(x => x.CreateTime)
                           .Select(x => new CarResponse
                           {
                               CarId = x.CarId,
                               CarNumber = x.CarNumber,
                               CarType = x.CarType.CarTypeName,
                               Description = x.Description,
                               SeatQuantity = x.CarType.SeatQuantity ?? 0,
                               CreateTime = x.CreateTime,
                           }).ToListAsync();

            return response;
        }

        public async Task<SeatDiagramResponse> GetSeatByCarNumber(string carNumber)
        {
            //Get car by car number
            var car = await _carRepo.GetQuery().Include(x => x.CarType).FirstOrDefaultAsync(x => x.CarNumber == carNumber);

            var response = new SeatDiagramResponse
            {
                Levels = car.CarType.FloorNumber,
                Columns = car.CarType.ColNumber,
                Rows = car.CarType.RowNumber,
                Seats = await _seatRepo.GetQuery().Where(x => x.CarTypeId == car.CarTypeId)
                                    .Select(x => new SeatResponse
                                    {
                                        Columns = x.Col ?? 0,
                                        Rows = x.Row ?? 0,
                                        Levels = x.Floor ?? 0,
                                        Actived = x.Actived ?? false,
                                        SeatNumber = x.SeatNumber,
                                        Status = x.Status
                                    }).AsNoTracking().ToListAsync()
            };
            return response;
        }
    }
}
