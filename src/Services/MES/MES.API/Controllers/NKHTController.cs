﻿using Core.Properties;
using MediatR;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.OutboundDelivery;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKHTController : ControllerBase
    {
        private readonly IOutboundDeliveryQuery _query;
        private readonly IMediator _mediator;

        public NKHTController(IOutboundDeliveryQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        /// <summary>
        /// Bảng 1
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-outbound-delivery")]
        public async Task<IActionResult> GetOutboundDeliveryAsync([FromBody] SearchOutboundDeliveryCommand command)
        {
            var response = await _query.GetOutboundDelivery(command);

            return Ok(new ApiSuccessResponse<List<OutboundDeliveryResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu nhập kho hàng trả)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-goods-return")]
        public async Task<IActionResult> GetGoodsReturnAsync([FromBody] SearchOutboundDeliveryCommand command)
        {
            var response = await _query.GetGoodsReturn(command);

            return Ok(new ApiSuccessResponse<List<GoodsReturnResponse>>
            {
                Data = response
            });
        }


        /// <summary>
        /// Save dữ liệu nkht
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-goods-return")]
        public async Task<IActionResult> SaveGoodsReturnAsync([FromBody] SaveGoodsReturnCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu nhập kho hàng trả")
            });
        }

        /// <summary>
        /// Update dữ liệu nkht
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-goods-return")]
        public async Task<IActionResult> UpdateGoodsReturnAsync([FromBody] UpdateGoodsReturnCommand command)
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
