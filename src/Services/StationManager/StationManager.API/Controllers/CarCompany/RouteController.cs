using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.RouteManager;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;
using StationManager.Application.Queries.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteQuery _query;
        private readonly IMediator _mediator;

        public RouteController(IRouteQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        /// <summary>
        /// Get danh sách tuyến xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("get-list-route")]
        public async Task<IActionResult> GetListRoute(GetListRouteRequest request)
        {
            var response = await _query.GetListRoute(request);

            return Ok(new ApiSuccessResponse<List<RouteResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh tuyến xe")
            });
        }

        /// <summary>
        /// Thêm tuyến
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("add-route")]
        public async Task<IActionResult> AddRoute(AddRouteCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                IsSuccess = response.IsSuccess,
                //Data = response,
                Message = response.Message
            });
        }

        /// <summary>
        /// Get chi tiết 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-detail-route")]
        public async Task<IActionResult> GetDetailRoute(Guid routeId)
        {
            var response = await _query.GetDetailRoute(routeId);

            return Ok(new ApiSuccessResponse<DetailRouteResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết tuyến")
            });
        }

        /// <summary>
        /// Cập nhật thông tin
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("update-route")]
        public async Task<IActionResult> UpdateRoute(UpdateRouteCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật thông tin tuyến")
            });
        }

        /// <summary>
        /// Xóa 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpDelete("remove-route/{routeId}")]
        public async Task<IActionResult> RemoveRoute(Guid routeId)
        {
            var request = new RemoveRouteCommand();
            request.RouteId = routeId;
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Xóa")
            });
        }
    }
}
