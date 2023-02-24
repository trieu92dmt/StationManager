using Core.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.NHLT;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NHLT;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NHLTController : ControllerBase
    {
        private readonly INHLTQuery _query;
        private readonly IMediator _mediator;

        public NHLTController(INHLTQuery query, IMediator mediator)
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchNHLTCommand command)
        {
            var response = await _query.GetInputDatas(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu nhlt
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nhlt")]
        public async Task<IActionResult> SaveNHLTAsync([FromBody] SaveNHLTCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu dữ liệu nhập hàng loại T")
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu nhập hàng loại T)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-nhlt")]
        public async Task<IActionResult> GetNHLTAsync([FromBody] SearchNHLTCommand command)
        {
            var response = await _query.GetDataNHLT(command);

            return Ok(new ApiSuccessResponse<List<SearchNHLTResponse>>
            {
                Data = response
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
