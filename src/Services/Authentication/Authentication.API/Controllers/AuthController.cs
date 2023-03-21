using Authentication.Application.Commands;
using Authentication.Application.DTOs;
using Authentication.Application.Queries;
using Core.Jwt.Models;
using DTOs.Models;
using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Authentication.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthQuery _query;

        public AuthController(IMediator mediator, IAuthQuery query)
        {
            _mediator = mediator;
            _query = query;
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

        /// <summary>
        /// Get plant by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("get-plant-by-username")]
        public async Task<IActionResult> GetPlantByUserName([Required] string username)
        {
            var response = await _query.GetPlantByUserName(username);

            return Ok(new ApiSuccessResponse<List<PlantByUserResponse>>
            {
                Data = response
            });
        }
    }
}
