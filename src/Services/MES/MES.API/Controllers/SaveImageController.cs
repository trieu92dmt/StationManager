using MediatR;
using MES.Application.Commands.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SaveImageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SaveImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("save-image")]
        public async Task<IActionResult> SaveImageAsync([FromForm] SaveImageCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<Guid>
            {
                Data = response
            });
        }
    }
}
