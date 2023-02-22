using Core.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.Commands.XKLXH;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XKLXH;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class XKLXHController : ControllerBase
    {
        private readonly IXKLXHQuery _query;
        private readonly IMediator _mediator;

        public XKLXHController(IXKLXHQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchXKLXHCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu xklxh
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-xklxh")]
        public async Task<IActionResult> SaveXKLXHAsync([FromBody] SaveXKLXHCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu xuất kho theo lệnh xuất hàng")
            });
        }


        /// <summary>
        /// Bảng 2 (Dữ liệu xuất kho theo lệnh xuất hàng)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-xklxh")]
        public async Task<IActionResult> GetXKLXHAsync([FromBody] SearchXKLXHCommand command)
        {
            var response = await _query.GetDataXKLXH(command);

            return Ok(new ApiSuccessResponse<List<SearchXKLXHResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Update dữ liệu xklxh
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-xklxh")]
        public async Task<IActionResult> UpdateXKLXHAsync([FromBody] UpdateXKLXHCommand command)
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
        /// Lấy dữ liệu theo od và od item
        /// </summary>
        /// <param name="od"></param>
        /// <param name="odItem"></param>
        /// <returns></returns>
        [HttpGet("get-data-by-od-oditem")]
        public async Task<IActionResult> GetDataByODAndODItem(string od, string odItem)
        {
            var response = await _query.GetDataByODODItem(od, odItem);

            return Ok(new ApiSuccessResponse<GetDataByODODItemResponse>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Lấy data")
            });
        }
    }
}
