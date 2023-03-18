using Core.Models;
using Core.SeedWork;
using MediatR;
using MES.Application.Commands.NCK;
using MES.Application.Commands.WeighSession;
using MES.Application.DTOs.MES.WeighSession;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WeighSessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeighSessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("get-data-scale")]
        public async Task<IActionResult> GetDataInputAsync([FromBody] SearchWeighSessionCommand command)
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
    }
}
