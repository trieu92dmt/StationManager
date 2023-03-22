using Core.Identity;
using Core.Properties;
using Microsoft.AspNetCore.Mvc;
using Shared.Identity;
using Shared.Models;

namespace Authentication.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _service;

        public AuthController(ITokenService service)
        {
            _service = service;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequest request)
        {
            //Get token
            var response = await _service.GetToken(request);
            return Ok(new ApiSuccessResponse<TokenResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Đăng nhập")
            });
        }
    }
}
