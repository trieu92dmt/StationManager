using Core.Interfaces.Databases;
using Core.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Commands.OutboundDelivery
{
    public class UpdateGoodsReturnCommand : IRequest<ApiResponse>
    {
        public List<UpdateGoodsReturn> UpdateGoodsReturns { get; set; } = new List<UpdateGoodsReturn>();
    }

    public class UpdateGoodsReturn
    {
        //Id nhập kho hàng trả
        public Guid NKHTId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Purchase Order
        public string ODCode { get; set; }
        //PO Item
        public string ODItem { get; set; }
        //Material
        public string Material { get; set; }
        //Storage Location
        public string StorageLocation { get; set; }
        //Batch
        public string Batch { get; set; }
        //Sl bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackaging { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Id cân xe tải
        public Guid? TruckInfoId { get; set; }
        //Số xe tải
        public string TruckQty { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //DocumentDate
        public DateTime? DocumentDate { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Create By
        public Guid? CreateBy { get; set; }
        //Create On
        public DateTime? CreateOn { get; set; }
        //Change By
        public Guid? ChangeBy { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        public string NewImage { get; set; }
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
    }

    public class UpdateGoodsReturnCommandHandler : IRequestHandler<UpdateGoodsReturnCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoodsReturnModel> _nkhtRepo;
        private readonly IRepository<OutboundDeliveryModel> _odRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailDORepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;

        public UpdateGoodsReturnCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReturnModel> nkhtRepo, IRepository<OutboundDeliveryModel> odRepo,
                                               IRepository<DetailOutboundDeliveryModel> detailDORepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                                               IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService)
        {
            _unitOfWork = unitOfWork;
            _nkhtRepo = nkhtRepo;
            _odRepo = odRepo;
            _detailDORepo = detailDORepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
        }

        public async Task<ApiResponse> Handle(UpdateGoodsReturnCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập kho hàng trả")
            };

            //Data nhập kho hàng trả
            var nkhts = _nkhtRepo.GetQuery();

            //Data po detail
            var odDetails = _detailDORepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();

            //Data material
            var material = _prdRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Data DimDate
            var dimDate = _dimDateRepo.GetQuery().AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateGoodsReturns.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NKHTIDs = x.Value.Select(v => v.NKHTId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackaging)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nkhts.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nkhts.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.GoodsReturnId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nkhts.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nkhts.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.GoodsReturnId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateGoodsReturns)
            {
                //Check trường hợp nhập od nhưng không có od item
                if (!string.IsNullOrEmpty(item.ODCode) && string.IsNullOrEmpty(item.ODItem))
                {
                    response.IsSuccess = false;
                    response.Message = $"Vui lòng nhập Outbound Delivery Item";

                    return response;
                }

                //Check tồn tại nkmh
                var nkht = await nkhts.FirstOrDefaultAsync(x => x.GoodsReturnId == item.NKHTId);

                //Lấy ra podetail
                var detailOD = odDetails.FirstOrDefault(x => !string.IsNullOrEmpty(item.ODCode) ? x.OutboundDeliveryItem == item.ODItem && x.OutboundDelivery.DeliveryCodeInt == long.Parse(item.ODCode) : false);

                //var img = await _utilitiesService.UploadFile(item.Image, "NKMH");

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NKHTId.ToString(), $"{item.NKHTId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKHT");
                }


                //Chưa có thì tạo mới
                if (nkht == null)
                {
                    _nkhtRepo.Add(new GoodsReturnModel
                    {
                        GoodsReturnId = item.NKHTId,
                        DetailODId = !string.IsNullOrEmpty(item.ODCode) ? detailOD.DetailOutboundDeliveryId : null,
                        PlantCode = item.Plant,
                        MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                        MaterialCodeInt = long.Parse(item.Material),
                        WeightVote = item.WeightVote,
                        BagQuantity = item.BagQuantity,
                        SingleWeight = item.SingleWeight,
                        WeightHeadCode = item.WeightHeadCode,
                        Weight = item.Weight,
                        ConfirmQty = item.ConfirmQty,
                        QuantityWithPackaging = item.QuantityWithPackaging,
                        VehicleCode = item.VehicleCode,
                        QuantityWeitght = item.QuantityWeight,
                        TruckInfoId = item.TruckInfoId.HasValue ? item.TruckInfoId : null,
                        TruckNumber = item.TruckQty,
                        InputWeight = item.InputWeight,
                        OutputWeight = item.OutputWeight,
                        Description = item.Description,
                        Image = string.IsNullOrEmpty(imgPath) ? null : imgPath,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        SlocCode = item.StorageLocation,
                        DocumentDate = item.DocumentDate,
                        CreateBy = item.CreateBy,
                        CreateTime = item.CreateOn,
                        LastEditBy = item.ChangeBy,
                        Status = item.isDelete == true ? "DEL" : "NOT"
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //ODDetailID
                    nkht.DetailODId = !string.IsNullOrEmpty(item.ODCode) ? detailOD.DetailOutboundDeliveryId : null;
                    //Material Code
                    nkht.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    //Material Code Int
                    nkht.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    nkht.SlocCode = item.StorageLocation;
                    //Sloc Name
                    //nkmh.SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation).StorageLocationName;
                    //Confirm Quantity
                    nkht.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nkht.QuantityWithPackaging = item.QuantityWithPackaging;
                    //Số phương tiện
                    nkht.VehicleCode = item.VehicleCode;
                    //Số cân đầu rea
                    nkht.OutputWeight = item.OutputWeight;
                    //Id cân xe tải
                    nkht.TruckInfoId = item.TruckInfoId.HasValue ? item.TruckInfoId : null;
                    //Số xe tải
                    nkht.TruckNumber = item.TruckQty;
                    //Ghi chú
                    nkht.Description = item.Description;
                    //Hình ảnh
                    nkht.Image = string.IsNullOrEmpty(imgPath) ? nkht.Image : imgPath;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        nkht.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nkht.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
