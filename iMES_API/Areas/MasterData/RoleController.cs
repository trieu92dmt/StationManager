using ISD.API.Applications.Commands.Company;
using ISD.API.Applications.Commands.Role;
using ISD.API.Applications.DTOs.Company;
using ISD.API.Applications.DTOs.Role;
using ISD.API.Applications.Queries.MasterData;
using ISD.API.Core;
using ISD.API.Core.Extensions;
using ISD.API.Resources;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLG_API.Areas.MasterData
{
    [Route("api/v{version:apiVersion}/MasterData/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RoleController : ControllerBaseAPI
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
        [HttpPost("get-detail-role")]
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
        [HttpPost("update-role")]
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
    }
}
