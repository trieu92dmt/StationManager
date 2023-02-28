using Core.Models;
using Core.Properties;
using MediatR;
using MES.Application.Commands.NNVL;
using MES.Application.DTOs.MES.NNVL;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NNVLController : ControllerBase
    {
        private readonly INNVLQuery _query;
        private readonly IMediator _mediator;

        public NNVLController(INNVLQuery query, IMediator mediator)
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchNNVLCommand command)
        {
            var response = await _query.GetInputDatas(command);

            return Ok(new ApiSuccessResponse<List<GetInputDataResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu nnvlgc
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nnvlgc")]
        public async Task<IActionResult> SaveNNVLAsync([FromBody] SaveNNVLCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu dữ liệu nhập nguyên vật liệu gia công")
            });
        }

        /// <summary>
        /// Bảng 2 (Dữ liệu nhập nnvlgc)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-nnvlgc")]
        public async Task<IActionResult> GetNNVLAsync([FromBody] SearchNNVLCommand command)
        {
            var response = await _query.GetDataNNVLGC(command);

            return Ok(new ApiSuccessResponse<List<SearchNNVLResponse>>
            {
                Data = response
            });
        }
    }
}
