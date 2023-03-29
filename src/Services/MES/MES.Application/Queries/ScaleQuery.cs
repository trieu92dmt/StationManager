using Core.Exceptions;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Services;
using Microsoft.EntityFrameworkCore;
using Shared.WeighSession;
using System.ComponentModel.DataAnnotations;

namespace MES.Application.Queries
{
    public interface IScaleQuery
    {
        /// <summary>
        /// Lấy chi tiết cân theo code
        /// </summary>
        /// <param name="scaleCode"></param>
        /// <returns></returns>
        Task<ScaleDetailResponse> GetScaleDetail(string scaleCode);
    }

    public class ScaleQuery : IScaleQuery
    {
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IWeighSessionService _weighSsService;
        private readonly IRepository<Screen_Scale_MappingModel> _scaleScreenMappingRepo;

        public ScaleQuery(IRepository<ScaleModel> scaleRepo, IRepository<PlantModel> plantRepo, IWeighSessionService weighSsService, 
                          IRepository<Screen_Scale_MappingModel> scaleScreenMappingRepo)
        {
            _scaleRepo = scaleRepo;
            _plantRepo = plantRepo;
            _weighSsService = weighSsService;
            _scaleScreenMappingRepo = scaleScreenMappingRepo;
        }

        public async Task<ScaleDetailResponse> GetScaleDetail(string scaleCode)
        {
            var scale = await _weighSsService.GetDetailScale(scaleCode);

            //Get query plant
            var plantQuery = _plantRepo.GetQuery().AsNoTracking();

            //Lấy danh sách màn hình
            var screen = await _scaleScreenMappingRepo.GetQuery(x => x.ScaleCode == scaleCode).Select(x => x.ScreenCode).AsNoTracking().ToListAsync();

            //Danh sách màn hình
            scale.Screens = screen;
            //Plant name
            scale.PlantName = plantQuery.FirstOrDefault(x => x.PlantCode == scale.Plant).PlantName;
            //Plant fmt
            scale.PlantFmt = !string.IsNullOrEmpty(scale.Plant) && !string.IsNullOrEmpty(scale.PlantName) ? $"{scale.Plant} | {scale.PlantName}" : "";

            return scale;
        }
    }
}
