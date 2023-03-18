using Core.Models;
using MES.Application.DTOs.Common;
using MES.Application.Queries;
using MES.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers.Utilities
{
    [Route("api/v{version:apiVersion}/Utilities/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CommonDateController : ControllerBase
    {
        private readonly ICommonQuery _commonQuery;
        private readonly ICommonDateService _commonDateService;

        public CommonDateController(ICommonQuery commonQuery, ICommonDateService commonDateService)
        {
            _commonQuery = commonQuery;
            _commonDateService = commonDateService;
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

        //[HttpGet("get-common-date")]
        //public async Task<IActionResult> GetCommonDate()
        //{
        //    var response = await _commonQuery.GetCommonDate();

        //    return Ok(new ApiSuccessResponse<List<CommonResponse>>()
        //    {
        //        Data = response
        //    });

        //}
    }
}
