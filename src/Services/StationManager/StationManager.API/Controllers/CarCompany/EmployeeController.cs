using Core.Properties;
using MediatR;
using MES.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using StationManager.Application.Commands.CarCompany.CarManager;
using StationManager.Application.Commands.CarCompany.EmployeeManager;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;
using StationManager.Application.Queries.CarCompany;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeQuery _query;
        private readonly IMediator _mediator;

        public EmployeeController(IEmployeeQuery query, IMediator mediator)
        {
            _query = query;
            _mediator = mediator;
        }

        /// <summary>
        /// Get danh sách nhân viên
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("get-list-employee")]
        public async Task<IActionResult> GetListEmployee(GetListEmployeeRequest request)
        {
            var response = await _query.GetListEmployee(request);

            return Ok(new ApiSuccessResponse<List<EmployeeResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách nhân viên")
            });
        }

        /// <summary>
        /// Get dropdown mã nhân viên
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-list-employee-code")]
        public async Task<IActionResult> GetListEmployeeCode(Guid accountId)
        {
            var response = await _query.GetListEmployeeCode(accountId);

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách mã nhân viên")
            });
        }

        /// <summary>
        /// Get dropdown tên nhân viên
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-list-employee-name")]
        public async Task<IActionResult> GetListEmployeeName(Guid accountId)
        {
            var response = await _query.GetListEmployeeName(accountId);

            return Ok(new ApiSuccessResponse<List<CommonResponse>>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy danh sách tên nhân viên")
            });
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpDelete("remove-employee/{employeeId}")]
        public async Task<IActionResult> RemoveRoute(Guid employeeId)
        {
            var request = new RemoveEmployeeCommand();
            request.EmployeeId = employeeId;
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Xóa nhân viên")
            });
        }

        /// <summary>
        /// Get chi tiết nhân viên
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("get-detail-employee")]
        public async Task<IActionResult> GetDetailEmployee(Guid employeeId)
        {
            var response = await _query.GetDetailEmployee(employeeId);

            return Ok(new ApiSuccessResponse<DetailEmployeeResponse>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết nhân viên")
            });
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("update-employee")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật thông tin nhân viên")
            });
        }

        /// <summary>
        /// Thêm nhân viên
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee(AddEmployeeCommand request)
        {
            var response = await _mediator.Send(request);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                Message = string.Format(CommonResource.Msg_Success, "Thêm nhân viên")
            });
        }
    }
}
