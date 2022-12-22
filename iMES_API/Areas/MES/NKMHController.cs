using ISD.API.Applications.Commands.MES;
using ISD.API.Core;
using ISD.API.Core.Extensions;
using ISD.API.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TLG_API.Areas.MES
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKMHController : ControllerBaseAPI
    {
        private readonly IMediator _mediator;

        public NKMHController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Save nhập kho mua hàng
        /// <summary>
        /// Save nhập kho mua hàng
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nkmh")]
        public async Task<IActionResult> SaveNKMHAsync([FromBody] SaveNKMHCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Lưu NKMH") });
        }
        #endregion
    }
}
