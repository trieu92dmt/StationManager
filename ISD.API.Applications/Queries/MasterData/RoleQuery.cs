using ISD.API.Applications.Commands.Role;
using ISD.API.Applications.DTOs.Common;
using ISD.API.Applications.DTOs.Role;
using ISD.API.Core.Exceptions;
using ISD.API.Core.SeedWork;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Resources;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Queries.MasterData
{
    public interface IRoleQuery
    {
        /// <summary>
        /// Search nhóm người dùng
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<PagingResultSP<RoleSearchResponse>> SearchRole(RoleSearchCommand req);

        /// <summary>
        /// Chi tiết nhóm người dùng
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<RoleDetailResponse> GetDetailRole(Guid roleId);
    }

    public class RoleQuery : IRoleQuery
    {
        private readonly IRepository<RolesModel> _roleRepo;
        private readonly IRepository<AccountModel> _accRepo;

        public RoleQuery(IRepository<RolesModel> roleRepo, IRepository<AccountModel> accRepo)
        {
            _roleRepo = roleRepo;
            _accRepo = accRepo;
        }

        #region Chi tiết nhóm người dùng
        public async Task<RoleDetailResponse> GetDetailRole(Guid roleId)
        {
            //Kiểm tra nhóm người dùng có tồn tại
            var role = await _roleRepo.FindOneAsync(x => x.RolesId == roleId);

            if (role == null)
                throw new ISDException(CommonResource.Msg_NotFound, "Nhóm người dùng");

            //Lấy danh sách người dùng thuộc role
            var accs = await _accRepo.GetQuery().Include(x => x.Roles).Where(x => x.Roles.FirstOrDefault(r => r.RolesId == role.RolesId) != null)
                                     .Select(x => new User
                                     {
                                         //Mã nhân viên
                                         EmployeeCode = x.EmployeeCode,
                                         //Tên tài khoản
                                         UserName = x.UserName,
                                         //Họ và tên
                                         FullName = x.FullName,
                                         //Trạng thái
                                         Actived = x.Actived.HasValue ? x.Actived.Value : false,
                                     })
                                     .ToListAsync();

            //Dữ liệu trả về
            var response = new RoleDetailResponse()
            {
                RoleId = role.RolesId,
                RoleCode = role.RolesCode,
                RoleName = role.RolesName,
                OrderIndex = role.OrderIndex.HasValue ? role.OrderIndex.Value : 0,
                Actived = role.Actived.HasValue ? role.Actived.Value : false,
                Employees = accs
            };

            return response;
        }
        #endregion

        #region Tìm kiếm nhóm người dùng
        public async Task<PagingResultSP<RoleSearchResponse>> SearchRole(RoleSearchCommand req)
        {
            var data = _roleRepo.GetQuery(x => !string.IsNullOrEmpty(req.RoleName) ? x.RolesName.Contains(req.RoleName) : true)
                                .Select(x => new RoleSearchResponse
                                {
                                    //Id
                                    RolesId = x.RolesId,
                                    //Mã nhóm
                                    RolesCode = x.RolesCode,
                                    //Tên nhóm
                                    RolesName = x.RolesName,
                                    //Phân nhóm
                                    IsEmployeeGroup = x.isEmployeeGroup.HasValue ? x.isEmployeeGroup.Value : false,
                                    //Thứ tự
                                    OrderIndex = x.OrderIndex.HasValue ? x.OrderIndex.Value : 0,
                                    //Trạng thái
                                    Actived = x.Actived.HasValue ? x.Actived.Value : false,
                                })
                                .AsNoTracking();

            //Số lượng record
            var totalRecords = await data.CountAsync();

            //Sắp xếp
            data = PagingSorting.Sorting(req.Paging, data);

            //Phân trang
            var responsePaging = await PaginatedList<RoleSearchResponse>.CreateAsync(data, req.Paging.Offset, req.Paging.PageSize);

            //Dữ liệu trả về
            var response = new PagingResultSP<RoleSearchResponse>(responsePaging, totalRecords, req.Paging.PageIndex, req.Paging.PageSize);

            //Gán số thứ tự
            if (response.Data.Any())
            {
                int i = req.Paging.Offset;
                foreach (var item in response.Data)
                {
                    i++;
                    item.STT = i;
                }
                
            }

            return response;
        }
        #endregion
    }
}
