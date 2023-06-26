using Core.Extensions;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StationManager.Application.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace StationManager.Application.Commands.CarCompany.Ticket
{
    public class ActionTicketCommand : IRequest<bool>
    {
        public Guid TripId { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public List<Guid> Seats { get; set; }
    }

    public class ActionTicketCommandHandler : IRequestHandler<ActionTicketCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TicketModel> _ticketRepo;
        private readonly IRepository<TripModel> _tripRepo;
        private readonly IRepository<Ticket_Seat_Mapping> _tkSeatRepo;
        private readonly IMailService _mailService;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<SeatModel> _seatRepo;
        private readonly ISMSService _smsService;
        private readonly IConfiguration _config;

        public ActionTicketCommandHandler(IUnitOfWork unitOfWork, IRepository<TicketModel> ticketRepo, 
                                          IRepository<TripModel> tripRepo, IRepository<Ticket_Seat_Mapping> tkSeatRepo,
                                          IMailService mailService, IRepository<CarCompanyModel> carCompanyRepo,
                                          IRepository<SeatModel> seatRepo, ISMSService smsService, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _ticketRepo = ticketRepo;
            _tripRepo = tripRepo;
            _tkSeatRepo = tkSeatRepo;
            _mailService = mailService;
            _carCompanyRepo = carCompanyRepo;
            _seatRepo = seatRepo;
            _smsService = smsService;
            _config = config;
        }

        public async Task<bool> Handle(ActionTicketCommand request, CancellationToken cancellationToken)
        {
            var trip = await _tripRepo.FindOneAsync(x => x.TripId == request.TripId);

            var newTicket = new TicketModel
            {
                CreatedBy = TokenExtensions.GetAccountId(),
                CreatedTime = DateTime.Now,
                Actived = true,
                TicketId = Guid.NewGuid(),
                TripId = request.TripId,
                CarCompanyId = trip.CarCompanyId,
                Price = request.Price,
                UserId = request.UserId.HasValue ? request.UserId.Value : null,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Status = request.Status,
            };

            //Add seat into ticket
            foreach(var item in request.Seats)
            {
                newTicket.Ticket_Seat_Mapping.Add(new Ticket_Seat_Mapping
                {
                    Ticket_Seat_MappingId = Guid.NewGuid(),
                    TicketId = newTicket.TicketId,
                    SeatId = item
                });
            }

            var company = await _carCompanyRepo.FindOneAsync(x => x.CarCompanyId == trip.CarCompanyId);
            var seatQuery = _seatRepo.GetQuery().AsNoTracking();
            var seatListFmt = string.Join(',', seatQuery.Where(x => request.Seats.Contains(x.SeatId)).Select(x => x.SeatNumber));

            if (!string.IsNullOrEmpty(request.Email))
            {
                var content = new MailContent
                {
                    To = request.Email,
                    Subject = "Thông báo đặt vé thành công",
                    Body = $"Quý khách vừa thành công đặt thành công mã ghế {seatListFmt} của nhà xe {company.CarCompanyName}" +
                    $"\nChuyến đi ngày {trip.StartDate.Value.Date}, giờ khởi hành {trip.StartDate.Value.ToString("HH:mm")}\nCảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!"
                };

                _mailService.SendMail(content);
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var body = $"Quý khách vừa thành công đặt thành công mã ghế {seatListFmt} của nhà xe {company.CarCompanyName}" +
                    $"\nChuyến đi ngày {trip.StartDate.Value.Date}, giờ khởi hành {trip.StartDate.Value.ToString("HH:mm")}\nCảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!";
                var to = "+84948513923";
                //_smsService.SendSMS(body, to);

                //Get Account
                var account = _config.GetSection("Twilio")["Account"];
                var authTokent = _config.GetSection("Twilio")["AuthToken"];
                var phoneNumber = _config.GetSection("Twilio")["PhoneNumber"];

                TwilioClient.Init(account, authTokent);

                var message = MessageResource.Create(
                    body: body,
                    from: new Twilio.Types.PhoneNumber(phoneNumber),
                    to: new Twilio.Types.PhoneNumber(to)
                );
            }

            _ticketRepo.Add(newTicket);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
