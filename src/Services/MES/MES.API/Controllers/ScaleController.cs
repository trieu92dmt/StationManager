using DTOs.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.Scale;
using MES.Application.DTOs.MES.Scale;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;

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
                RecordsTotal = response.PagingRep.TotalResultsCount,
                ResultsCount = response.PagingRep.FilterResultsCount,
                PagesCount = response.PagingRep.TotalPagesCount
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
        public async Task<IActionResult> GetScaleInfoAsync(Guid ScaleId)
        {
            var response = await _query.GetScaleDetail(ScaleId);

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
    }
}
