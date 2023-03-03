using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
using Core.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace MES.Application.Commands.XK
{
    public class UpdateXKCommand : IRequest<ApiResponse>
    {
        public List<UpdateXKData> UpdateXKDatas { get; set; } = new List<UpdateXKData>();
    }
    public class UpdateXKData
    {
        //XKID 
        public Guid XKId { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Reservation item
        public string ReservationItem { get; set; }
        //Plant
        public string Plant { get; set; }
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
        //Uom
        public string Unit { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Customer
        public string Customer { get; set; }
        //Special Stock
        public string SpecialStock { get; set; }
        //Confirm quantity
        public decimal? ConfirmQuantity { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        public string NewImage { get; set; }
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
        //Create By
        public Guid? CreateBy { get; set; }
        //Create On
        public DateTime? CreateOn { get; set; }
    }

    public class UpdateXKCommandHandler : IRequestHandler<UpdateXKCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OtherExportModel> _xkRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        private readonly IRepository<DetailReservationModel> _dtResRepo;

        public UpdateXKCommandHandler(IUnitOfWork unitOfWork, IRepository<OtherExportModel> xkRepo, IUtilitiesService utilitiesService,
                                    IRepository<ScaleModel> scaleRepo, IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo,
                                    IRepository<WeighSessionModel> weightSsRepo, IRepository<TruckInfoModel> truckRepo, IRepository<DetailReservationModel> dtResRepo)
        {
            _unitOfWork = unitOfWork;
            _xkRepo = xkRepo;
            _utilitiesService = utilitiesService;
            _scaleRepo = scaleRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _weightSsRepo = weightSsRepo;
            _truckRepo = truckRepo;
            _dtResRepo = dtResRepo;
        }

        public async Task<ApiResponse> Handle(UpdateXKCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật dữ liệu xuất khác")
            };

            //Data nhập khác
            var xks = _xkRepo.GetQuery();

            //Data material
            var material = _prodRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Get query reservation
            var dtRes = _dtResRepo.GetQuery().Include(x => x.Reservation).AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateXKDatas.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     XKIds = x.Value.Select(v => v.XKId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQuantity),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = xks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = xks.Where(x => x.WeightVote == item.WeightVote && !item.XKIds.Contains(x.OtherExportId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = xks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = xks.Where(x => x.WeightVote == item.WeightVote && !item.XKIds.Contains(x.OtherExportId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateXKDatas)
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

                //Check trường hợp nhập res nhưng không có res item
                if (!string.IsNullOrEmpty(item.Reservation) && string.IsNullOrEmpty(item.ReservationItem))
                {
                    response.IsSuccess = false;
                    response.Message = $"Vui lòng nhập Reservation Item";

                    return response;
                }

                //Check tồn tại xk
                var xk = await xks.FirstOrDefaultAsync(x => x.OtherExportId == item.XKId);

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.XKId.ToString(), $"{item.XKId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XK");
                }

                //Dữ liệu cân
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy data reservation
                var reservation = !string.IsNullOrEmpty(item.Reservation) && !string.IsNullOrEmpty(item.ReservationItem) ?
                                    dtRes.FirstOrDefault(d => d.Reservation.ReservationCodeInt == long.Parse(item.Reservation) &&
                                                             d.ReservationItem == item.ReservationItem) : null;

                //Chưa có thì tạo mới
                if (xk == null)
                {
                    _xkRepo.Add(new OtherExportModel
                    {
                        //Id
                        OtherExportId = item.XKId,
                        //Reservation id
                        DetailReservationId = reservation != null ? reservation.DetailReservationId : null,
                        //Plant
                        PlantCode = item.Plant,
                        //Material
                        MaterialCode = !string.IsNullOrEmpty(item.Plant) ? material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "",
                        MaterialCodeInt = long.Parse(item.Material),
                        //Đầu cân
                        WeightHeadCode = item.WeightHeadCode,
                        //Số phiếu cân
                        WeightVote = item.WeightVote,
                        //Trọng lượng cân
                        Weight = item.Weight,
                        //Confỉm quantity
                        ConfirmQty = item.ConfirmQuantity,
                        //Cusstomer
                        Customer = item.Customer,
                        //Special Stock
                        SpecialStock = item.SpecialStock,
                        //Số lượng kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackage,
                        //Số phương tiện
                        VehicleCode = item.VehicleCode,
                        //Số lần cân
                        QuantityWeight = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.TotalNumberOfWeigh : null,
                        //UOM
                        UOM = item.Unit,
                        //Số cân đàu ra
                        OutputWeight = item.OutputWeight,
                        //Ghi chú
                        Description = item.Description,
                        //Hình ảnh
                        Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                        //Thời gian bắt đầu
                        StartTime = item.StartTime,
                        //Thời gian kết thúc
                        EndTime = item.EndTime,
                        //Sloc
                        SlocCode = item.Sloc,
                        SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                        //Đánh dấu xóa
                        Status = item.isDelete == true ? "DEL" : "NOT",
                        CreateBy = item.CreateBy,
                        CreateTime = DateTime.Now,
                        LastEditBy = TokenExtensions.GetAccountId(),
                        LastEditTime = DateTime.Now,
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Detail Res Id
                    xk.DetailReservationId = reservation != null ? reservation.DetailReservationId : null;
                    //Material Code
                    xk.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    //Material Code Int
                    xk.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    xk.SlocCode = item.Sloc;
                    //Sloc Name
                    xk.SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "";
                    //Customer
                    xk.Customer = item.Customer;
                    //Special Stock
                    xk.SpecialStock = item.SpecialStock;
                    //Confirm Quantity
                    xk.ConfirmQty = item.ConfirmQuantity;
                    //Sl kèm bao bì
                    xk.QuantityWithPackaging = item.QuantityWithPackage;
                    //Số phương tiện
                    xk.VehicleCode = item.VehicleCode;
                    //Số cân đầu ra
                    xk.OutputWeight = item.OutputWeight;
                    //Ghi chú
                    xk.Description = item.Description;
                    //Hình ảnh
                    xk.Image = string.IsNullOrEmpty(imgPath) ? xk.Image : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath);
                    xk.LastEditBy = TokenExtensions.GetAccountId();
                    xk.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                    {
                        xk.Status = "DEL";
                    }
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        xk.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
