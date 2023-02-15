using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.Reservation
{
    public class DeleteReservationIntegrationCommand : IRequest<bool>
    {
        public string ReservationCode { get; set; }
    }
    public class DeleteReservationIntegrationCommandHandler : IRequestHandler<DeleteReservationIntegrationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ReservationModel> _reservationRepo;
        private readonly IRepository<DetailReservationModel> _detailReservationRepo;

        public DeleteReservationIntegrationCommandHandler(IUnitOfWork unitOfWork, IRepository<ReservationModel> reservationRepo,
                                                          IRepository<DetailReservationModel> detailReservationRepo)
        {
            _unitOfWork = unitOfWork;
            _reservationRepo = reservationRepo;
            _detailReservationRepo = detailReservationRepo;
        }

        public async Task<bool> Handle(DeleteReservationIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Xóa Reservation
            var reservation = await _reservationRepo.GetQuery(x => x.ReservationCode == request.ReservationCode).Include(x => x.DetailReservationModel).FirstOrDefaultAsync();
            if (reservation is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Reservation {request.ReservationCode}");

            //Xóa Reservation Detail
            _detailReservationRepo.RemoveRange(reservation.DetailReservationModel);

            _reservationRepo.Remove(reservation);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }

}
