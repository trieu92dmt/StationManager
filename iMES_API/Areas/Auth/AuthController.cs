using ISD.API.Applications.Commands.Auth;
using ISD.API.Core.Extensions;
using ISD.API.Resources;
using ISD.API.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TLG_API.Areas.Auth
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

            return Ok(new ApiSuccessResponse<UserTokens>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success , "Đăng nhập")
            });
        }
    }
}
