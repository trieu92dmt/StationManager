using MediatR;
using MES.Application.Commands.ScaleMonitor;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.WeighSession;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ScaleMonitorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScaleMonitorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Search Scale Monitor
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-scale-monitor")]
        public async Task<IActionResult> GetDataScaleMonitorAsync([FromBody] SearchScaleMinitorCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<List<SearchScaleMonitorResponse>>
            {
                Data = response
            });
        }
    }
}
