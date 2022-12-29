using Azure.Core;
using ISD.API.Applications.Commands.Company;
using ISD.API.Applications.DTOs.Company;
using ISD.API.Core.SeedWork;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Queries.MasterData
{
    public interface ICompanyQuery
    {
        /// <summary>
        /// Lấy danh sách công ty
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        Task<PagingResultSP<CompanySearchResponse>> SearchCompany(CompanySearchCommand req);
    }

    public class CompanyQuery : ICompanyQuery
    {
        private readonly IRepository<CompanyModel> _companyRepo;

        public CompanyQuery(IRepository<CompanyModel> companyRepo)
        {
            _companyRepo = companyRepo;
        }

        public async Task<PagingResultSP<CompanySearchResponse>> SearchCompany(CompanySearchCommand req)
        {
            var data = _companyRepo.GetQuery(x => (!req.CompanyName.IsNullOrEmpty() ? x.CompanyName.Contains(req.CompanyName) : true) &&
                                                      (req.Actived.HasValue ? x.Actived == req.Actived : true))
                                       .Select(x => new CompanySearchResponse
                                       {
                                           //Id
                                           CompanyId = x.CompanyId,
                                           //Mã công ty
                                           CompanyCode = x.CompanyCode,
                                           //Tên công ty
                                           CompanyName = x.CompanyName,
                                           //Logo
                                           Logo = x.Logo,
                                           //Trạng thái
                                           Actived = x.Actived
                                       })
                                       .AsNoTracking();


            //Sắp xếp data
            data = PagingSorting.Sorting(req.Paging, data);


            //Số lượng record
            var totalRecord = await data.CountAsync();

            //Phân trang
            var responsePaging = await PaginatedList<CompanySearchResponse>.CreateAsync(data, req.Paging.Offset, req.Paging.PageSize);

            //Dữ liệu trả về
            var response = new PagingResultSP<CompanySearchResponse>(responsePaging, totalRecord, req.Paging.PageIndex, req.Paging.PageSize);

            //Gắn số thứ tự
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
