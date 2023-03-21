using DTOs.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.XCK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XCK;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class XCKController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IXCKQuery _query;

        public XCKController(IMediator mediator, IXCKQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        /// <summary>GET Bảng 1</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/get-data-input
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///             
        /// OUT PUT
        /// 
        /// 
        ///</remarks>
        [HttpPost("get-data-input")]
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchXCKCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu xck
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-xck")]
        public async Task<IActionResult> SaveXCKAsync([FromBody] SaveXCKCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu xuất chuyển kho")
            });
        }


        /// <summary>
        /// Bảng 2 (Dữ liệu xuất chuyển kho)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-xck")]
        public async Task<IActionResult> GetXCKAsync([FromBody] SearchXCKCommand command)
        {
            var response = await _query.GetDataXCK(command);

            return Ok(new ApiSuccessResponse<List<SearchXCKResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Update dữ liệu xck
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-xck")]
        public async Task<IActionResult> UpdateXCKAsync([FromBody] UpdateXCKCommand command)
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
        public async Task<IActionResult> GetDataByResAndResItemAsync(string reservation, string reservationItem)
        {
            var response = await _query.GetDataByRsvAndRsvItem(reservation, reservationItem);

            return Ok(new ApiSuccessResponse<GetDataByRsvAndRsvItemResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Get Data")
            });
        }

        #region Get số phiếu cân
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("list-weight-vote")]
        public async Task<IActionResult> GetWeightVoteAsync(string keyword)
        {
            var dropdownList = await _query.GetDropDownWeightVote(keyword);
            return Ok(new ApiSuccessResponse<List<CommonResponse>> { Data = dropdownList });
        }
        #endregion
    }
}
