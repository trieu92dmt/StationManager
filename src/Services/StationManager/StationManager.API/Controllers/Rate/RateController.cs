using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.Rating;
using StationManager.Application.Queries.Rates;

namespace StationManager.API.Controllers.Rate
{
    [Route("api/v{version:apiVersion}/Rate/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRateQuery _query;

        public RateController(IMediator mediator, IRateQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        /// <summary>
        /// Tạo đánh giá
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("create-rating")]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<object>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Tạo đánh giá")
            });
        }

        /// <summary>
        /// Kiểm tra quyền đánh giá
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("check-rate-permission")]
        public async Task<IActionResult> CheckRatePermission(Guid accountId, Guid companyId)
        {
            var response = await _query.CheckRatePermission(accountId, companyId);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response.IsSuccess,
                IsSuccess = response.IsSuccess,
                Message = response.Message
            });
        }
    }
}
