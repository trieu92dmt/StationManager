using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Commands.CarCompany.CarManager
{
    public class AddCarTypeCommand : IRequest<bool>
    {
        public Guid? AccountId { get; set; }
        public string CarTypeName { get; set; }
        public int Levels { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }

    public class Seat
    {
        public string SeatNumber { get; set; }
        public int Levels { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public bool Actived { get; set; }
    }

    public class AddCarTypeCommandHandler : IRequestHandler<AddCarTypeCommand, bool>
    {
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<CarTypeModel> _carTypeRepo;
        private readonly IRepository<SeatModel> _seatRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddCarTypeCommandHandler(IRepository<CarCompanyModel> carCompanyRepo, IRepository<CarTypeModel> carTypeRepo,
                                    IRepository<SeatModel> seatRepo, IUnitOfWork unitOfWork)
        {
            _carCompanyRepo = carCompanyRepo;
            _carTypeRepo = carTypeRepo;
            _seatRepo = seatRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddCarTypeCommand request, CancellationToken cancellationToken)
        {
            var carType = _carTypeRepo.GetQuery().Include(x => x.SeatModel).AsNoTracking();

            var carCompanyRepo = _carCompanyRepo.GetQuery().AsNoTracking();

            //New car type
            var newCarType = new CarTypeModel()
            {
                CarTypeId = Guid.NewGuid(),
                CarTypeCode = $"CT{100000 + carType.Count()}",
                isCustomCarType = request.AccountId.HasValue ? true : false,
                CarCompanyId = request.AccountId.HasValue ? 
                               carCompanyRepo.FirstOrDefault(x => x.AccountId == request.AccountId).CarCompanyId : null,
                CarTypeName = request.CarTypeName,
                SeatQuantity = request.Seats.Count(),
                ColNumber = request.Columns,
                RowNumber = request.Rows,
                FloorNumber = request.Levels,
                CreateTime = DateTime.Now,
                Actived = true,
            };

            //New list seat
            foreach (var seat in request.Seats)
            {
                newCarType.SeatModel.Add(new SeatModel
                {
                    SeatId = Guid.NewGuid(),
                    CarTypeId = newCarType.CarTypeId,
                    SeatNumber = seat.SeatNumber,
                    Floor = seat.Levels,
                    Col = seat.Columns,
                    Row = seat.Rows,
                    Status = seat.Actived ? "available" : "unAvailable",
                    Actived = seat.Actived,
                });
            }

            _carTypeRepo.Add(newCarType);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
