using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.NKDCNB;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NKDCNB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INKDCNBQuery
    {
        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchNKDCNBCommand command);

        /// <summary>
        /// Lấy data nkdcnb
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNKDCNBResponse>> GetNKPPPP(SearchNKDCNBCommand command);

        /// <summary>
        /// Drop down số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data theo wo
        /// </summary>
        /// <param name="workorder"></param>
        /// <returns></returns>
        //Task<GetDataByWoAndComponentResponse> GetDataByWoAndComponent(string workorder, string component);
    }

    public class NKDCNBQuery : INKDCNBQuery
    {
        private readonly IRepository<InhouseTransferModel> _nkdcnbRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailOdRepo;

        public NKDCNBQuery(IRepository<InhouseTransferModel> nkdcnbRepo, IRepository<DetailOutboundDeliveryModel> detailOdRepo)
        {
            _nkdcnbRepo = nkdcnbRepo;
            _detailOdRepo = detailOdRepo;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _nkdcnbRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.WeightVote,
                                             Value = x.WeightVote
                                         }).Distinct().Take(20).ToListAsync();
        }

        public Task<List<GetInputDataResponse>> GetInputData(SearchNKDCNBCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<List<SearchNKDCNBResponse>> GetNKPPPP(SearchNKDCNBCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
