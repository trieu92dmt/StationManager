using Core.Properties;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.Queries.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageQuery _query;

        public PackageController(IPackageQuery query)
        {
            _query = query;
        }

        /// <summary>
        /// Get list package
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-list-package")]
        public async Task<IActionResult> GetListPackage()
        {
            var response = await _query.GetListPackage();

            return Ok(new ApiSuccessResponse<List<PackageResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách gói nhà xe")
            });
        }
    }
}
