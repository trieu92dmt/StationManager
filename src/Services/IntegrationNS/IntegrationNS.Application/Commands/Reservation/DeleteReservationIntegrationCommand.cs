using IntegrationNS.Application.Commands.WorkOrder;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
