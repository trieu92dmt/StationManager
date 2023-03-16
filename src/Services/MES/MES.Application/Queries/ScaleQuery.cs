using Core.Exceptions;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.DTOs.MES.Scale;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface IScaleQuery
    {
        /// <summary>
        /// Lấy chi tiết cân theo id
        /// </summary>
        /// <param name="scaleId"></param>
        /// <returns></returns>
        Task<ScaleDetailResponse> GetScaleDetail(Guid scaleId);
    }

    public class ScaleQuery : IScaleQuery
    {
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<PlantModel> _plantRepo;

        public ScaleQuery(IRepository<ScaleModel> scaleRepo, IRepository<PlantModel> plantRepo)
        {
            _scaleRepo = scaleRepo;
            _plantRepo = plantRepo;
        }

        public async Task<ScaleDetailResponse> GetScaleDetail(Guid scaleId)
        {
            //Get query plant
            var plantQuery = _plantRepo.GetQuery().AsNoTracking();

            //Check tồn tại cân
            var scale = await _scaleRepo.GetQuery().Include(x => x.Screen_Scale_MappingModel).ThenInclude(x => x.Screen).FirstOrDefaultAsync(s => s.ScaleId == scaleId);

            if (scale == null)
                throw new ISDException(string.Format(CommonResource.Msg_NotFound, "Cân"));

            return new ScaleDetailResponse
            {
                //Id cân
                ScaleId = scale.ScaleId,
                //Nhà máy
                Plant = scale.Plant,
                //Tên nhà máy
                PlantName = plantQuery.FirstOrDefault(x => x.PlantCode == scale.Plant).PlantName,
                //Mã cân
                ScaleCode = scale.ScaleCode,
                //Tên cân
                ScaleName = scale.ScaleName,
                //Cân tích hợp
                isIntegrated = scale.ScaleType == true ? true : false,
                //Cân xe tải
                isTruckScale = scale.isCantai == true ? true : false,
                //Trạng thái
                Status = scale.Actived == true ? true : false,
                //List màn hình đã chọn
                Screens = scale.Screen_Scale_MappingModel.Where(m => m.ScaleId == scale.ScaleId).Select(m => m.Screen.ScreenCode).ToList()
            };
        }
    }
}
