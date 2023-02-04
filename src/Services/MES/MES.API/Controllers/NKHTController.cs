using ISD.Core.Models;
using MediatR;
using MES.Application.Commands.MES;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.DTOs.MES;
using MES.Application.DTOs.MES.OutboundDelivery;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKHTController : ControllerBase
    {
        private readonly IOutboundDeliveryQuery _query;
        private readonly IMediator _mediator;

        public NKHTController(IOutboundDeliveryQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        [HttpPost("get-outbound-delivery")]
        public async Task<IActionResult> GetOutboundDeliveryAsync([FromBody] SearchOutboundDeliveryCommand command)
        {
            var response = await _query.GetOutboundDelivery(command);

            return Ok(new ApiSuccessResponse<List<OutboundDeliveryResponse>>
            {
                Data = response
            });
        }
    }
}
