using Core.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.Scale;
using MES.Application.DTOs.MES.Scale;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ScaleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScaleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Search Data thông tin cân
        /// <summary>
        /// Search Data thông tin cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("search-scale-info")]
        public async Task<IActionResult> SearchScaleInfoAsync([FromBody] SearchScaleCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<ScaleListResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data scale") });
        }
        #endregion

        #region Save Data thông tin cân
        /// <summary>
        /// Save Data thông tin cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("save-scale-info")]
        public async Task<IActionResult> SaveScaleInfoAsync([FromBody] SaveScaleCommad request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool> 
                          { 
                            Data = response.IsSuccess, 
                            Message = response.Message,
                            IsSuccess = response.IsSuccess
                          });
        }
        #endregion

        #region Update Data thông tin cân
        /// <summary>
        /// Update Data thông tin cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("update-scale-info")]
        public async Task<IActionResult> UpdateScaleInfoAsync([FromBody] UpdateScaleCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Chỉnh sửa thông tin cân") });
        }
        #endregion
    }
}
