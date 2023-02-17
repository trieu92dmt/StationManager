using Core.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.XK;
using MES.Application.DTOs.MES.XK;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class XKController : ControllerBase
    {
        private readonly IXKQuery _query;
        private readonly IMediator _mediator;

        public XKController(IXKQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        /// <summary>
        /// Bảng 1
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-data-input")]
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchXKCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu xuất khác)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-xk")]
        public async Task<IActionResult> GetXKAsync([FromBody] SearchXKCommand command)
        {
            var response = await _query.GetDataXK(command);

            return Ok(new ApiSuccessResponse<List<SearchXKResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu xk
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-xk")]
        public async Task<IActionResult> SaveXKAsync([FromBody] SaveXKCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu dữ liệu xuất khac")
            });
        }

        /// <summary>
        /// Update dữ liệu xk
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-xk")]
        public async Task<IActionResult> UpdateNKAsync([FromBody] UpdateXKCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response.IsSuccess,
                IsSuccess = response.IsSuccess,
                Message = response.Message
            });
        }

        /// <summary>
        /// Get data by reservation and reservation item
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-data-by-res-and-item")]
        public async Task<IActionResult> SaveXKAsync(string reservation, string reservationItem)
        {
            var response = await _query.GetDataByResAndResItem(reservation, reservationItem);

            return Ok(new ApiSuccessResponse<GetDataByRevAndRevItemResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Get Data")
            });
        }
    }
}
