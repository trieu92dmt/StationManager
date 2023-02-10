using ISD.Core.Models;
using ISD.Core.Properties;
using MediatR;
using MES.Application.Commands.NKPPPP;
using MES.Application.Commands.OutboundDelivery;
using MES.Application.Commands.ReceiptFromProduction;
using MES.Application.DTOs.MES.NKPPPP;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKPPPPController : ControllerBase
    {
        private readonly INKPPPPQuery _query;
        private readonly IMediator _mediator;

        public NKPPPPController(INKPPPPQuery query, IMediator mediator)
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
        ///     Url: /api/v{version}/MasterDataIntegration/update-nkmh
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
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchNKPPPPCommand command)
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
        public async Task<IActionResult> SaveNKPPPPAsync([FromBody] SaveNKPPPPCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lưu nhập kho phụ phẩm phế phẩm")
            });
        }

    }
}
