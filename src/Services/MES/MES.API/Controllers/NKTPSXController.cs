using ISD.Core.Models;
using MediatR;
using MES.Application.Commands.MES;
using MES.Application.Commands.ReceiptFromProduction;
using MES.Application.DTOs.MES;
using MES.Application.DTOs.MES.NKTPSX;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class NKTPSXController : ControllerBase
    {
        private readonly INKTPSXQuery _query;
        private readonly IMediator _mediator;

        public NKTPSXController(INKTPSXQuery query, IMediator mediator)
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
        ///               "orderType": "string",
        ///               "workOrderFrom": "string",
        ///               "workOrderTo": "string",
        ///               "saleOrderFrom": "string",
        ///               "saleOrderTo": "string",
        ///               "materialFrom": "string",
        ///               "materialTo": "string",
        ///               "scheduledStartFrom": "2023-02-09T04:32:42.459Z",
        ///               "scheduledStartTo": "2023-02-09T04:32:42.459Z",
        ///               "weightHeadCode": "string",
        ///               "weightVoteFrom": "string",
        ///               "weightVoteTo": "string",
        ///               "weightDateFrom": "2023-02-09T04:32:42.459Z",
        ///               "weightDateTo": "2023-02-09T04:32:42.459Z",
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
        ///               "plant": "A200",                                      ---Plant
        ///               "workOrder": "1070000020",                            ---Production Order
        ///               "material": "5310000011",                             ---Material
        ///               "materialDesc": "Gạo 504 25% tấm đã sortex ĐX22",     ---Material Desc
        ///               "sloc": null,                                         ---Stor.Loc
        ///               "batch": null,                                        ---Batch
        ///               "totalQuantity": 100000,                              ---Total Quantity
        ///               "deliveryQuantity": 100000,                           ---Delivery Quantity
        ///               "unit": "KG",                                         ---UoM
        ///               "orderType": "Z007 | ",                               
        ///               "salesOrder": null,
        ///               "saleOrderItem": null
        ///             }
        ///           ],
        ///           "message": null,
        ///           "isSuccess": true,
        ///           "resultsCount": null,
        ///           "recordsTotal": null,
        ///           "pagesCount": null
        ///         }
        ///</remarks>
[HttpPost("get-workorder")]
        public async Task<IActionResult> GetWorkOrderAsync([FromBody] SearchNKTPSXCommand command)
        {
           var response = await _query.GetWO(command);

            return Ok(new ApiSuccessResponse<List<SearchWOResponse>>
            {
                Data = response
            });
        }
    }
}
