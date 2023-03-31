using Core.SeedWork.Repositories;
using Infrastructure.Models;
using IntegrationNS.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.XNVLGCs
{
    public class XNVLGCIntegrationCommand : IRequest<List<XNVLGCResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Vendor
        public string VendorFrom { get; set; }
        public string VendorTo { get; set; }
        //PO type
        public string POTypeFrom { get; set; }
        public string POTypeTo { get; set; }
        //Purchase Order
        public string PurchaseOrderFrom { get; set; }
        public string PurchaseOrderTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Component
        public string ComponentFrom { get; set; }
        public string ComponentTo { get; set; }
        //Document Date
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        //CreateBy
        public Guid? CreateBy { get; set; }
        public string Status { get; set; }
    }

    public class XNVLGCIntegrationCommandHandler : IRequestHandler<XNVLGCIntegrationCommand, List<XNVLGCResponse>>
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

        public XNVLGCIntegrationCommandHandler(IRepository<PurchaseOrderDetailModel> poDetailRepo, IRepository<PurchaseOrderMasterModel> poRepo, IRepository<DetailReservationModel> resDetailRepo,
                           IRepository<PlantModel> plantRepo, IRepository<VendorModel> vendorRepo, IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo,
                           IRepository<ComponentExportModel> xnvlgcRepo, IRepository<AccountModel> userRepo, IRepository<CatalogModel> cataRepo)
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
        }
        public async Task<List<XNVLGCResponse>> Handle(XNVLGCIntegrationCommand request, CancellationToken cancellationToken)
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

            //Theo po type
            if (!string.IsNullOrEmpty(request.POTypeFrom))
            {
                if (string.IsNullOrEmpty(request.POTypeTo)) request.POTypeTo = request.POTypeFrom;
                query = query.Where(x => x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.POType.CompareTo(request.POTypeFrom) >= 0 &&
                                                                            x.PurchaseOrderDetail.PurchaseOrder.POType.CompareTo(request.POTypeTo) <= 0 : false);
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

            //Search Status
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(x => x.Status == request.Status && x.ReverseDocument == null);
            }


            //User Query
            var user = _userRepo.GetQuery().AsNoTracking();


            //Catalog status
            var status = _cataRepo.GetQuery(x => x.CatalogTypeCode == "NKMHStatus").AsNoTracking();

            var data = await query.OrderByDescending(x => x.WeightVote).ThenByDescending(x => x.CreateTime).Select(x => new XNVLGCResponse
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
                //Trọng lượng cân
                Weight = x.Weight ?? 0,
                //Confirm quantity
                ConfirmQty = x.ConfirmQty ?? 0,
                //SL kèm bao bì
                QuantityWithPackage = x.QuantityWithPackaging ?? 0,
                //Số phương tiện
                VehicleCode = x.VehicleCode ?? "",
                //Đơn vị vận chuyển
                TransportUnit = x.TransportUnit ?? "",
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
                //Document Date
                DocumentDate = x.PurchaseOrderDetailId.HasValue ? x.PurchaseOrderDetail.PurchaseOrder.DocumentDate : null,
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
                Image = string.IsNullOrEmpty(x.Image) ? "" : $"https://itp-mes.isdcorp.vn/{x.Image}",
                //Status
                Status = status.FirstOrDefault(s => s.CatalogCode == x.Status).CatalogText_vi,
                //Số phiếu cân
                WeightVote = x.WeightVote,
                //29 Thời gian bắt đầu
                StartTime = x.StartTime ?? null,
                //30 Thời gian kết thúc
                EndTime = x.EndTime ?? null,
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
                //Mat doc item
                MaterialDocItem = x.MaterialDocumentItem ?? null,
                //35 Reverse Doc
                ReverseDoc = x.ReverseDocument ?? null,
                isDelete = x.Status == "DEL" ? true : false

            }).ToListAsync();

            return data;
        }
    }
}
