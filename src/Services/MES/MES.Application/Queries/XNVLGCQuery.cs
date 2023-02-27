using Azure.Core;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.XNVLGC;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XNVLGC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface IXNVLGCQuery
    {
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchXNVLGCCommand command);
    }

    public class XNVLGCQuery : IXNVLGCQuery
    {
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRepo;
        private readonly IRepository<PurchaseOrderMasterModel> _poRepo;

        public XNVLGCQuery(IRepository<PurchaseOrderDetailModel> poDetailRepo, IRepository<PurchaseOrderMasterModel> poRepo)
        {
            _poDetailRepo = poDetailRepo;
            _poRepo = poRepo;
        }

        public Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetInputDataResponse>> GetInputData(SearchXNVLGCCommand request)
        {
            #region Format Day

            //Document date
            if (request.DocumentDateFrom.HasValue)
            {
                request.DocumentDateFrom = request.DocumentDateFrom.Value.Date;
            }
            if (request.DocumentDateTo.HasValue)
            {
                request.DocumentDateTo = request.DocumentDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }

            //Ngày cân
            if (request.WeightDateFrom.HasValue)
            {
                request.WeightDateFrom = request.WeightDateFrom.Value.Date;
            }
            if (request.WeightDateTo.HasValue)
            {
                request.WeightDateTo = request.WeightDateTo.Value.Date.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //Get query
            var queryPO = _poDetailRepo.GetQuery()
                                            .Include(x => x.PurchaseOrder)
                                            .Where(x => x.PurchaseOrder.POType == "Z003" &&
                                                        x.DeliveryCompleted != "X" &&
                                                        x.DeletionInd != "X" &&
                                                        x.PurchaseOrder.DeletionInd != "X" &&
                                                        x.PurchaseOrder.ReleaseIndicator == "R")
                                            .AsNoTracking();

            //Lọc điều kiện theo plant
            if (!string.IsNullOrEmpty(request.Plant))
            {
                queryPO = queryPO.Where(x => x.PurchaseOrder.Plant == request.Plant);
            }
            //if (!string.IsNullOrEmpty(request.PurchasingOrgFrom))
            //{
            //    if (string.IsNullOrEmpty(request.PurchasingOrgTo)) request.PurchasingOrgTo = request.PurchasingOrgFrom;
            //    queryPO = queryPO.Where(x => x.PurchaseOrder.PurchasingOrg == request.PurchasingOrgFrom);
            //}
            ////!x.PurchaseOrder.VendorCode.IsNullOrEmpty() &&
            //if (!string.IsNullOrEmpty(request.VendorFrom))
            //{
            //    if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
            //    queryPO = queryPO.Where(x =>
            //                                 x.PurchaseOrder.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
            //                                 x.PurchaseOrder.VendorCode.CompareTo(request.VendorTo) <= 0);
            //}

            //if (!string.IsNullOrEmpty(request.POType))
            //{
            //    queryPO = queryPO.Where(x => x.PurchaseOrder.POType.Contains(request.POType));
            //}
            //if (!string.IsNullOrEmpty(request.MaterialFrom))
            //{
            //    if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
            //    queryPO = queryPO.Where(x => x.ProductCodeInt >= long.Parse(request.MaterialFrom) &&
            //                             x.ProductCodeInt <= long.Parse(request.MaterialTo));
            //}

            //if (!string.IsNullOrEmpty(request.PurchasingGroupFrom))
            //{
            //    if (string.IsNullOrEmpty(request.PurchasingGroupTo)) request.PurchasingGroupTo = request.PurchasingGroupFrom;
            //    queryPO = queryPO.Where(x => x.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupFrom) >= 0 &&
            //                                 x.PurchaseOrder.PurchasingGroup.CompareTo(request.PurchasingGroupTo) <= 0);
            //}

            //if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            //{
            //    if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
            //    queryPO = queryPO.Where(x => x.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderFrom) >= 0 &&
            //                                 x.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderTo) <= 0);
            //}
            //Lọc document date
            if (request.DocumentDateFrom.HasValue)
            {
                if (!request.DocumentDateTo.HasValue)
                {
                    request.DocumentDateTo = request.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                queryPO = queryPO.Where(x => x.PurchaseOrder.DocumentDate >= request.DocumentDateFrom &&
                                                 x.PurchaseOrder.DocumentDate <= request.DocumentDateTo);
            }

            throw new NotImplementedException();
        }
    }
}
