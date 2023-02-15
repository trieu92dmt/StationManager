using Core.Models;
using Core.Properties;
using ISD.API.Applications.Queries.MasterData;
using MasterData.Application.DTOs;
using MasterData.Applications.Commands.Role;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MasterData.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleQuery _roleQuery;
        private readonly IMediator _mediator;

        public RoleController(IRoleQuery roleQuery, IMediator mediator)
        {
            _roleQuery = roleQuery;
            _mediator = mediator;
        }

        #region Search nhóm người dùng
        /// <summary>
        /// Search nhóm người dùng
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("search-role")]
        public async Task<IActionResult> SearchRole(RoleSearchCommand req)
        {
            var response = await _roleQuery.SearchRole(req);

            return Ok(new ApiSuccessResponse<IList<RoleSearchResponse>>
            {
                Data = response.Data,
                IsSuccess = true,
                RecordsTotal = response.Paging.TotalCount,
                PagesCount = response.Paging.TotalPages,
                ResultsCount = response.Paging.PageSize
            });
        }
        #endregion

        #region Chi tiết nhóm người dùng
        /// <summary>
        /// Chi tiết nhóm người dùng
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("get-detail-role")]
        public async Task<IActionResult> DetailRole(Guid roleId)
        {
            var response = await _roleQuery.GetDetailRole(roleId);

            return Ok(new ApiSuccessResponse<RoleDetailResponse>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Lấy chi tiết nhóm người dùng")
            });
        }
        #endregion

        #region Tạo nhóm người dùng
        /// <summary>
        /// Tạo nhóm người dùng
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole(RoleCreateCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Tạo nhóm người dùng")
            });
        }
        #endregion

        #region Chỉnh sửa nhóm người dùng
        /// <summary>
        /// Chỉnh sửa nhóm người dùng
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole(RoleUpdateCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Chỉnh sửa nhóm người dùng")
            });
        }
        #endregion

        #region Xóa nhóm người dùng
        /// <summary>
        /// Xóa nhóm người dùng
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("delete-role")]
        public async Task<IActionResult> DeleteRole(RoleDeleteCommand req)
        {
            var response = await _mediator.Send(req);

            return Ok(new ApiSuccessResponse<bool>
            {
                Data = response,
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Xóa nhóm người dùng")
            });
        }
        #endregion
    }
}
