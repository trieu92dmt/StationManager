using Core.Exceptions;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Services;
using Microsoft.EntityFrameworkCore;
using Shared.WeighSession;

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
        private readonly IWeighSessionService _weighSsService;

        public WeighSessionQuery(IRepository<WeighSessionDetailModel> detailWeighSs, IWeighSessionService weighSsService)
        {
            _detailWeighSs = detailWeighSs;
            _weighSsService = weighSsService;
        }

        public async Task<List<DetailWeighSsResponse>> GetDetailWeighSs(string WeighSessionCode)
        {
            return await _weighSsService.GetListDetailWeighSession(WeighSessionCode);
        }
    }
}
