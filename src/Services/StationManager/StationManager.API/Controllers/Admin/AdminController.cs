using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.Admin;
using StationManager.Application.DTOs.CarCompany.Admin;
using StationManager.Application.Queries.Admin;

namespace StationManager.API.Controllers.Admin
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICarCompanyQuery _query;

        public AdminController(IMediator mediator, ICarCompanyQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        /// <summary>
        /// Get danh sách nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("get-list-car-company")]
        public async Task<IActionResult> GetListCarCompany(SearchCarCompanyCommand command)
        {
            var response = await _query.SearchCarCompany(command);

            return Ok(new ApiSuccessResponse<List<CarCompanyResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách nhà xe"),
            });
        }

        /// <summary>
        /// Duyệt nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("approve-car-company")]
        public async Task<IActionResult> ApproveStatusCarCompany(ApproveStatusCarCompanyCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = string.Format(CommonResource.Msg_Success, "Duyệt nhà xe"),
            });
        }

        /// <summary>
        /// Đổi trạng thái nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("change-status-car-company")]
        public async Task<IActionResult> ChangeStatusCarCompany(ChangeStatusCarCompanyCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = string.Format(CommonResource.Msg_Success, "Đổi trạng thái nhà xe"),
            });
        }

        /// <summary>
        /// Get chi tiết nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-detail-car-company")]
        public async Task<IActionResult> GetDetailCarCompany(Guid carCompanyId)
        {
            var response = await _query.GetCarCompanyDetail(carCompanyId);

            return Ok(new ApiSuccessResponse<CarCompanyDetailResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết nhà xe"),
            });
        }
    }
}
