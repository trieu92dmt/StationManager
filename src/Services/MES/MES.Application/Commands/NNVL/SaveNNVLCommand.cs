using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NNVL
{
    public class SaveNNVLCommand : IRequest<bool>
    {
        public List<SaveNNVL> SaveNNVLs { get; set; } = new List<SaveNNVL>();
    }

    public class SaveNNVL
    {
        //public Guid Id { get; set; }
        //Plant
        public string Plant { get; set; }
        //Vendor
        public string Vendor { get; set; }
        //Material
        public string Material { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //Sl kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //UOM
        public string Unit { get; set; }
    }

    public class SaveNNVLCommandHandler : IRequestHandler<SaveNNVLCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ComponentImportModel> _nnvlgcRepo;
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
        public SaveNNVLCommandHandler(IUnitOfWork unitOfWork, IRepository<ComponentImportModel> nnvlgcRepo, IRepository<WeighSessionModel> weightSsRepo,
                                      IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService, IRepository<PurchaseOrderDetailModel> detailPoRepo,
                                      IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<TruckInfoModel> truckRepo,
                                      IRepository<VendorModel> vendorRepo, IRepository<WeighSessionChoseModel> weightSsChoseRepo)
        {
            _unitOfWork = unitOfWork;
            _nnvlgcRepo = nnvlgcRepo;
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

        public async Task<bool> Handle(SaveNNVLCommand request, CancellationToken cancellationToken)
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

            //Danh sách nhập kho nnvlgc
            var xnvlgcs = await _nnvlgcRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = xnvlgcs.Count() > 0 ? xnvlgcs.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            var index = 1;
            foreach (var item in request.SaveNNVLs)
            {

                ////Lấy ra dòng dữ liệu đã lưu
                //var record = await _nnvlgcRepo.FindOneAsync(n => n.ComponentImportId == item.Id);

                ////Lấy ra dòng dữ liệu mapping với đợt cân
                //var weightSsChose = await _weightSsChoseRepo.FindOneAsync(w => w.RecordId == item.Id);

                ////Check status
                //if (item.Status == "DAXOA")
                //{

                //    _weightSsChoseRepo.Remove(weightSsChose);

                //    _nnvlgcRepo.Remove(record);

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

                var ComponentImportId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, ComponentImportId.ToString(), $"{ComponentImportId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NNVLGC");
                }

                //Lấy ra cân hiện tại
                var scale = !string.IsNullOrEmpty(item.WeightHeadCode) ? scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode) : null;

                //Lấy ra đợt cân
                var weightSession = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                 weightSs.Where(x => x.ScaleCode == scale.ScaleCode).OrderByDescending(x => x.OrderIndex).FirstOrDefault() : null;

                ////Nếu có đợt cân thì lưu vào bảng mapping
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

                _nnvlgcRepo.Add(new ComponentImportModel
                {
                    //1. ID
                    ComponentImportId = ComponentImportId,
                    //2. Đầu cân
                    WeightHeadCode = item.WeightHeadCode,
                    //2 WeightSession
                    DateKey = weightSession != null ?
                               weightSession.DateKey : null,
                    OrderIndex = weightSession != null ?
                               weightSession.OrderIndex : null,
                    //4. WeightVote
                    WeightVote = $"X{long.Parse(lastIndex) + index}",
                    //6. PlantCode
                    PlantCode = item.Plant,
                    PlantName = plants.FirstOrDefault(x => x.PlantCode == item.Plant).PlantName,
                    //7. MaterialCode
                    MaterialCode = !string.IsNullOrEmpty(item.Material) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "",
                    MaterialName = !string.IsNullOrEmpty(item.Material) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName : "",
                    MaterialCodeInt = !string.IsNullOrEmpty(item.Material) ? long.Parse(item.Material) : null,
                    //8. SlocCode
                    SlocCode = item.Sloc,
                    //9. SlocName
                    SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                    //12. Batch
                    Batch = item.Batch,
                    //15. SL bao 
                    BagQuantity = item.BagQuantity,
                    //16. Đơn trọng 
                    SingleWeight = item.SingleWeight,
                    //17  Weight - ZTLCAN
                    Weight = item.Weight,
                    //18  ConfirmQty
                    ConfirmQty = item.ConfirmQty,
                    //19  QuantityWithPackaging 
                    QuantityWithPackaging = item.QuantityWithPackage,
                    //20  QuantityWeitght
                    QuantityWeight = item.QuantityWeight,
                    //21  Số phương tiện
                    VehicleCode = item.VehicleCode,
                    //Unit
                    UOM = item.Unit,
                    //Vendor name
                    VendorCode = item.Vendor,
                    VendorName = !string.IsNullOrEmpty(item.Vendor) ? vendors.FirstOrDefault(x => x.VendorCode == item.Vendor).VendorName : null,
                    //21  Description
                    Description = item.Description,
                    //Input weight
                    InputWeight = item.InputWeight,
                    //Output weight
                    OutputWeight = item.OutputWeight,
                    //21  Image
                    Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
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
