using Core.Exceptions;
using Core.Extensions;
using Core.Implements;
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

namespace MES.Application.Commands.NK
{
    public class SaveNKCommand : IRequest<bool>
    {
        public List<SaveNKData> SaveNKDatas { get; set; } = new List<SaveNKData>();
    }

    public class SaveNKData
    {
        //Plant
        public string Plant { get; set; }
        //Customer
        public string Customer { get; set; }
        //Material
        public string Material { get; set; }
        //Unit
        public string Unit { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQuantity { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //SpecialStock
        public string SpecialStock { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
    }

    public class SaveNKCommandHandler : IRequestHandler<SaveNKCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OtherImportModel> _nkRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;

        public SaveNKCommandHandler(IUnitOfWork unitOfWork, IRepository<OtherImportModel> nkRepo, IUtilitiesService utilitiesService, 
                                    IRepository<ScaleModel> scaleRepo, IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, 
                                    IRepository<WeighSessionModel> weightSsRepo,IRepository<TruckInfoModel> truckRepo)
        {
            _unitOfWork = unitOfWork;
            _nkRepo = nkRepo;
            _utilitiesService = utilitiesService;
            _scaleRepo = scaleRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _weightSsRepo = weightSsRepo;
            _truckRepo = truckRepo;
        }

        public async Task<bool> Handle(SaveNKCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query truck info
            var truckInfos = _truckRepo.GetQuery().AsNoTracking();

            //Get query nk
            var nks = _nkRepo.GetQuery().AsNoTracking();

            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nks.Count() > 0 ? nks.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            var index = 1;
            foreach (var item in request.SaveNKDatas)
            {
                //Check điều kiện lưu
                #region Check điều kiện lưu

                if (!item.ConfirmQuantity.HasValue || item.ConfirmQuantity <= 0)
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

                var OtherImportId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, OtherImportId.ToString(), $"{OtherImportId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NK");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);


                _nkRepo.Add(new OtherImportModel
                {
                    //1 NK Id
                    OtherImportId = OtherImportId,
                    //3 PlantCode
                    PlantCode = item.Plant,
                    //Customer
                    Customer = item.Customer, 
                    //4 MaterialCode
                    MaterialCode = !string.IsNullOrEmpty(item.Material) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material) && x.PlantCode == item.Plant).ProductCode : "",
                    MaterialCodeInt = long.Parse(item.Material), 
                    //5   WeightId
                    WeightSessionId = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.WeighSessionID : null,
                    //6 WeightVote
                    WeightVote = $"N{long.Parse(lastIndex) + index}",
                    //7   BagQuantity
                    BagQuantity = item.BagQuantity,
                    //8   SingleWeight
                    SingleWeight = item.SingleWeight,
                    //9  WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,
                    //10  Weight
                    Weight = item.Weight,
                    //11  ConfirmQty
                    ConfirmQty = item.ConfirmQuantity,
                    //12  QuantityWithPackaging
                    QuantityWithPackaging = item.QuantityWithPackage,
                    //VehicleCode
                    VehicleCode = item.VehicleCode,
                    //Special Stock
                    SpecialStock = item.SpecialStock,
                    //Số lần cân
                    QuantityWeight = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.TotalNumberOfWeigh : null,
                    //UOM
                    UOM = item.Unit,
                    //Số cân đầu vào
                    InputWeight = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(x => x.TruckInfoId == item.TruckInfoId).InputWeight : null,
                    //Số cân đầu ra
                    OutputWeight = item.OutputWeight,
                    //14  Description
                    Description = item.Description,
                    //15  Image
                    Image = !string.IsNullOrEmpty(imgPath) ? imgPath : null,
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.StartTime : null,
                    //18  EndTime
                    EndTime = DateTime.Now,
                    //21  SlocCode
                    SlocCode = item.Sloc,
                    //22  SlocName
                    SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                    //Truckinfo
                    TruckInfoId = item.TruckInfoId,
                    //TruckNumber
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
