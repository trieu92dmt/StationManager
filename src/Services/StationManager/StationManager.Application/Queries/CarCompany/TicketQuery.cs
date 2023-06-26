using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;

namespace StationManager.Application.Queries.CarCompany
{
    public interface ITicketQuery
    {
        Task<List<TicketResponse>> GetListTicket(string phoneNumber);
    }

    public class TicketQuery : ITicketQuery
    {
        private readonly IRepository<TicketModel> _ticketRepo;

        public TicketQuery(IRepository<TicketModel> ticketRepo)
        {
            _ticketRepo = ticketRepo;
        }

        public async Task<List<TicketResponse>> GetListTicket(string phoneNumber)
        {
            var response = await _ticketRepo.GetQuery(x => (!string.IsNullOrEmpty(phoneNumber)) ?
                                                            x.PhoneNumber.Contains(phoneNumber) && x.Status == "booked" : false)
                                      .Include(x => x.Ticket_Seat_Mapping)
                                      .Select(x => new TicketResponse
                                      {
                                          TicketId = x.TicketId,
                                          TicketCode = x.TicketCode.ToString(),
                                          Status = x.Status,
                                          Price = x.Price,
                                          BookDate = x.CreatedTime,
                                          Name = x.Name,
                                          PhoneNumber = x.PhoneNumber,
                                          Email = x.Email,
                                          Seats = x.Ticket_Seat_Mapping.Select(x => x.Seat.SeatNumber).ToList()
                                      }).ToListAsync();

            return response;
        }
    }
}
