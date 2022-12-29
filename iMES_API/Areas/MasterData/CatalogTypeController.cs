using ISD.API.Applications.Commands.Catalog;
using ISD.API.Applications.Commands.Company;
using ISD.API.Applications.DTOs.Company;
using ISD.API.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace TLG_API.Areas.MasterData
{
    [Route("api/v{version:apiVersion}/MasterData/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CatalogTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-catalog-type")]
        public async Task<IActionResult> CreateCatalogType(CatalogTypeCreateCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>()
            {
                IsSuccess= true,
                Data = response
            });
        }
    }
}
