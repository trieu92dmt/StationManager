using Core.Properties;
using MediatR;
using MES.Application.Commands.NCK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NCK;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NCKController : ControllerBase
    {
        private readonly INCKQuery _query;
        private readonly IMediator _mediator;

        public NCKController(INCKQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchNCKCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<IList<GetInputDataResponse>>
            {
                Data = response.Data,
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
            });
        }

        /// <summary>
        /// Save dữ liệu nck
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nck")]
        public async Task<IActionResult> SaveNCKAsync([FromBody] SaveNCKCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu dữ liệu nhập chuyển kho")
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu nhập chuyển kho)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-nck")]
        public async Task<IActionResult> GetNCKAsync([FromBody] SearchNCKCommand command)
        {
            var response = await _query.GetDataNCK(command);

            return Ok(new ApiSuccessResponse<IList<SearchNCKResponse>>
            {
                Data = response.Data,
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
            });
        }

        /// <summary>
        /// Update dữ liệu nck
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-nck")]
        public async Task<IActionResult> UpdateXCKAsync([FromBody] UpdateNCKCommand command)
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
        /// Get data by mat doc and mat doc item
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-data-by-matdoc-and-matdoc-item")]
        public async Task<IActionResult> GetDataByMatDocAndMatDocItemAsync(string matdoc, string matdocItem)
        {
            var response = await _query.GetDataByMatDocAndMatDocItem(matdoc, matdocItem);

            return Ok(new ApiSuccessResponse<GetDataByMatDocAndMatDocItemResponse>
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
