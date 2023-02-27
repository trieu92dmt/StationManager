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
        private readonly IRepository<DetailReservationModel> _resDetailRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<VendorModel> _vendorRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public XNVLGCQuery(IRepository<PurchaseOrderDetailModel> poDetailRepo, IRepository<PurchaseOrderMasterModel> poRepo, IRepository<DetailReservationModel> resDetailRepo,
                           IRepository<PlantModel> plantRepo, IRepository<VendorModel> vendorRepo, IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo)
        {
            _poDetailRepo = poDetailRepo;
            _poRepo = poRepo;
            _resDetailRepo = resDetailRepo;
            _plantRepo = plantRepo;
            _vendorRepo = vendorRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
        }

        public Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetInputDataResponse>> GetInputData(SearchXNVLGCCommand request)
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

            //Get plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Get vendor
            var vendors = _vendorRepo.GetQuery().AsNoTracking();

            //Get material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Get sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query Reservation
            var queryRES = _resDetailRepo.GetQuery().Include(x => x.Reservation)
                                         .Where(x => x.PurchasingDoc != null && x.Item != null).AsNoTracking();
            //Theo component
            if (!string.IsNullOrEmpty(request.ComponentFrom))
            {
                if (string.IsNullOrEmpty(request.ComponentTo)) request.ComponentTo = request.ComponentFrom;
                queryRES = queryRES.Where(x => x.MaterialCodeInt >= long.Parse(request.ComponentFrom) &&
                                               x.MaterialCodeInt <= long.Parse(request.ComponentTo));
            }

            //Get query PO
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
                //PO
                queryPO = queryPO.Where(x => x.PurchaseOrder.Plant == request.Plant);
            }
            //Theo vendor
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                queryPO = queryPO.Where(x =>
                                             x.PurchaseOrder.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
                                             x.PurchaseOrder.VendorCode.CompareTo(request.VendorTo) <= 0);
            }
            //Theo material
            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                queryPO = queryPO.Where(x => x.ProductCodeInt >= long.Parse(request.MaterialFrom) &&
                                         x.ProductCodeInt <= long.Parse(request.MaterialTo));
            }
            //Theo po
            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                queryPO = queryPO.Where(x => x.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderFrom) >= 0 &&
                                             x.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderTo) <= 0);
            }
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

            var data = await (from res in queryRES
                        join po in queryPO on
                        new { PO = res.PurchasingDoc, POLine = res.Item } equals
                        new { PO = po.PurchaseOrder.PurchaseOrderCode, POLine = po.POLine }
                        select new GetInputDataResponse
                        {
                            //Plant
                            Plant = po.PurchaseOrder.Plant,
                            //Plant name
                            PlantName = plants.FirstOrDefault(x => x.PlantCode == po.PurchaseOrder.Plant).PlantName,
                            //Vendor
                            Vendor = po.PurchaseOrder.VendorCode,
                            //Vendor name
                            VendorName = vendors.FirstOrDefault(x => x.VendorCode == po.PurchaseOrder.VendorCode).VendorName,
                            //Purchase order
                            PurchaseOrder = po.PurchaseOrder.PurchaseOrderCodeInt.ToString(),
                            //PO item
                            PurchaseOrderItem = po.POLine,
                            //Material
                            Material = po.ProductCodeInt.ToString(),
                            //Material Desc
                            MaterialDesc = materials.FirstOrDefault(x => x.ProductCode == po.ProductCode).ProductName,
                            //Component
                            Component = res.MaterialCodeInt.ToString(),
                            //Component Desc
                            ComponentDesc = materials.FirstOrDefault(x => x.ProductCode == res.Material).ProductName,
                            //Document date
                            DocumentDate = po.PurchaseOrder.DocumentDate ?? null,
                            //Sloc
                            Sloc = res.Reservation.Sloc,
                            //Slocname
                            SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == res.Reservation.Sloc).StorageLocationName,
                            //Batch
                            Batch = res.Batch ?? "",
                            //Số phương tiện
                            VehicleCode = po.VehicleCode ?? "",
                            //Order quantity
                            OrderQuantity = po.OrderQuantity ?? 0,
                            //Order unit
                            OrderUnit = po.Unit ?? "",
                            //Requirement Qty
                            RequirementQuantity = res.RequirementQty ?? 0,
                            //Requirement unit
                            RequirementUnit = res.BaseUnit
                        }).ToListAsync();

            var index = 1;
            foreach (var item in data)
            {
                item.IndexKey = index;
                index++;
            }

            return data;
        }
    }
}
