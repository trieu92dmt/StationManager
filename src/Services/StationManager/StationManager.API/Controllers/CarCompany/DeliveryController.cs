using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.CarCompany.CarManager;
using StationManager.Application.Commands.CarCompany.DeliveryManager;
using StationManager.Application.DTOs.CarCompany;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using StationManager.Application.DTOs.CarCompany.Request;
using StationManager.Application.Queries.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDeliveryQuery _query;

        public DeliveryController(IMediator mediator, IDeliveryQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        /// <summary>
        /// Thêm đơn hàng
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("add-delivery")]
        public async Task<IActionResult> AddDelivery(AddDeliveryCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thêm đơn hàng")
            });
        }

        /// <summary>
        /// Nhận hàng hàng
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("update-status-delivery")]
        public async Task<IActionResult> UpdateStatusDelivery(UpdateStatusDeliveryCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Nhận hàng")
            });
        }

        /// <summary>
        /// Get danh sách đơn vận chuyển
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("get-list-delivery")]
        public async Task<IActionResult> GetListDelivery(GetListDeliveryRequest request)
        {
            var response = await _query.GetListDelivery(request);

            return Ok(new ApiSuccessResponse<List<DeliveryResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh chuyến xe")
            });
        }
    }
}
