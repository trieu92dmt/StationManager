using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Commands.XCK
{
    public class SaveXCKCommand : IRequest<bool>
    {
        public List<SaveXCK> SaveXCKs { get; set; } = new List<SaveXCK>();
    }

    public class SaveXCK
    {
        //1. Plant
        public string Plant { get; set; }
        //2. Reservation
        public string Reservation { get; set; }
        //3. Reservation Item
        public string ReservationItem { get; set; }
        //4. Material
        public string Material { get; set; }
        //5. Material Desc
        public string MaterialDesc { get; set; }
        //6. Movement Type
        public string MovementType { get; set; }
        //7. Stor. Loc
        public string Sloc { get; set; }
        //8. Receiving Storage Location
        public string ReceivingSloc { get; set; }
        //9. Batch
        public string Batch { get; set; }
        //10. Unit
        public string Unit { get; set; }
        //11. SL bao
        public decimal? BagQuantity { get; set; }
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
        //22. Số xe tải
        public Guid? TruckInfoId { get; set; }
        public string TruckNumber { get; set; }
        //23. Số cân đầu vào
        public string InputWeight { get; set; }
        //24. Số cân đầu ra
        public string OutputWeight { get; set; }
        //25. Ghi chú
        public string Description { get; set; }
        //24. Hình ảnh
        public string Image { get; set; }
        //25. Trạng thái
        public string Status { get; set; }
    }

    public class SaveXCKCommandHandler : IRequestHandler<SaveXCKCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WarehouseTransferModel> _xckRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<DetailReservationModel> _detailRsRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<PlantModel> _plantRepo;

        public SaveXCKCommandHandler(IUnitOfWork unitOfWork, IRepository<WarehouseTransferModel> xckRepo, IRepository<WeighSessionModel> weightSsRepo,
                                     IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService, IRepository<DetailReservationModel> detailRsRepo,
                                     IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo)
        {
            _unitOfWork = unitOfWork;
            _xckRepo = xckRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _detailRsRepo = detailRsRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
        }

        public async Task<bool> Handle(SaveXCKCommand request, CancellationToken cancellationToken)
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

            //Danh sách nhập kho xck
            var xcks = await _xckRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = xcks.Count() > 0 ? xcks.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            var index = 1;
            foreach (var item in request.SaveXCKs)
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

                var WarehouseTranferId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, WarehouseTranferId.ToString(), $"{WarehouseTranferId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XCK");
                }

                //Lấy ra detail reservation
                var detailRes = _detailRsRepo.GetQuery().Include(x => x.Reservation).Where(x => x.Reservation.ReservationCodeInt == long.Parse(item.Reservation) &&
                                                                                                x.ReservationItem == item.ReservationItem).FirstOrDefault();

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                _xckRepo.Add(new WarehouseTransferModel
                {
                    //1. Warehouse Tranfer ID
                    WarehouseTransferId = WarehouseTranferId,
                    //2. Đầu cân
                    WeightHeadCode = item.WeightHeadCode,
                    //3. WeightId
                    WeightId = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.WeighSessionID : null,
                    //4. WeightVote
                    WeightVote = $"X{long.Parse(lastIndex) + index}",
                    //5. Detail Reservation Id
                    DetailReservationId = detailRes != null ? detailRes.DetailReservationId : null,
                    //6. PlantCode
                    PlantCode = item.Plant,
                    PlantName = plants.FirstOrDefault(x => x.PlantCode == item.Plant).PlantName,
                    //7. MaterialCode
                    MaterialCode = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material) && x.PlantCode == item.Plant).ProductCode,
                    MaterialCodeInt = long.Parse(item.Material),
                    //8. SlocCode
                    SlocCode = item.Sloc,
                    //9. SlocName
                    SlocName = string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                    //10. ReceivingSlocCode - UMLGO - ZUMLGOTXT
                    ReceivingSlocCode = item.ReceivingSloc,
                    //11. ReceivingSlocName
                    ReceivingSlocName = string.IsNullOrEmpty(item.ReceivingSloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.ReceivingSloc).StorageLocationName : "",
                    //12. Batch - CHARG
                    Batch = item.Batch,
                    //13. Movement Type
                    MovementType = item.MovementType,
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
                    QuantityWeitght = item.QuantityWeight,
                    //21  Số phương tiện - ZSOPT
                    VehicleCode = item.VehicleCode,
                    //21  Description
                    Description = item.Description,
                    //21  Image
                    Image = !string.IsNullOrEmpty(imgPath) ? imgPath : null,
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.StartTime : null,
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
