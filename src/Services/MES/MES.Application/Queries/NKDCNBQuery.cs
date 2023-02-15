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
        //Task<List<GetInputDataResponse>> GetInputData(SearchNKDCNBCommand command);

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

        //public Task<List<GetInputDataResponse>> GetInputData(SearchNKDCNBCommand command)
        //{
        //    #region Format Day

        //    if (command.DocumentDateFrom.HasValue)
        //    {
        //        command.DocumentDateFrom = command.DocumentDateFrom.Value.Date;
        //    }
        //    if (command.DocumentDateTo.HasValue)
        //    {
        //        command.DocumentDateTo = command.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
        //    }
        //    #endregion

        //    //Tạo query detail od
        //    var query = _detailOdRepo.GetQuery()
        //                                .Include(x => x.OutboundDelivery)
        //                                //Lọc delivery type
        //                                .Where(x => x.OutboundDelivery.DeliveryType == "ZNLC" && x.OutboundDelivery.DeliveryType == "ZNLN" &&
        //                                            //Lấy delivery đã hoàn tất giao dịch
        //                                            x.OutboundDelivery.GoodsMovementSts == "C" &&
        //                                            )
        //                                .AsNoTracking();

        //    //Products
        //    //var prods = _prdRepo.GetQuery().AsNoTracking();

        //    //Sloc
        //    //var slocs = _slocRepo.GetQuery().AsNoTracking();

        //    //Plant
        //    //var plants = _plantRepo.GetQuery().AsNoTracking();

        //    //Lọc điều kiện
        //    //Theo plant
        //    if (!string.IsNullOrEmpty(command.Plant))
        //    {
        //        query = query.Where(x => x.OutboundDelivery.ReceivingPlant == command.Plant);
        //    }

        //    //Theo shipping point
        //    if (!string.IsNullOrEmpty(command.ShippingPoint))
        //    {
        //        query = query.Where(x => x.OutboundDelivery.ShippingPoint == command.ShippingPoint);
        //    }

        //    //Theo sale order
        //    if (!string.IsNullOrEmpty(command.SalesOrderFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.SalesOrderTo))
        //            command.SalesOrderTo = command.SalesOrderFrom;

        //        query = query.Where(x => x.ReferenceDocument1.CompareTo(command.SalesOrderFrom) >= 0 &&
        //                                 x.ReferenceDocument1.CompareTo(command.SalesOrderTo) <= 0);
        //    }

        //    //Theo outbound deliver
        //    if (!string.IsNullOrEmpty(command.OutboundDeliveryFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.OutboundDeliveryTo))
        //            command.OutboundDeliveryTo = command.OutboundDeliveryFrom;

        //        query = query.Where(x => x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryFrom) >= 0 &&
        //                                 x.OutboundDelivery.DeliveryCode.CompareTo(command.OutboundDeliveryTo) <= 0);
        //    }

        //    //Theo ship to party
        //    if (!string.IsNullOrEmpty(command.ShipToPartyFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.ShipToPartyTo))
        //            command.ShipToPartyTo = command.ShipToPartyFrom;
        //        query = query.Where(x => x.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyFrom) >= 0 &&
        //                                 x.OutboundDelivery.ShiptoParty.CompareTo(command.ShipToPartyTo) <= 0);
        //    }

        //    //Theo Material
        //    if (!string.IsNullOrEmpty(command.MaterialFrom))
        //    {
        //        if (string.IsNullOrEmpty(command.MaterialTo))
        //            command.MaterialTo = command.MaterialFrom;

        //        query = query.Where(x => x.ProductCodeInt >= long.Parse(command.MaterialFrom) &&
        //                                 x.ProductCodeInt <= long.Parse(command.MaterialTo));
        //    }

        //    //Theo document date
        //    if (command.DocumentDateFrom.HasValue)
        //    {
        //        if (!command.DocumentDateTo.HasValue)
        //        {
        //            command.DocumentDateTo = command.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
        //        }
        //        query = query.Where(x => x.OutboundDelivery.DocumentDate >= command.DocumentDateFrom &&
        //                                 x.OutboundDelivery.DocumentDate <= command.DocumentDateTo);
        //    }
        //}

        public Task<List<SearchNKDCNBResponse>> GetNKPPPP(SearchNKDCNBCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
