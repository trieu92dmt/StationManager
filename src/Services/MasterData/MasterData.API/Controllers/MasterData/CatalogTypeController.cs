using DTOs.Models;
using MasterData.Applications.Commands.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasterData.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
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
