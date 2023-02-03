using ISD.Core.Models;
using ISD.Core.Properties;
using MediatR;
using MES.Application.Commands.MES;
using MES.Application.Commands.TruckInfo;
using MES.Application.DTOs.MES.TruckInfo;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TruckWeightController : ControllerBase
    {
        private readonly ITruckInfoQuery _truckInfoQuery;
        private readonly IMediator _mediator;

        public TruckWeightController(ITruckInfoQuery truckInfoQuery, IMediator mediator)
        {
            _truckInfoQuery = truckInfoQuery;
            _mediator = mediator;
        }

        #region Search Data thông tin xe tải
        /// <summary>
        /// Search Data thông tin xe tải
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("search-truck-weight-info")]
        public async Task<IActionResult> SearchTruckWeightInfoAsync([FromBody] SearchTruckInfoCommand command)
        {
            var response = await _truckInfoQuery.SearchTruckInfo(command);

            return Ok(new ApiSuccessResponse<List<SearchTruckInfoResponse>> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Get data truck info") });
        }
        #endregion

        #region Save Data thông tin xe tải
        /// <summary>
        /// Save Data thông tin xe tải
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-truck-weight-info")]
        public async Task<IActionResult> SaveTruckWeightInfoAsync([FromBody] SaveTruckInfoCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Save data truck info") });
        }
        #endregion
    }
}
