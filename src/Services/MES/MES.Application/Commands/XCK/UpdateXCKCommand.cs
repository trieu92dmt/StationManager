using Core.Extensions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace MES.Application.Commands.XCK
{
    public class UpdateXCKCommand : IRequest<ApiResponse>
    {
        public List<UpdateXCK> UpdateXCKs { get; set; } = new List<UpdateXCK>();
    }

    public class UpdateXCK
    {
        //1. XCK ID
        public Guid XCKId { get; set; }
        //2. Plant
        public string Plant { get; set; }
        //3. Reservation
        public string Reservation { get; set; }
        //4. Reservation Item
        public string ReservationItem { get; set; }
        //5. Material
        public string Material { get; set; }
        //6. Sloc
        public string Sloc { get; set; }
        //7. Batch
        public string Batch { get; set; }
        //MVT
        public string MovementType { get; set; }
        //SL bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //7. Đầu cân
        public string WeightHeadCode { get; set; }
        //8. Trọng lượng cân
        public decimal? Weight { get; set; }
        //9. Số lần cân
        public int? QuantityWeight { get; set; }
        //10. UoM
        public string Unit { get; set; }
        //11. Số phiếu cân
        public string WeightVote { get; set; }
        //12. Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //13. Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //14. Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //15. Sl kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //16. Số phương tiện
        public string VehicleCode { get; set; }
        //17. Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //17. Ghi chú
        public string Description { get; set; }
        //18. Hình ảnh
        public string Image { get; set; }
        public string NewImage { get; set; }
        //19. Đánh dấu xóa
        public bool? isDelete { get; set; }
        //Create By
        public Guid? CreateBy { get; set; }
        //Create On
        public DateTime? CreateOn { get; set; }
    }

    public class UpdateXCKCommandHandler : IRequestHandler<UpdateXCKCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WarehouseExportTransferModel> _xckRepo;
        private readonly IRepository<DetailReservationModel> _detailRsRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;

        public UpdateXCKCommandHandler(IUnitOfWork unitOfWork, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo, 
                                      IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService, IRepository<WarehouseExportTransferModel> xckRepo,
                                      IRepository<DetailReservationModel> detailRsRepo)
        {
            _unitOfWork = unitOfWork;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
            _xckRepo = xckRepo;
            _detailRsRepo = detailRsRepo;
        }

        public async Task<ApiResponse> Handle(UpdateXCKCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật xuất chuyển kho")
            };

            //Data xuất chuyển kho
            var xcks = _xckRepo.GetQuery();

            //Data reservation detail
            var reserDetails = _detailRsRepo.GetQuery().Include(x => x.Reservation).AsNoTracking();

            //Data material
            var material = _prdRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Data DimDate
            var dimDate = _dimDateRepo.GetQuery().AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateXCKs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NKHTIDs = x.Value.Select(v => v.XCKId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = xcks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = xcks.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.WarehouseTransferId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = xcks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = xcks.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.WarehouseTransferId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateXCKs)
            {
                //Check tồn tại xck
                var xck = await xcks.FirstOrDefaultAsync(x => x.WarehouseTransferId == item.XCKId);

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.XCKId.ToString(), $"{item.XCKId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XCK");
                }

                //Lấy ra detail reservation
                var rsDetail = !string.IsNullOrEmpty(item.Reservation) && !string.IsNullOrEmpty(item.ReservationItem) ?
                               reserDetails.FirstOrDefault(x => x.Reservation.ReservationCodeInt == long.Parse(item.Reservation) && x.ReservationItem == item.ReservationItem &&
                                                                x.Reservation.Plant == item.Plant) : null;

                //Check reservation có khớp với material
                if (rsDetail != null && rsDetail.MaterialCodeInt != long.Parse(item.Material))
                {
                    response.IsSuccess = false;
                    response.Message = $"Reservation và Material Không mapping với nhau";

                    return response;
                }

                //Chưa có thì tạo mới
                if (xck == null)
                {
                    _xckRepo.Add(new WarehouseExportTransferModel
                    {
                        //Id
                        WarehouseTransferId = item.XCKId,
                        //Reservation - reservation item
                        DetailReservationId = rsDetail != null ? rsDetail.DetailReservationId : null,
                        //Plant
                        PlantCode = item.Plant,
                        //Material
                        MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                        MaterialCodeInt = long.Parse(item.Material),
                        MaterialName = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName,
                        //Batch
                        Batch = item.Batch,
                        MovementType = item.MovementType,
                        //UoM
                        Unit = item.Unit,
                        //Sl bao
                        BagQuantity = item.BagQuantity,
                        //Đơn trọng
                        SingleWeight = item.SingleWeight,
                        //Số phiếu cân
                        WeightVote = item.WeightVote,
                        //Weight Head Code
                        WeightHeadCode = item.WeightHeadCode,
                        //Vehicle code
                        VehicleCode = item.VehicleCode,
                        //Output weight
                        OutputWeight = item.OutputWeight,
                        //Weight
                        Weight = item.Weight,
                        //Confirm Quantity
                        ConfirmQty = item.ConfirmQty,
                        //Số lượng kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackage,
                        //Số lần cân
                        QuantityWeitght = item.QuantityWeight,
                        //Ghi chú
                        Description = item.Description,
                        //Hình ảnh
                        Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                        //Start time
                        StartTime = item.StartTime,
                        //End time
                        EndTime = item.EndTime,
                        //Sloc
                        SlocCode = item.Sloc,
                        SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc)?.StorageLocationName,
                        //Trạng thái
                        Status = item.isDelete == true ? "DEL" : "NOT",
                        CreateBy = TokenExtensions.GetAccountId(),
                        CreateTime = DateTime.Now,
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Detail reservation detail id
                    xck.DetailReservationId = rsDetail != null ? rsDetail.DetailReservationId : null;
                    //Material 
                    xck.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    xck.MaterialCodeInt = long.Parse(item.Material);
                    xck.MaterialName = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName;
                    //Storage Location
                    xck.SlocCode = item.Sloc;
                    //Sloc Name
                    xck.SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc)?.StorageLocationName;
                    //Batch
                    xck.Batch = item.Batch;
                    //Vehicle code
                    xck.VehicleCode = item.VehicleCode;
                    //MVT
                    xck.MovementType = item.MovementType;
                    //Số lượng bao
                    xck.BagQuantity = item.BagQuantity;
                    //Đơn trọng
                    xck.SingleWeight = item.SingleWeight;
                    //Trọng lượng cân
                    xck.Weight = item.Weight;
                    //Confirm Quantity
                    xck.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    xck.QuantityWithPackaging = item.QuantityWithPackage;
                    //Output weight
                    xck.OutputWeight = item.OutputWeight;
                    //Ghi chú
                    xck.Description = item.Description;
                    //Hình ảnh
                    xck.Image = string.IsNullOrEmpty(imgPath) ? xck.Image : imgPath;
                    xck.LastEditBy = TokenExtensions.GetAccountId();
                    xck.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        xck.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        xck.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
