using Azure.Core;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MES.Application.Commands.TruckInfo;
using MES.Application.DTOs.MES.TruckInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface ITruckInfoQuery
    {
        /// <summary>
        /// Search data thôgg tin xe tải
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<List<SearchTruckInfoResponse>> SearchTruckInfo(SearchTruckInfoCommand req);

        /// <summary>
        /// Lấy số cân xe tải
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<decimal> GetInputWeight(Guid id);
    }

    public class TruckInfoQuery : ITruckInfoQuery
    {
        private readonly IRepository<TruckInfoModel> _truckInfoRepo;
        private readonly IRepository<AccountModel> _accRepo;

        public TruckInfoQuery(IRepository<TruckInfoModel> truckInfoRepo, IRepository<AccountModel> accRepo)
        {
            _truckInfoRepo = truckInfoRepo;
            _accRepo = accRepo;
        }

        public async Task<decimal> GetInputWeight(Guid id)
        {
            var truckInfo = await _truckInfoRepo.FindOneAsync(x => x.TruckInfoId == id);

            return truckInfo.InputWeight.HasValue ? truckInfo.InputWeight.Value : 0;
        }

        public async Task<List<SearchTruckInfoResponse>> SearchTruckInfo(SearchTruckInfoCommand req)
        {
            if (string.IsNullOrEmpty(req.TruckNumberTo))
                req.TruckNumberTo = req.TruckNumberFrom;
            if (!req.RecordTimeTo.HasValue)
                req.RecordTimeTo = req.RecordTimeFrom;

            #region Format Day

            if (req.RecordTimeFrom.HasValue)
            {
                req.RecordTimeFrom = req.RecordTimeFrom.Value.Date;
            }
            if (req.RecordTimeTo.HasValue)
            {
                req.RecordTimeTo = req.RecordTimeTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Data account
            var accs = _accRepo.GetQuery().AsNoTracking();

            //Lọc data thông tin xe tải
            var data = await _truckInfoRepo.GetQuery(x => //Lọc theo plant
                                                     x.PlantCode == req.Plant &&
                                                     //Lọc theo số xe tải
                                                     (!string.IsNullOrEmpty(req.TruckNumberFrom) ?
                                                     x.TruckNumber.CompareTo(req.TruckNumberFrom) >= 0 && x.TruckNumber.CompareTo(req.TruckNumberTo) <= 0 : true) &&
                                                     //Lọc theo ngày ghi nhận
                                                     (req.RecordTimeFrom.HasValue ? x.CreateTime >= req.RecordTimeFrom && x.CreateTime <= req.RecordTimeTo : true) &&
                                                     //Lọc theo create by
                                                     (req.CreateBy.HasValue ? x.CreateBy == req.CreateBy : true))
                                      .OrderByDescending(x => x.CreateTime)
                                      .Select(x => new SearchTruckInfoResponse
                                      {
                                          TruckInfoId = x.TruckInfoCode,
                                          PlantCode = x.PlantCode,
                                          TruckNumber = x.TruckNumber,
                                          Driver = x.Driver,
                                          InputWeight = x.InputWeight,
                                          RecordTime = x.CreateTime,
                                          CreateById = x.CreateBy,
                                          CreateBy = accs.FirstOrDefault(a => a.AccountId == x.CreateBy).UserName
                                      }).ToListAsync();


            return data;
        }
    }
}
