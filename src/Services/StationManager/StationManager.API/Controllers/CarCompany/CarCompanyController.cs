using Core.Properties;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.CarCompany;
using StationManager.Application.Commands.CarCompany.CarManager;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.Queries.CarCompany;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CarCompanyController : ControllerBase
    {
        private readonly ICarCompanyQuery _query;
        private readonly IMediator _mediator;

        public CarCompanyController(ICarCompanyQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        /// <summary>
        /// Get chi tiết nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-detail-car-company")]
        public async Task<IActionResult> GetDetailCarCompany(Guid AccountId)
        {
            var response = await _query.GetDetailCarCompany(AccountId);

            return Ok(new ApiSuccessResponse<DetailCarCompanyResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết nhà xe")
            });
        }

        /// <summary>
        /// Chỉnh sửa chi tiết nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("update-car-company-info")]
        public async Task<IActionResult> UpdateCarCompanyInfo([FromForm] UpdateCarCompanyInfoCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Code = response.Code,
                IsSuccess = response.IsSuccess,
                Data = response.IsSuccess,
                Message = response.Message
            });
        }


        /// <summary>
        /// Đăng ký nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("car-company-register")]
        public async Task<IActionResult> CarCompanyRegister([FromBody] CarCompanyRegisterCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Đăng ký nhà xe")
            });
        }

        /// <summary>
        /// Get danh sách nhà xe
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-list-car-company")]
        public async Task<IActionResult> GetListCarCompany(int page)
        {
            var response = await _query.GetListCarCompany(page);

            return Ok(new ApiSuccessResponse<IList<CarCompanyItemResponse>>
            {
                Data = response.Data,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách nhà xe"),
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
            });
        }

        /// <summary>
        /// Get chi tiết nhà xe phía người dùng
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-detail-car-company-by-user-side")]
        public async Task<IActionResult> GetDetailCarCompanyByUserSide(Guid CarCompanyId)
        {
            var response = await _query.GetDetailCarCompanyByUserSide(CarCompanyId);

            return Ok(new ApiSuccessResponse<DetailCarCompanyResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết nhà xe")
            });
        }
    }
}
