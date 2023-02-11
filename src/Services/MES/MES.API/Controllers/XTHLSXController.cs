using ISD.Core.Models;
using ISD.Core.Properties;
using MediatR;
using MES.Application.Commands.XTHLSX;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XTHLSX;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class XTHLSXController : ControllerBase
    {
        private readonly IXTHLSXQuery _query;
        private readonly IMediator _mediator;

        public XTHLSXController(IXTHLSXQuery query, IMediator mediator)
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
        ///             {
        ///               "plant": "string",
        ///               "material": "string",
        ///               "componentFrom": "string",
        ///               "componentTo": "string",
        ///               "workorderFrom": "string",
        ///               "workorderTo": "string",
        ///               "salesOrderFrom": "string",
        ///               "salesOrderTo": "string",
        ///               "orderTypeFrom": "string",
        ///               "orderTypeTo": "string",
        ///               "scheduledStartFrom": "2023-02-10T04:42:36.754Z",
        ///               "scheduledStartTo": "2023-02-10T04:42:36.754Z",
        ///               "weightHeadCode": "string",
        ///               "weightVotes": [
        ///                 "string"
        ///               ],
        ///               "weightDateFrom": "2023-02-10T04:42:36.754Z",
        ///               "weightDateTo": "2023-02-10T04:42:36.754Z",
        ///               "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///             }
        ///             
        /// OUT PUT
        /// 
        /// 
        ///         {
        ///           "code": 200,
        ///           "data": [
        ///             {
        ///               "indexKey": 1,
        ///               "plant": "A200",
        ///               "workOrder": "1070000020",
        ///               "material": "5310000011",
        ///               "materialDesc": "Gạo 504 25% tấm đã sortex ĐX22",
        ///               "component": "5210000022",
        ///               "componentDesc": "Gạo lứt 504 25% tấm ĐX22",
        ///               "salesOrder": "",
        ///               "orderType": "Z007 | Điều chuyển NB STO",
        ///               "scheduleStartTime": "2023-02-06T00:00:00",
        ///               "scheduleFinishTime": "2023-02-06T00:00:00",
        ///               "sloc": "",
        ///               "batch": "",
        ///               "requirementQty": 50000,
        ///               "withdrawQty": 50000,
        ///               "unit": "KG"
        ///             }
        ///           ],
        ///           "message": null,
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        ///</remarks>
        [HttpPost("get-data-input")]
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchXTHLSXCommand command)
        {
            var response = await _query.GetInputData(command);

            return Ok(new ApiSuccessResponse<List<GetDataInputResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Save dữ liệu ntpppp
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("save-nkpppp")]
        public async Task<IActionResult> SaveNKPPPPAsync([FromBody] SaveXTHLSXCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu xuất tiêu hao theo lệnh sản xuất")
            });
        }


        /// <summary>
        /// Bảng 2 (Dữ liệu xuất tiêu hao theo lsx)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("get-xthlsx")]
        public async Task<IActionResult> GetXTHLSXAsync([FromBody] SearchXTHLSXCommand command)
        {
            var response = await _query.GetXTHLSX(command);

            return Ok(new ApiSuccessResponse<List<SearchXTHLSXResponse>>
            {
                Data = response
            });
        }

        /// <summary>
        /// Update dữ liệu xthlsx
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("update-xthlsx")]
        public async Task<IActionResult> UpdateNKPPPPAsync([FromBody] UpdateXTHLSXCommand command)
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
        /// Lấy dữ liệu theo wo và nvl
        /// </summary>
        /// <param name="workorder"></param>
        /// <returns></returns>
        [HttpGet("get-data-by-wo-and-component")]
        public async Task<IActionResult> GetDataByWoAndComponent(string workorder, string component)
        {
            var response = await _query.GetDataByWoAndComponent(workorder, component);

            return Ok(new ApiSuccessResponse<GetDataByWoAndComponentResponse>
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
