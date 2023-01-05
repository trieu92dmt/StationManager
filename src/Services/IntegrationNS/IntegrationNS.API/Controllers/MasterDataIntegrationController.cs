using IntegrationNS.Application.Commands;
using IntegrationNS.Application.DTOs;
using ISD.Core.Models;
using ISD.Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationNS.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class MasterDataIntegrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MasterDataIntegrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Tích hợp Vendor

        /// <summary>Tích hợp Vendor</summary>
        /// <returns></returns>
        /// <remarks>
        /// Mẫu request
        /// 
        /// POST
        /// 
        ///     Url: /api/v{version}/MasterDataIntegration/vendor
        ///     Params: 
        ///             + version : 1
        ///     Body: 
        ///
        ///
        ///                {
        ///                  "vendor": "string",
        ///                  "vendorName": "string",
        ///                  "country": "string"
        ///                }
        /// OUT PUT
        /// 
        ///                {
        ///                     "code": 200,
        ///                     "message": "Tích hợp Vendor thành công.",
        ///                     "data": true
        ///                }
        /// </remarks>
        [HttpPost("vendor")]
        public async Task<IActionResult> VendorIntegration([FromBody] VendorIntegrationCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<IntegrationNSResponse> { Data = response, Message = string.Format(CommonResource.Msg_Success, "Tích hợp Vendor") });
        }
        #endregion
    }
}
