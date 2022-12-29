using ISD.API.Applications.Commands.MES;
using ISD.API.Applications.DTOs.MES;
using ISD.API.Applications.Queries.MES.NKHMH;
using ISD.API.Core;
using ISD.API.Core.Extensions;
using ISD.API.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLG_API.Areas.MES
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKMHController : ControllerBaseAPI
    {
        private readonly IMediator _mediator;
        private readonly INKMHQuery _query;

        public NKMHController(IMediator mediator, INKMHQuery query)
        {
            _mediator = mediator;
            _query = query;
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

        /// <summary>
        /// GET nhập kho mua hàng
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-nkmh")]
        public async Task<IActionResult> GetNKMHAsync([FromBody] GetNKMHCommand command)
        {
            var response = await _query.GetNKMHAsync(command);

            return Ok(new ApiSuccessResponse<IList<NKMHMesResponse>>
            {
                Data = response.Data,
                //RecordsTotal = response.Paging.TotalCount,
                //PagesCount = response.Paging.TotalPages,
                //ResultsCount = response.Paging.PageSize
            });
        }
    }
}
