using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.CarCompany.Ticket;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.Queries.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITicketQuery _ticketQuery;

        public TicketController(IMediator mediator, ITicketQuery ticketQuery)
        {
            _mediator = mediator;
            _ticketQuery = ticketQuery;
        }

        /// <summary>
        /// Thao tác trên vé
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("action-ticket")]
        public async Task<IActionResult> BookTicket(ActionTicketCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thao tác vé")
            });
        }

        /// <summary>
        /// Get danh sách vé
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-list-ticket")]
        public async Task<IActionResult> GetListTicket(string phoneNumber)
        {
            var response = await _ticketQuery.GetListTicket(phoneNumber);

            return Ok(new ApiSuccessResponse<List<TicketResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Tìm kiếm vé")
            });
        }
    }
}
