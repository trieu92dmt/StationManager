using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.CarCompany.Report;
using StationManager.Application.DTOs.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Báo cáo doanh thu
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("revenue-report")]
        public async Task<IActionResult> GetRevenueReport(RevenueReportCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<List<RevenueReportResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thống kê doanh thu")
            });
        }

        /// <summary>
        /// Báo cáo tần suất
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("frequency-report")]
        public async Task<IActionResult> GetFrequencyReport(FrequencyReportCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<List<FrequencyReportResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thống kê tần suất")
            });
        }
    }
}
