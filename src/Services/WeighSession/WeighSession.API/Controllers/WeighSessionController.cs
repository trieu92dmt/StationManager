using DTOs.Models;
using DTOs.WeighSession;
using Microsoft.AspNetCore.Mvc;
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
    }
}
