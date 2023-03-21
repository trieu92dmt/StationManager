using Core.Properties;
using MediatR;
using MES.Application.Commands.NK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NK;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKController : ControllerBase
    {
        private readonly INKQuery _query;
        private readonly IMediator _mediator;

        public NKController(INKQuery query, IMediator mediator)
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchNKCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu nhập khác)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-nk")]
        public async Task<IActionResult> GetNKAsync([FromBody] SearchNKCommand command)
        {
            var response = await _query.SearchNK(command);

            return Ok(new ApiSuccessResponse<List<SearchNKResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu nk
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nk")]
        public async Task<IActionResult> SaveNKAsync([FromBody] SaveNKCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu dữ liệu nhập khac")
            });
        }

        /// <summary>
        /// Update dữ liệu nk
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-nk")]
        public async Task<IActionResult> UpdateNKAsync([FromBody] UpdateNKCommand command)
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
        /// Get unit by plant and material
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-unit-by-material-and-plant")]
        public async Task<IActionResult> SaveNKAsync(string material, string plant)
        {
            var response = await _query.GetUnitByMaterialAndPlant(material, plant);

            return Ok(new ApiSuccessResponse<string>
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
