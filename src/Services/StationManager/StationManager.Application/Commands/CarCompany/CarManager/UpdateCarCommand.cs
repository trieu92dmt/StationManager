using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace StationManager.Application.Commands.CarCompany.CarManager
{
    public class UpdateCarCommand : IRequest<bool>
    {
        public Guid CarId { get; set; }
        public string CarNumber { get; set; }
        public string Description { get; set; }
        public string CarTypeCode { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }

    public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarModel> _carRepo;
        private readonly IRepository<CarTypeModel> _carTypeRepo;
        private readonly IRepository<SeatModel> _seatRepo;

        public UpdateCarCommandHandler(IUnitOfWork unitOfWork, IRepository<CarModel> carRepo, IRepository<CarTypeModel> carTypeRepo,
                                       IRepository<SeatModel> seatRepo)
        {
            _unitOfWork = unitOfWork;
            _carRepo = carRepo;
            _carTypeRepo = carTypeRepo;
            _seatRepo = seatRepo;
        }

        public async Task<bool> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
        {
            //Get car
            var car = await _carRepo.FindOneAsync(x => x.CarId == request.CarId);

            //Get car type
            var carType = await _carTypeRepo.FindOneAsync(x => x.CarTypeCode == request.CarTypeCode);

            //Danh sách ghế
            var seats = await _seatRepo.GetQuery(x => x.CarTypeId == carType.CarTypeId).ToListAsync();
            //Xóa cũ add mới
            _seatRepo.RemoveRange(seats);
            foreach (var seat in request.Seats)
            {
                carType.SeatModel.Add(new SeatModel
                {
                    SeatId = Guid.NewGuid(),
                    SeatNumber = seat.SeatNumber,
                    Floor = seat.Levels,
                    CarTypeId = carType.CarTypeId,
                    Col = seat.Columns,
                    Row = seat.Rows,
                    Status = seat.Actived ? "available" : "unAvailable",
                    Actived = seat.Actived,
                });
            }


            //Update car
            car.CarNumber = request.CarNumber;
            car.Description = request.Description;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
