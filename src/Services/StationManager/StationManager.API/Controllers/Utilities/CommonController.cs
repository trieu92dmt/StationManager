using Core.Properties;
using Infrastructure.Models;
using MES.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.DTOs.Common;
using StationManager.Application.Queries.Common;

namespace StationManager.API.Controllers.Utilities
{
    [Route("api/v{version:apiVersion}/Utilities/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonQuery _query;

        public CommonController(ICommonQuery query)
        {
            _query = query;
        }

        /// <summary>
        /// Danh sách tỉnh thành phố
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-province")]
        public async Task<IActionResult> ListProvince()
        {
            var response = await _query.ListProvince();

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tỉnh thành phố")
            });
        }

        /// <summary>
        /// Danh sách tỉnh thành phố/ quận huyện
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-province-district")]
        public async Task<IActionResult> ListProvinceDistrict(string keyword)
        {
            var response = await _query.ListProvinceDistrict(keyword);

            return Ok(new ApiSuccessResponse<List<ProviceDistrictResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tỉnh thành phố/ quận huyện")
            });
        }

        /// <summary>
        /// Danh sách quận huyện theo thành phố
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-district-by-province")]
        public async Task<IActionResult> ListDistrictByProvince(string keyword)
        {
            var response = await _query.ListDistrictByProvince(keyword);

            return Ok(new ApiSuccessResponse<List<DistrictByProvinceResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tỉnh thành phố/ quận huyện")
            });
        }

        /// <summary>
        /// Danh sách tỉnh thành phố
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-province-v2")]
        public async Task<IActionResult> ListProvinceV2(string keyword)
        {
            var response = await _query.ListProvinceV2(keyword);

            return Ok(new ApiSuccessResponse<List<Common4Response>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tỉnh thành phố")
            });
        }

        /// <summary>
        /// Danh sách tất cả tỉnh thành phố
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-all-province")]
        public async Task<IActionResult> ListAllProvince()
        {
            var response = await _query.ListAllProvince();

            return Ok(new ApiSuccessResponse<List<Common4Response>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tỉnh thành phố")
            });
        }

        /// <summary>
        /// Danh sách loại xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-cartype")]
        public async Task<IActionResult> ListCarType(string keyword, Guid? accountId)
        {
            var response = await _query.ListCarType(keyword, accountId);

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách loại xe")
            });
        }

        /// <summary>
        /// Danh sách mã số xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-car-number")]
        public async Task<IActionResult> ListCarNumber(string keyword, Guid? accountId)
        {
            var response = await _query.ListCarNumber(keyword, accountId);

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách mã số xe")
            });
        }

        /// <summary>
        /// Danh sách vị trí làm việc
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-position")]
        public async Task<IActionResult> ListPosition()
        {
            var response = await _query.ListPosition();

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách vị trí làm việc")
            });
        }

        /// <summary>
        /// Danh sách tài xế
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-driver")]
        public async Task<IActionResult> ListDriver(Guid? accountId)
        {
            var response = await _query.ListDriver(accountId);

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tài xế")
            });
        }

        /// <summary>
        /// Danh sách tuyến xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-route")]
        public async Task<IActionResult> ListRoute(Guid? accountId)
        {
            var response = await _query.ListRoute(accountId);

            return Ok(new ApiSuccessResponse<List<Common2Response>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tuyến xe")
            });
        }

        /// <summary>
        /// Danh sách chuyến xe theo tuyến
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-trip-by-route-id")]
        public async Task<IActionResult> ListTripByRouteId(Guid routeId, DateTime startDate)
        {
            var response = await _query.ListTripByRouteId(routeId, startDate);

            return Ok(new ApiSuccessResponse<List<Common2Response>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách chuyến xe")
            });
        }

        /// <summary>
        /// Danh sách nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-carCompany")]
        public async Task<IActionResult> ListCarCompany()
        {
            var response = await _query.ListCarCompany();

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách nhà xe")
            });
        }

        /// <summary>
        /// Danh sách tên nhà xe -- admin
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-car-company-admin")]
        public async Task<IActionResult> ListCarCompanyAdmin(string keyword)
        {
            var response = await _query.ListCarCompanyAdmin(keyword);

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tên nhà xe")
            });
        }

        /// <summary>
        /// Danh sách vai trò
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("list-role")]
        public async Task<IActionResult> ListRole()
        {
            var response = await _query.ListRole();

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách vai trò")
            });
        }
    }
}
