using Azure;
using Core.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.WeighSession;
using WeighSession.API.DTOs;
using WeighSession.API.Repositories.Interfaces;

namespace WeighSession.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeighSessionController : ControllerBase
    {
        private readonly IWeighSessionRepository _repository;

        public WeighSessionController(IWeighSessionRepository repository)
        {
            _repository = repository;
        }
        [HttpGet("get-weight-head")]
        public async Task<IActionResult> GetListWeightHeadByPlant(string keyword, string plantCode, string type)
        {
            var result = await _repository.GetWeightHeadAsync(keyword, plantCode, type);
            return Ok(new ApiSuccessResponse<IList<WeightHeadResponse>>
            {
                Data = result,
                IsSuccess = true
            });
        }

        [HttpGet("get-weight-num")]
        public async Task<IActionResult> GetWeightNum(string weightHeadCode)
        {
            var result = await _repository.GetWeighNum(weightHeadCode);
            return Ok(new ApiSuccessResponse<GetWeighNumResponse>
            {
                Data = result,
                IsSuccess = true
            });
        }
        [HttpGet("get-scale-by-code")]
        public async Task<IActionResult> GetScaleByCode(string ScaleCode)
        {
            var result = await _repository.GetScaleByCode(ScaleCode);
            return Ok(new ApiSuccessResponse<ScaleDetailResponse>
            {
                Data = result,
                IsSuccess = true
            });
        }

        [HttpGet("get-weigh-session-by-scale-code")]
        public async Task<IActionResult> GeWeighSessionByScaleCode(string ScaleCode)
        {
            var result = await _repository.GeWeighSessionByScaleCode(ScaleCode);
            return Ok(new ApiSuccessResponse<WeighSessionDetailResponse>
            {
                Data = result,
                IsSuccess = true
            });
        }

        [HttpPost("get-weigh-monitor-by-scale-code")]
        public async Task<IActionResult> SearchScaleMonitor([FromBody] SearchScaleMinitorRequest request)
        {
            var result = await _repository.SearchScaleMonitor(request);
            return Ok(result);
        }
    }
}
