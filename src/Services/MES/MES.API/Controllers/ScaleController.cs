using Core.Properties;
using MediatR;
using MES.Application.Commands.Scale;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.WeighSession;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ScaleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IScaleQuery _query;

        public ScaleController(IMediator mediator, IScaleQuery query)
        {
            _mediator = mediator;
            _query = query;
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

            return Ok(new ApiSuccessResponse<ScaleListResponse> { 
                Data = response, 
                Message = string.Format(CommonResource.Msg_Success, "Get data scale"),
                RecordsTotal = response.TotalResultsCount,
                ResultsCount = response.FilterResultsCount,
                PagesCount = response.TotalPagesCount
            });
        }
        #endregion

        #region Get chi tiết thông tin cân
        /// <summary>
        /// Get chi tiết thông tin cân
        /// </summary>
        /// <param name="ScaleId"></param>
        /// <returns></returns>
        [HttpGet("get-detail-scale")]
        public async Task<IActionResult> GetScaleInfoAsync(string ScaleCode)
        {
            var response = await _query.GetScaleDetail(ScaleCode);

            return Ok(new ApiSuccessResponse<ScaleDetailResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết cân")
            });
        }
        #endregion

        #region Save Data thông tin cân
        /// <summary>
        /// Save Data thông tin cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("save-scale-info")]
        public async Task<IActionResult> SaveScaleInfoAsync([FromBody] SaveScaleCommand request)
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

        #region Báo cáo tình trạng cân
        /// <summary>
        /// Báo cáo tình trạng cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("scale-status-report")]
        public async Task<IActionResult> ScaleStatusReport([FromBody] ScaleStatusReportCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<List<ScaleStatusReportResponse>> { Data = response, IsSuccess = true });
        }
        #endregion
    }
}
