using Core.Exceptions;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.DTOs.MES.WeighSession;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface IWeighSessionQuery
    {
        /// <summary>
        /// Lấy chi tiết đợt cân
        /// </summary>
        /// <param name="WeighSessionCode">ID đợt cân</param>
        /// <returns></returns>
        Task<List<DetailWeighSsResponse>> GetDetailWeighSs(string WeighSessionCode);
    }

    public class WeighSessionQuery : IWeighSessionQuery
    {
        private readonly IRepository<WeighSessionDetailModel> _detailWeighSs;

        public WeighSessionQuery(IRepository<WeighSessionDetailModel> detailWeighSs)
        {
            _detailWeighSs = detailWeighSs;
        }

        public async Task<List<DetailWeighSsResponse>> GetDetailWeighSs(string WeighSessionCode)
        {
            //Cho tiết đợt cân lấy theo id đợt cân
            var detailWeighSs = await _detailWeighSs.GetQuery(x => x.WeighSessionCode == WeighSessionCode)
                                                    .OrderByDescending(x => x.NumberOfWeigh)
                                                    .Select(x => new DetailWeighSsResponse
                                                    {
                                                        //Id chi tiết đầu cân
                                                        WeighSessionDetailId = x.WeighSessionDetailID,
                                                        //Id đợt cân
                                                        WeighSessionCode = x.WeighSessionCode,
                                                        //Số lần cân
                                                        NumberOfWeigh = x.NumberOfWeigh ?? 0,
                                                        //Trọng lượng chi tiết
                                                        DetailWeigh = x.DetailWeight ?? 0
                                                    })
                                                    .AsNoTracking().ToListAsync();

            //Không tồn tại => báo lỗi
            if (detailWeighSs == null)
            {
                throw new ISDException(string.Format(CommonResource.Msg_NotFound, "Đợt cân"));
            }

            //Trả data
            return detailWeighSs;
        }
    }
}
