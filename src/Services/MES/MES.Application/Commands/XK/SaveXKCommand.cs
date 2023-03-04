using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Commands.XK
{
    public class SaveXKCommand : IRequest<bool>
    {
        public List<DataSaveXK> DataSaveXKs { get; set; } = new List<DataSaveXK>();
    }

    public class DataSaveXK 
    {
        //Plant
        public string Plant { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Reservation item
        public string ReservationItem { get; set; }
        //Material
        public string Material { get; set; }
        //Unit
        public string Unit { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Rec Sloc
        public string ReceivingSloc { get; set; }
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
        //Confirm qty
        public decimal? ConfirmQty { get; set; }
        //Số lượng kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Customer
        public string Customer { get; set; }
        //Special Stock
        public string SpecialStock { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Ghi chus
        public string Description { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
    }

    public class SaveXKCommandHandler : IRequestHandler<SaveXKCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<OtherExportModel> _xkRepo;
        private readonly IRepository<DetailReservationModel> _dtResRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        public SaveXKCommandHandler(IUnitOfWork unitOfWork, IRepository<WeighSessionModel> weightSsRepo, IRepository<TruckInfoModel> truckRepo,
                                    IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService,
                                    IRepository<ProductModel> prodRepo, IRepository<OtherExportModel> xkRepo,
                                    IRepository<DetailReservationModel> dtResRepo, IRepository<StorageLocationModel> slocRepo)
        {
            _unitOfWork = unitOfWork;
            _weightSsRepo = weightSsRepo;
            _truckRepo = truckRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _prodRepo = prodRepo;
            _xkRepo = xkRepo;
            _dtResRepo = dtResRepo;
            _slocRepo = slocRepo;
        }

        public async Task<bool> Handle(SaveXKCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query reservation
            var dtRes = _dtResRepo.GetQuery().Include(x => x.Reservation).AsNoTracking();

            //Get query truck info
            var truckInfos = _truckRepo.GetQuery().AsNoTracking();

            //Danh sách nhập kho xk
            var xks = await _xkRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = xks.Count() > 0 ? xks.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            var index = 1;
            foreach (var item in request.DataSaveXKs)
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

                var OtherExportId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, OtherExportId.ToString(), $"{OtherExportId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XK");
                }

                //Lấy od detail
                var detailRes = dtRes.FirstOrDefault(x => !string.IsNullOrEmpty(item.Reservation) && !string.IsNullOrEmpty(item.ReservationItem) ?
                                                             x.Reservation.ReservationCodeInt == long.Parse(item.Reservation) &&
                                                             x.ReservationItem == item.ReservationItem : false);

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra đợt cân
                var weightSession = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                 weightSs.Where(x => x.ScaleCode == scale.ScaleCode).OrderByDescending(x => x.OrderIndex).FirstOrDefault() : null;

                _xkRepo.Add(new OtherExportModel
                {
                    //1 XK Id
                    OtherExportId = OtherExportId,
                    //2 Detail reservation id
                    DetailReservationId = detailRes != null ? detailRes.DetailReservationId : null,
                    //3 PlantCode
                    PlantCode = item.Plant,
                    //4   MaterialCode
                    MaterialCode = !string.IsNullOrEmpty(item.Material) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "",
                    MaterialCodeInt = long.Parse(item.Material),
                    //Batch
                    Batch = item.Batch,
                    //2 WeightSession
                    DateKey = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSession.DateKey : null,
                    OrderIndex = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSession.OrderIndex : null,
                    //6 WeightVote
                    WeightVote = $"X{long.Parse(lastIndex) + index}",
                    //7   BagQuantity
                    BagQuantity = item.BagQuantity,
                    //8   SingleWeight
                    SingleWeight = item.SingleWeight,
                    //9  WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,
                    //10  Weight
                    Weight = item.Weight,
                    //11  ConfirmQty
                    ConfirmQty = item.ConfirmQty,
                    //12  QuantityWithPackaging
                    QuantityWithPackaging = item.QuantityWithPackage,
                    //VehicleCode
                    VehicleCode = item.VehicleCode,
                    //Số lần cân
                    QuantityWeight = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSession.TotalNumberOfWeigh : null,
                    //UOM
                    UOM = item.Unit,
                    //Customer
                    Customer = item.Customer,
                    //Special Stock
                    SpecialStock = item.SpecialStock,
                    //Số cân đầu vào
                    InputWeight = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(x => x.TruckInfoId == item.TruckInfoId).InputWeight : null,
                    //Số cân đầu ra
                    OutputWeight = item.OutputWeight,
                    //14  Description
                    Description = item.Description,
                    //15  Image
                    Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSession.StartTime : null,
                    //18  EndTime
                    EndTime = DateTime.Now,
                    //21  SlocCode
                    SlocCode = item.Sloc,
                    //22  SlocName
                    SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                    //Rec sloc
                    ReceivingSlocCode = item.ReceivingSloc,
                    ReceivingSlocName = !string.IsNullOrEmpty(item.ReceivingSloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.ReceivingSloc).StorageLocationName : "",
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
