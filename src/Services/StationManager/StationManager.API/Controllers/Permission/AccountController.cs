using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.Permission;
using StationManager.Application.DTOs.CarCompany.Admin;
using StationManager.Application.DTOs.CarCompany.Permission;
using StationManager.Application.Queries.Permission;

namespace StationManager.API.Controllers.Permission
{
    [Route("api/v{version:apiVersion}/Permission/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountQuery _query;
        private readonly IMediator _mediator;

        public AccountController(IAccountQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        /// <summary>
        /// Get danh sách tài khoản
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("search-account")]
        public async Task<IActionResult> SearchAccount(SearchAccountCommand command)
        {
            var response = await _query.SearchAccount(command);

            return Ok(new ApiSuccessResponse<List<AccountResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tài khoản"),
            });
        }

        /// <summary>
        /// Cập nhật tài khoản
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("update-account")]
        public async Task<IActionResult> UpdateAccount(UpdateAccountCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật tài khoản"),
            });
        }
    }
}
