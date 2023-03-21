using Microsoft.EntityFrameworkCore;
using Shared.WeighSession;
using WeighSession.API.Repositories.Interfaces;
using WeighSession.Infrastructure.Models;

namespace WeighSession.API.Repositories
{
    public class WeighSessionRepository : IWeighSessionRepository
    {
        private readonly DataCollectionContext _context;

        public WeighSessionRepository(DataCollectionContext context)
        {
            _context = context;
        }
        public async Task<List<WeightHeadResponse>> GetWeightHeadAsync(string keyWord, string plantCode, string type)
        {
            var result = await _context.ScaleModel.Where(x => true).Select(x => new WeightHeadResponse
            {
                //Mã đầu cân
                Key = x.ScaleCode,
                //Mã đầu cân | Tên đầu cân
                Value = $"{x.ScaleCode} | {x.ScaleName.Trim()}",
                Data = x.ScaleType.Value == true ? true : false,
                //Loại cân
                Type = x.IsCantai == true ? "CANXETAI" : (x.ScaleType == true ? "TICHHOP" : "KHONGTICHHOP")
            }).ToListAsync();

            //var response = await _scaleRepo.GetQuery(x =>
            //                                  //Lọc theo từ khóa
            //                                  (!string.IsNullOrEmpty(keyword) ? x.ScaleCode.Contains(keyword) || x.ScaleName.Contains(keyword) : true) &&
            //                                  //Lấy theo mã nhà máy
            //                                  (!string.IsNullOrEmpty(plantCode) ? x.Plant == plantCode : true))
            //            .OrderBy(x => x.ScaleCode)
            //            .Select(x => new DropdownWeightHeadResponse
            //            {
            //                //Mã đầu cân
            //                Key = x.ScaleCode,
            //                //Mã đầu cân | Tên đầu cân
            //                Value = $"{x.ScaleCode} | {x.ScaleName}",
            //                Data = x.ScaleType.Value == true ? true : false,
            //                //Loại cân
            //                Type = x.isCantai == true ? "CANXETAI" : (x.ScaleType == true ? "TICHHOP" : "KHONGTICHHOP")
            //            }).AsNoTracking().ToListAsync();

            return result;
        }
    }
}
