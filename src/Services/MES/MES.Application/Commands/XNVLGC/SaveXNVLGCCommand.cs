using Core.Exceptions;
using Core.Extensions;
using Core.Implements;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XNVLGC
{
    public class SaveXNVLGCCommand : IRequest<bool>
    {
        public List<SaveXNVLGC> SaveXNVLGCs { get; set; } = new List<SaveXNVLGC>();
    }

    public class SaveXNVLGC
    {
        //public Guid Id { get; set; }
        //Plant
        public string Plant { get; set; }
        //po
        public string PurchaseOrder { get; set; }
        //po item
        public string PurchaseOrderItem { get; set; }
        //4. Material
        public string Material { get; set; }
        //component
        public string Component { get; set; }
        //Component item
        public string ComponentItem { get; set; }
        //vendor name
        public string VendorCode { get; set; }
        //7. Stor. Loc
        public string Sloc { get; set; }
        //9. Batch
        public string Batch { get; set; }
        //11. SL bao
        public int? BagQuantity { get; set; }
        //12. Đơn trọng
        public decimal? SingleWeight { get; set; }
        //13. Đầu cân
        public string WeightHeadCode { get; set; }
        //14. Trọng lượng cân
        public decimal? Weight { get; set; }
        //15. Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //16. SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //17. Số phương tiện
        public string VehicleCode { get; set; }
        //18. Số lần cân
        public int? QuantityWeight { get; set; }
        //Order quantity
        public decimal? OrderQuantity { get; set; }
        //Order unit
        public string OrderUnit { get; set; }
        //Requirement quantity
        public decimal? RequirementQuantity { get; set; }
        //Requirement unit
        public string RequirementUnit { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
        //23. Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //24. Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
    }


    public class SaveXNVLGCCommandHandler : IRequestHandler<SaveXNVLGCCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ComponentExportModel> _xnvlgcRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<PurchaseOrderDetailModel> _detailPoRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        private readonly IRepository<VendorModel> _vendorRepo;
        private readonly IRepository<WeighSessionChoseModel> _weightSsChoseRepo;

        public SaveXNVLGCCommandHandler(IUnitOfWork unitOfWork, IRepository<ComponentExportModel> xnvlgcRepo, IRepository<WeighSessionModel> weightSsRepo,
                                        IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService, IRepository<PurchaseOrderDetailModel> detailPoRepo,
                                        IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<TruckInfoModel> truckRepo,
                                        IRepository<VendorModel> vendorRepo, IRepository<WeighSessionChoseModel> weightSsChoseRepo)
        {
            _unitOfWork = unitOfWork;
            _xnvlgcRepo = xnvlgcRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _detailPoRepo = detailPoRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
            _truckRepo = truckRepo;
            _vendorRepo = vendorRepo;
            _weightSsChoseRepo = weightSsChoseRepo;
        }

        public async Task<bool> Handle(SaveXNVLGCCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query Plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Get query truck info
            var truckInfos = _truckRepo.GetQuery().AsNoTracking();

            //Get query vendor
            var vendors = _vendorRepo.GetQuery().AsNoTracking();

            //Danh sách nhập kho xnvlgc
            var xnvlgcs = await _xnvlgcRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = xnvlgcs.Count() > 0 ? xnvlgcs.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            var index = 1;
            foreach (var item in request.SaveXNVLGCs)
            {

                ////Lấy ra dòng dữ liệu đã lưu
                //var record = await _xnvlgcRepo.FindOneAsync(n => n.ComponentExportId == item.Id);

                ////Lấy ra dòng dữ liệu mapping với đợt cân
                //var weightSsChose = await _weightSsChoseRepo.FindOneAsync(w => w.RecordId == item.Id);

                ////Check status
                //if (item.Status == "DAXOA")
                //{

                //    _weightSsChoseRepo.Remove(weightSsChose);

                //    _xnvlgcRepo.Remove(record);

                //    await _unitOfWork.SaveChangesAsync();

                //    continue;
                //}

                //Check điều kiện lưu
                #region Check điều kiện lưu

                if (!item.ConfirmQty.HasValue || item.ConfirmQty <= 0)
                {
                    throw new ISDException("Confirm Quantity phải lớn hơn 0");
                }

                if (string.IsNullOrEmpty(item.WeightHeadCode))
                {
                    if (!item.BagQuantity.HasValue || item.BagQuantity <= 0)
                    {
                        throw new ISDException("Số lượng bao phải lớn hơn 0");
                    }
                    if (!item.SingleWeight.HasValue || item.SingleWeight <= 0)
                    {
                        throw new ISDException("Đơn trọng phải lớn hơn 0");
                    }
                }
                #endregion

                var ComponentExportId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, ComponentExportId.ToString(), $"{ComponentExportId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XNVLGC");
                }

                //Lấy ra detail po
                var detailPo = !string.IsNullOrEmpty(item.PurchaseOrder) && !string.IsNullOrEmpty(item.PurchaseOrderItem) ?
                                        _detailPoRepo.GetQuery().Include(x => x.PurchaseOrder).Where(x => x.PurchaseOrder.PurchaseOrderCodeInt == long.Parse(item.PurchaseOrder) &&
                                                                                                x.POLine == item.PurchaseOrderItem).FirstOrDefault() : null;

                //Lấy ra cân hiện tại
                var scale = !string.IsNullOrEmpty(item.WeightHeadCode) ? scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode) : null;

                //Lấy ra đợt cân
                var weightSession = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                 weightSs.Where(x => x.ScaleCode == scale.ScaleCode).OrderByDescending(x => x.OrderIndex).FirstOrDefault() : null;

                //Nếu có đợt cân thì lưu vào bảng mapping
                //if (weightSession != null)
                //{
                //    if (weightSsChose != null)
                //        _weightSsChoseRepo.Add(new WeighSessionChoseModel
                //        {
                //            Id = Guid.NewGuid(),
                //            DateKey = weightSession.DateKey,
                //            OrderIndex = weightSession.OrderIndex,
                //            ScaleCode = weightSession.ScaleCode,
                //            RecordId = item.Id
                //        });
                //}

                _xnvlgcRepo.Add(new ComponentExportModel
                {
                    //1. ID
                    ComponentExportId = ComponentExportId,
                    //2. Đầu cân
                    WeightHeadCode = item.WeightHeadCode,
                    //2 WeightSession
                    DateKey = weightSession != null ?
                               weightSession.DateKey : null,
                    OrderIndex = weightSession != null ?
                               weightSession.OrderIndex : null,
                    //4. WeightVote
                    WeightVote = $"X{long.Parse(lastIndex) + index}",
                    //5. Detail Reservation Id
                    PurchaseOrderDetailId = detailPo != null ? detailPo.PurchaseOrderDetailId : null,
                    //6. PlantCode
                    PlantCode = item.Plant,
                    PlantName = plants.FirstOrDefault(x => x.PlantCode == item.Plant).PlantName,
                    //7. MaterialCode
                    MaterialCode = !string.IsNullOrEmpty(item.Material) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "",
                    MaterialName = !string.IsNullOrEmpty(item.Material) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName : "",
                    MaterialCodeInt = !string.IsNullOrEmpty(item.Material) ? long.Parse(item.Material) : null,
                    //Component
                    ComponentCode = !string.IsNullOrEmpty(item.Component) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductCode : "",
                    ComponentName = !string.IsNullOrEmpty(item.Component) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductName : "",
                    ComponentCodeInt = !string.IsNullOrEmpty(item.Component) ? long.Parse(item.Component) : null,
                    //Component item
                    ComponentItem = item.ComponentItem,
                    //8. SlocCode
                    SlocCode = item.Sloc,
                    //9. SlocName
                    SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                    //12. Batch
                    Batch = item.Batch,
                    //15. SL bao - ZSLBAO
                    BagQuantity = item.BagQuantity,
                    //16. Đơn trọng - ZDONTRONG
                    SingleWeight = item.SingleWeight,
                    //17  Weight - ZTLCAN
                    Weight = item.Weight,
                    //18  ConfirmQty - ZCONFQTY
                    ConfirmQty = item.ConfirmQty,
                    //19  QuantityWithPackaging - ZSLCOBAOBI
                    QuantityWithPackaging = item.QuantityWithPackage,
                    //20  QuantityWeitght - ZSOLANCAN
                    QuantityWeight = item.QuantityWeight,
                    //21  Số phương tiện - ZSOPT
                    VehicleCode = item.VehicleCode,
                    //Vendor name
                    VendorCode = item.VendorCode,
                    VendorName = !string.IsNullOrEmpty(item.VendorCode) ? vendors.FirstOrDefault(x => x.VendorCode == item.VendorCode).VendorName : null,
                    //21  Description
                    Description = item.Description,
                    //Input weight
                    InputWeight = item.InputWeight,
                    //Output weight
                    OutputWeight = item.OutputWeight,
                    //Order quantity
                    TotalQuantity = detailPo != null ? detailPo.OpenQuantity : 0,
                    //Order unit
                    OrderUnit = detailPo != null ? detailPo.Unit : null,
                    //Requirement quantity
                    RequirementQuantity = item.RequirementQuantity ?? 0,
                    //Requirement Unit
                    RequirementUnit = item.RequirementUnit,
                    //21  Image
                    Image = string.IsNullOrEmpty(imgPath) ? null : imgPath,
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = weightSession != null ?
                               weightSession.StartTime : null,
                    //18  EndTime
                    EndTime = DateTime.Now,
                    //Số xe tải
                    TruckInfoId = item.TruckInfoId,
                    TruckNumber = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(t => t.TruckInfoId == item.TruckInfoId).TruckNumber : null,
                    //24  CreateTime
                    CreateTime = DateTime.Now,
                    //25  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //28  Actived
                    Actived = true

                });

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
