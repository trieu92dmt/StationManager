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
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NCK
{
    public class SaveNCKCommand : IRequest<bool>
    {
        public List<SaveNCK> SaveNCKs { get; set; } = new List<SaveNCK>();
    }

    public class SaveNCK
    {
        //Plant
        public string Plant { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Material Doc
        public string MaterialDoc { get; set; }
        //Material Doc Item
        public string MaterialDocItem { get; set; }
        //Material
        public string Material { get; set; }
        //UoM
        public string Unit { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Mã đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //Số lượng kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số xe tải
        public string TruckNumber { get; set; }
        public Guid? TruckInfoId { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
    }

    public class SaveNCKCommandHandler : IRequestHandler<SaveNCKCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WarehouseImportTransferModel> _nckRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<MaterialDocumentModel> _matDocRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        private readonly IRepository<ReservationModel> _resRepo;

        public SaveNCKCommandHandler(IUnitOfWork unitOfWork, IRepository<WarehouseImportTransferModel> nckRepo, IRepository<WeighSessionModel> weightSsRepo,
                                     IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService, IRepository<MaterialDocumentModel> matDocRepo,
                                     IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo,
                                     IRepository<TruckInfoModel> truckRepo, IRepository<ReservationModel> resRepo)
        {
            _unitOfWork = unitOfWork;
            _nckRepo = nckRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _matDocRepo = matDocRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
            _truckRepo = truckRepo;
            _resRepo = resRepo;
        }

        public async Task<bool> Handle(SaveNCKCommand request, CancellationToken cancellationToken)
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

            //Get query mat doc
            var matDocs = _matDocRepo.GetQuery().AsNoTracking();

            //Get quert reservation
            var reservations = _resRepo.GetQuery().AsNoTracking();  

            //Danh sách nhập kho nck
            var ncks = await _nckRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = ncks.Count() > 0 ? ncks.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            var index = 1;
            foreach (var item in request.SaveNCKs)
            {
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

                var WarehouseImportTranferId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, WarehouseImportTranferId.ToString(), $"{WarehouseImportTranferId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NCK");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra đợt cân
                var weightSession = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                 weightSs.Where(x => x.ScaleCode == scale.ScaleCode).OrderByDescending(x => x.OrderIndex).FirstOrDefault() : null;

                //Lấy material doc
                var matDoc = !string.IsNullOrEmpty(item.MaterialDoc) && !string.IsNullOrEmpty(item.MaterialDocItem) ?
                                    matDocs.FirstOrDefault(m => m.MaterialDocCode == item.MaterialDoc && m.MaterialDocItem == item.MaterialDocItem) : null;

                _nckRepo.Add(new WarehouseImportTransferModel
                {
                    //1. Warehouse import Tranfer ID
                    WarehouseImportTransferId = WarehouseImportTranferId,
                    //2. Đầu cân
                    WeightHeadCode = item.WeightHeadCode,
                    //2 WeightSession
                    DateKey = weightSession != null ?
                               weightSession.DateKey : null,
                    OrderIndex = weightSession != null ?
                               weightSession.OrderIndex : null,
                    //4. WeightVote
                    WeightVote = $"X{long.Parse(lastIndex) + index}",
                    //Material Doc id
                    MaterialDocId = matDoc != null ? matDoc.MaterialDocId : null,
                    //5. Reservation
                    Reservation = !string.IsNullOrEmpty(item.Reservation) ? reservations.FirstOrDefault(x => x.ReservationCodeInt == long.Parse(item.Reservation)).ReservationCode : null,
                    ReservationInt = !string.IsNullOrEmpty(item.Reservation) ? long.Parse(item.Reservation) : null,
                    //6. PlantCode
                    PlantCode = item.Plant,
                    PlantName = plants.FirstOrDefault(x => x.PlantCode == item.Plant).PlantName,
                    //7. MaterialCode
                    MaterialCode = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                    MaterialCodeInt = long.Parse(item.Material),
                    //8. SlocCode
                    SlocCode = item.Sloc,
                    //9. SlocName
                    SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                    //12. Batch - CHARG
                    Batch = item.Batch,
                    //14. UOM
                    Unit = item.Unit,
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
                    QuantityWeitght = weightSession != null ?
                               weightSession.TotalNumberOfWeigh : null,
                    //Số xe tải
                    TruckInfoId = item.TruckInfoId,
                    TruckNumber = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(t => t.TruckInfoId == item.TruckInfoId).TruckNumber : null,
                    //Số cân đầu vào
                    InputWeight = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(x => x.TruckInfoId == item.TruckInfoId).InputWeight : null,
                    //Số cân đầu ra
                    OutputWeight = item.OutputWeight,
                    //21  Số phương tiện - ZSOPT
                    VehicleCode = item.VehicleCode,
                    //21  Description
                    Description = item.Description,
                    //21  Image
                    Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = weightSession != null ?
                               weightSession.StartTime : null,
                    //18  EndTime
                    EndTime = DateTime.Now,

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
