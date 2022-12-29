using ISD.API.Applications.Commands.Role;
using ISD.API.Applications.DTOs.Role;
using ISD.API.Core.SeedWork;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
    }

    public class RoleQuery : IRoleQuery
    {
        private readonly IRepository<RolesModel> _roleRepo;

        public RoleQuery(IRepository<RolesModel> roleRepo)
        {
            _roleRepo = roleRepo;
        }

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
    }
}
