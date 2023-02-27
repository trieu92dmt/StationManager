using Core.Models;
using MediatR;
using MES.Application.Commands.XNVLGC;
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
    }
}
