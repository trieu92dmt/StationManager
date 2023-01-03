using ISD.API.Applications.DTOs.Common;
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
    public interface ICommonQuery
    {
        /// <summary>
        /// Dropdown Plant
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropdownPlant(string keyword);
    }

    public class CommonQuery : ICommonQuery
    {
        private readonly IRepository<PlantModel> _plantRepo;

        public CommonQuery(IRepository<PlantModel> plantRepo)
        {
            _plantRepo = plantRepo;
        }

        public async Task<List<CommonResponse>> GetDropdownPlant(string keyword)
        {
            var response = await _plantRepo.GetQuery(x => !string.IsNullOrEmpty(keyword) ? x.PlantName.Contains(keyword) : true)
                                     .OrderBy(x => x.PlantCode)
                                     .Select(x => new CommonResponse
                                     {
                                         Key = x.PlantCode,
                                         Value = x.PlantName
                                     }).Take(10).ToListAsync();

            return response;
        }
    }
}
