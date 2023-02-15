using Core.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.NKDCNB;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NKDCNB;
using MES.Application.DTOs.MES.OutboundDelivery;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKDCNBController : ControllerBase
    {
        private readonly INKDCNBQuery _query;
        private readonly IMediator _mediator;

        public NKDCNBController(INKDCNBQuery query, IMediator mediator)
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchNKDCNBCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu nhập kho điều chuyển nội bộ)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-nkdcnb")]
        public async Task<IActionResult> GetNKDCNBAsync([FromBody] SearchNKDCNBCommand command)
        {
            var response = await _query.GetNKDCNB(command);

            return Ok(new ApiSuccessResponse<List<SearchNKDCNBResponse>>
            {
                Data = response
            });
        }


        /// <summary>
        /// Save dữ liệu nkdcnb
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nkdcnb")]
        public async Task<IActionResult> SaveNKDCNBAsync([FromBody] SaveNKDCNBCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu nhập kho điều chuyển nội bộ")
            });
        }

        /// <summary>
        /// Update dữ liệu nkdcnb
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-nkdcnb")]
        public async Task<IActionResult> UpdateGoodsReturnAsync([FromBody] UpdateNKDCNBCommand command)
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
            var response = await _query.GetDataByOdAndOdItem(od, odItem);

            return Ok(new ApiSuccessResponse<GetDataByOdAndOdItem>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Lấy data")
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
