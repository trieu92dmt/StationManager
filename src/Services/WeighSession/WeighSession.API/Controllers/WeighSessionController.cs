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
        [HttpGet("get-weigh-session")]
        public async Task<IActionResult> GetListWeightHeadByPlant(string keyword, string plantCode, string type)
        {
            var result = await _repository.GetWeighSessionAsync(keyword, plantCode, type);
            return Ok(result);
        }
    }
}
