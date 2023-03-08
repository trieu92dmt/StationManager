using Core.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.XKLXH;
using MES.Application.Commands.XNVLGC;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XNVLGC;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class XNVLGCController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IXNVLGCQuery _query;

        public XNVLGCController(IMediator mediator, IXNVLGCQuery query)
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchXNVLGCCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu xuất nvl gia công)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-xnvlgc")]
        public async Task<IActionResult> GetXNVLGCAsync([FromBody] SearchXNVLGCCommand command)
        {
            var response = await _query.GetDataXNVLGC(command);

            return Ok(new ApiSuccessResponse<List<SearchXNVLGCResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu xnvlgc
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-xnvlgc")]
        public async Task<IActionResult> SaveXNVLGCAsync([FromBody] SaveXNVLGCCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu xuất nguyên vật liệu gia công")
            });
        }

        /// <summary>
        /// Update dữ liệu xnvlgc
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-xnvlgc")]
        public async Task<IActionResult> UpdateXNVLGCAsync([FromBody] UpdateXNVLGCCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response.IsSuccess,
                IsSuccess = response.IsSuccess,
                Message = response.Message
            });
        }

        #region Get dropdown data by component
        /// <summary>
        /// Get dropdown data by component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="componentItem"></param>
        /// <returns></returns>
        [HttpGet("get-data-by-component")]
        public async Task<IActionResult> GetDataByComponentAsync(string keyword, string component, string componentItem)
        {
            var dropdownList = await _query.GetListPOByComponent(keyword ,component, componentItem);
            return Ok(new ApiSuccessResponse<List<GetDataByComponent>> { Data = dropdownList });
        }
        #endregion

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
