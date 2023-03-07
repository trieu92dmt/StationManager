using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.Commands.XNVLGC;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.XNVLGC;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Get data nvl gia công
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchXNVLGCResponse>> GetDataXNVLGC(SearchXNVLGCCommand command);

        /// <summary>
        /// Get list po by component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        Task<List<GetDataByComponent>> GetListPOByComponent(string component, string componentItem);
    }

    public class GetDataByComponent
    {
        public string PurchaseOrder { get; set; }
        public string PurchaseOrderItem { get; set; }
        public string Material { get; set; }
        public string MaterialDesc { get; set; }
        public string Vendor { get; set; }
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
        private readonly IRepository<ComponentExportModel> _xnvlgcRepo;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;

        public XNVLGCQuery(IRepository<PurchaseOrderDetailModel> poDetailRepo, IRepository<PurchaseOrderMasterModel> poRepo, IRepository<DetailReservationModel> resDetailRepo,
                           IRepository<PlantModel> plantRepo, IRepository<VendorModel> vendorRepo, IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo,
                           IRepository<ComponentExportModel> xnvlgcRepo, IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo, IRepository<ScaleModel> scaleRepo)
        {
            _poDetailRepo = poDetailRepo;
            _poRepo = poRepo;
            _resDetailRepo = resDetailRepo;
            _plantRepo = plantRepo;
            _vendorRepo = vendorRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _xnvlgcRepo = xnvlgcRepo;
            _userRepo = userRepo;
            _cataRepo = cataRepo;
            _scaleRepo = scaleRepo;
        }

        public async Task<List<SearchXNVLGCResponse>> GetDataXNVLGC(SearchXNVLGCCommand request)
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

            var query = _xnvlgcRepo.GetQuery().Include(x => x.PurchaseOrderDetail).AsNoTracking();


            //Lọc điều kiện theo plant
            if (!string.IsNullOrEmpty(request.Plant))
            {
                query = query.Where(x => x.PlantCode == request.Plant);
            }
            //Theo vendor
            if (!string.IsNullOrEmpty(request.VendorFrom))
            {
                if (string.IsNullOrEmpty(request.VendorTo)) request.VendorTo = request.VendorFrom;
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ?
                                             x.VendorCode.CompareTo(request.VendorFrom) >= 0 &&
                                             x.VendorCode.CompareTo(request.VendorTo) <= 0 : false);
            }
            //Theo material
            if (!string.IsNullOrEmpty(request.MaterialFrom))
            {
                if (string.IsNullOrEmpty(request.MaterialTo)) request.MaterialTo = request.MaterialFrom;
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.MaterialCodeInt >= long.Parse(request.MaterialFrom) &&
                                         x.MaterialCodeInt <= long.Parse(request.MaterialTo) : false);
            }
            if (!string.IsNullOrEmpty(request.ComponentFrom))
            {
                if (string.IsNullOrEmpty(request.ComponentTo)) request.ComponentTo = request.ComponentFrom;
                query = query.Where(x => x.ComponentCodeInt >= long.Parse(request.ComponentFrom) &&
                                         x.ComponentCodeInt <= long.Parse(request.ComponentTo));
            }
            //Theo po
            if (!string.IsNullOrEmpty(request.PurchaseOrderFrom))
            {
                if (string.IsNullOrEmpty(request.PurchaseOrderTo)) request.PurchaseOrderTo = request.PurchaseOrderFrom;
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderFrom) >= 0 &&
                                                                            x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCode.CompareTo(request.PurchaseOrderTo) <= 0 : false);
            }
            //Lọc document date
            if (request.DocumentDateFrom.HasValue)
            {
                if (!request.DocumentDateTo.HasValue)
                {
                    request.DocumentDateTo = request.DocumentDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                }
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.DocumentDate >= request.DocumentDateFrom &&
                                                                            x.PurchaseOrderDetail.PurchaseOrder.DocumentDate <= request.DocumentDateTo : false);
            }

            //Search dữ liệu đã cân
            if (!string.IsNullOrEmpty(request.WeightHeadCode))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.WeightHeadCode) ? x.WeightHeadCode.Trim().ToLower() == request.WeightHeadCode.Trim().ToLower() : false);
            }

            //Check Ngày thực hiện cân
            if (request.WeightDateFrom.HasValue)
            {
                if (!request.WeightDateTo.HasValue) request.WeightDateTo = request.WeightDateFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.CreateTime >= request.WeightDateFrom &&
                                         x.CreateTime <= request.WeightDateTo);
            }

            //Check số phiếu cân
            if (request.WeightVotes != null && request.WeightVotes.Any())
            {
                query = query.Where(x => request.WeightVotes.Contains(x.WeightVote));
            }

            //Check create by
            if (request.CreateBy.HasValue)
            {
                query = query.Where(x => x.CreateBy == request.CreateBy);
            }


            //User Query
            var user = _userRepo.GetQuery().AsNoTracking();

            //Scale
            var scale =_scaleRepo.GetQuery().AsNoTracking();


            //Catalog status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new SearchXNVLGCResponse
            {
                //Id
                XNVLGCId = x.ComponentExportId,
                //Plant
                Plant = x.PlantCode,
                //Plant name
                PlantName = x.PlantName,
                //PO
                PurchaseOrder = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.PurchaseOrderCodeInt.ToString() : "",
                //POItem
                PurchaseOrderItem = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.POLine : "",
                //Material
                Material = x.MaterialCodeInt.HasValue ? x.MaterialCodeInt.ToString() : "",
                //Material desc
                MaterialDesc = x.MaterialName ?? "",
                //Component
                Component = x.ComponentCodeInt.ToString(),
                //Component item
                ComponentItem = x.ComponentItem,
                //Component desc
                ComponentDesc = x.ComponentName,
                //Sloc
                Sloc = x.SlocCode ?? "",
                SlocName = x.SlocName ?? "",
                //Batch
                Batch = x.Batch ?? "",
                //SL bao
                BagQuantity = x.BagQuantity ?? 0,
                //Đơn trọng
                SingleWeight = x.SingleWeight ?? 0,
                //Đầu cân
                WeightHeadCode = x.WeightHeadCode ?? "",
                ScaleType = !string.IsNullOrEmpty(x.WeightHeadCode) ? scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).isCantai == true ? "CANXETAI" :
                                                                      scale.FirstOrDefault(s => s.ScaleCode == x.WeightHeadCode).ScaleType == true ? "TICHHOP" : "KHONGTICHHOP" : "KHONGTICHHOP",
                //Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Số lần cân
                QuantityWeight = x.QuantityWeight ?? 0,
                //Order quantity
                OrderQuantity = x.TotalQuantity,
                //Order unit
                OrderUnit = x.OrderUnit ?? "",
                //Requirement quantity
                RequirementQuantity = x.RequirementQuantity,
                //Requirement unit
                RequirementUnit = x.RequirementUnit ?? "",
                //VendorName
                Vendor = x.VendorCode ?? "",
                VendorName = x.VendorName ?? "",
                //Số xe tải
                TruckInfoId = x.TruckInfoId ?? null,
                TruckNumber = x.TruckNumber ?? "",
                //Số cân đầu vào
                InputWeight = x.InputWeight ?? 0,
                //Số cân đầu ra
                OutputWeight = x.OutputWeight ?? 0,
                //Ghi chú
                Description = x.Description ?? "",
                //Hình ảnh
                Image = !string.IsNullOrEmpty(x.Image) ? $"https://itp-mes.isdcorp.vn/{x.Image}" : "",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote,
                //29 Thời gian bắt đầu
                StartTime = x.StartTime ?? null,
                //30 Thời gian kết thúc
                EndTime = x.EndTime ?? null,
                DocumentDate = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.DocumentDate : null,
                //31 Create by
                CreateById = x.CreateBy ?? null,
                CreateBy = x.CreateBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.CreateBy).FullName : "",
                //32 Crete On
                CreateOn = x.CreateTime ?? null,
                //33 Change by
                ChangeById = x.LastEditBy ?? null,
                ChangeBy = x.LastEditBy.HasValue ? user.FirstOrDefault(a => a.AccountId == x.LastEditBy).FullName : "",
                ChangeOn = x.LastEditTime ?? null,
                //34 Material Doc
                MaterialDoc = x.MaterialDocument ?? null,
                //35 Reverse Doc
                ReverseDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false,
                isEdit = !string.IsNullOrEmpty(x.MaterialDocument) ? false : true

            }).ToListAsync();

            return data;
        }

        public async Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            return await _xnvlgcRepo.GetQuery(x => string.IsNullOrEmpty(keyword) ? true : x.WeightVote.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.WeightVote,
                                            Value = x.WeightVote
                                        }).Distinct().Take(20).ToListAsync();
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
                            Id = Guid.NewGuid(),
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
                            //Component item
                            ComponentItem = res.ReservationItem,
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
                            OrderQuantity = po.OrderQuantity,
                            //Order unit
                            OrderUnit = po.Unit ?? "",
                            //Requirement Qty
                            RequirementQuantity = res.RequirementQty,
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

        public async Task<List<GetDataByComponent>> GetListPOByComponent(string component, string componentItem)
        {
            //Get query po
            var pos = _poDetailRepo.GetQuery().Include(x => x.PurchaseOrder).AsNoTracking();

            //Get query material
            var materials = _prodRepo.GetQuery().AsNoTracking();

            //Get query reservation
            var reservations = _resDetailRepo.GetQuery(x => x.MaterialCodeInt == long.Parse(component) &&
                                                            x.ReservationItem == componentItem).AsNoTracking();

            var data = await (from res in reservations
                        join p in pos on new { PO = res.PurchasingDoc, POLine = res.Item } equals
                                         new { PO = p.PurchaseOrder.PurchaseOrderCode, POLine = p.POLine }
                        join m in materials on p.ProductCode equals m.ProductCode
                        select new GetDataByComponent
                        {
                            PurchaseOrder = res.PurchasingDoc,
                            PurchaseOrderItem = res.Item,
                            Material = m.ProductCodeInt.ToString(),
                            MaterialDesc = m.ProductName,
                            Vendor = p.PurchaseOrder.VendorCode
                        }).ToListAsync();

            return data;
        }
    }
}
