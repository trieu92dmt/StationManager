using MediatR;
using MES.Application.Commands.WeighSessionFactory;
using MES.Application.DTOs.Common;
using MES.Application.Queries;
using MES.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MES.API.Controllers.Utilities
{
    [Route("api/v{version:apiVersion}/Utilities/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CommonDateController : ControllerBase
    {
        private readonly ICommonQuery _commonQuery;
        private readonly ICommonDateService _commonDateService;
        private readonly IMediator _mediator;

        public CommonDateController(ICommonQuery commonQuery, ICommonDateService commonDateService, IMediator mediator)
        {
            _commonQuery = commonQuery;
            _commonDateService = commonDateService;
            _mediator = mediator;
        }

        [HttpGet("GetDateByCommonDate")]
        public IActionResult Get(string CommonDate)
        {
            DateTime? fromDate;
            DateTime? toDate;

            _commonDateService.GetDateBy(CommonDate, out fromDate, out toDate);

            return Ok(new
            {
                FromDate = string.Format("{0:yyyy-MM-dd}", fromDate),
                ToDate = string.Format("{0:yyyy-MM-dd}", toDate)
            });
        }

        [HttpGet("get-common-date")]
        public async Task<IActionResult> GetCommonDate()
        {
            var response = await _commonQuery.GetCommonDate();

            return Ok(new ApiSuccessResponse<List<CommonResponse>>()
            {
                Data = response
            });

        }

        [HttpPost("factory-scale")]
        public async Task<IActionResult> FactoryScale(FactoryCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(new ApiSuccessResponse<bool>()
            {
                Data = response
            });

        }
    }
}
