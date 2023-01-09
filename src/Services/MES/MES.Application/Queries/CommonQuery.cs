using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Queries
{
    public interface ICommonQuery
    {
        /// <summary>
        /// Dropdown Plant
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPlant(string keyword);

        /// <summary>
        /// Dropdown Sale Org
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownSaleOrg();

        /// <summary>
        /// Dropdown Material
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownMaterial(string keyword);
    }

    public class CommonQuery : ICommonQuery
    {
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<SaleOrgModel> _saleOrgRepo;
        private readonly IRepository<ProductModel> _prodRepo;

        public CommonQuery(IRepository<PlantModel> plantRepo, IRepository<SaleOrgModel> saleOrgRepo, IRepository<ProductModel> prodRepo)
        {
            _plantRepo = plantRepo;
            _saleOrgRepo = saleOrgRepo;
            _prodRepo = prodRepo;
        }

        #region
        public Task<List<CommonResponse>> GetDropdownMaterial(string keyword)
        {
            var response = _prodRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.ProductName.Contains(keyword) || x.ProductCode.Contains(keyword) : true)
                                    .OrderBy(x => x.ProductCode)
                                    .Select(x => new CommonResponse
                                     {
                                         Key = x.ProductCode,
                                         Value = $"{x.ProductCode} | {x.ProductName}"
                                     }).Take(10).ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown Plant
        public async Task<List<CommonResponse>> GetDropdownPlant(string keyword)
        {
            var response = await _plantRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.PlantName.Contains(keyword) : true)
                                     .OrderBy(x => x.PlantCode)
                                     .Select(x => new CommonResponse
                                     {
                                         Key = x.PlantCode,
                                         Value = $"{x.PlantCode} | {x.PlantName}" 
                                     }).Take(10).ToListAsync();

            return response;
        }
        #endregion

        #region Dropdown Sale Org
        public async Task<List<CommonResponse>> GetDropdownSaleOrg()
        {
            var response = await _saleOrgRepo.GetQuery(x => x.Actived == true).Select(x => new CommonResponse
            {
                Key = x.SaleOrgCode,
                Value = $"{x.SaleOrgCode} | {x.SaleOrgName}"
            }).AsNoTracking().ToListAsync();

            return response;
        }
        #endregion
    }
}
