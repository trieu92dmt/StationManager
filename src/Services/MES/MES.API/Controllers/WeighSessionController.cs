using Core.Properties;
using MediatR;
using MES.Application.Commands.WeighSession;
using MES.Application.DTOs.MES.WeighSession;
using MES.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WeighSessionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWeighSessionQuery _query;

        public WeighSessionController(IMediator mediator, IWeighSessionQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        /// <summary>
        /// Search hoạt động cân
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-data-scale")]
        public async Task<IActionResult> GetDataWeighSesion([FromBody] SearchWeighSessionCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<IList<WeighSessionResponse>>
            {
                Data = response.Data,
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
            });
        }

        #region Get chi tiết thông tin đợt cân
        /// <summary>
        /// Get chi tiết thông tin đợt cân
        /// </summary>
        /// <param name="WeighSessionCode">ID đợt cân</param>
        /// <returns></returns>
        [HttpGet("get-detail-weigh-session")]
        public async Task<IActionResult> GetDetailWeighSession(string WeighSessionCode)
        {
            var response = await _query.GetDetailWeighSs(WeighSessionCode);

            return Ok(new ApiSuccessResponse<List<DetailWeighSsResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết đợt cân")
            });
        }
        #endregion
    }
}
