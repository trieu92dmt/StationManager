using Authentication.Application.Commands;
using ISD.Core.Jwt.Models;
using ISD.Core.Models;
using ISD.Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> GetNKMHAsync([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<UserToken>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Đăng nhập")
            });
        }
    }
}
