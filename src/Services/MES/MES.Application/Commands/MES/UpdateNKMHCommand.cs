using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Models;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Core.Utilities;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.MES
{
    public class UpdateNKMHCommand : IRequest<ApiResponse>
    {
        public List<UpdateNKMH> UpdateNKMHs { get; set; } = new List<UpdateNKMH>();
    }

    public class UpdateNKMH
    {
        //Id nhập kho mua hàng
        public Guid NKMHId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Purchase Order
        public string PurchaseOrderCode { get; set; }
        //PO Item
        public string POItem { get; set; }
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
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
    }
    public class UpdateNKMHCommandHandler : IRequestHandler<UpdateNKMHCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoodsReceiptModel> _nkmhRepo;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<ProductModel> _materialRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;

        public UpdateNKMHCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReceiptModel> nkmhRepo, IRepository<PurchaseOrderDetailModel> poDetailRepo,
                                        IRepository<StorageLocationModel> slocRepo, IRepository<ProductModel> materialRepo, IRepository<DimDateModel> dimDateRepo,
                                        IUtilitiesService utilitiesService)
        {
            _unitOfWork = unitOfWork;
            _nkmhRepo = nkmhRepo;
            _poDetailRepo = poDetailRepo;
            _slocRepo = slocRepo;
            _materialRepo = materialRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
        }

        public async Task<ApiResponse> Handle(UpdateNKMHCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập kho mua hàng")
            };

            //Data nhập kho mua hàng
            var nkmhs = _nkmhRepo.GetQuery();

            //Data po detail
            var poDetails = _poDetailRepo.GetQuery().Include(x => x.PurchaseOrder).AsNoTracking();

            //Data material
            var material = _materialRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Data DimDate
            var dimDate = _dimDateRepo.GetQuery().AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateNKMHs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NKMHIds = x.Value.Select(v => v.NKMHId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackaging)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nkmhs.Where(x => x.WeitghtVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nkmhs.Where(x => x.WeitghtVote == item.WeightVote && !item.NKMHIds.Contains(x.GoodsReceiptId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nkmhs.Where(x => x.WeitghtVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nkmhs.Where(x => x.WeitghtVote == item.WeightVote && !item.NKMHIds.Contains(x.GoodsReceiptId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }    
            }


            foreach (var item in request.UpdateNKMHs)
            {
                //Check tồn tại nkmh
                var nkmh = await nkmhs.FirstOrDefaultAsync(x => x.GoodsReceiptId == item.NKMHId);

                //Lấy ra podetail
                var detailPO = poDetails.FirstOrDefault(x => !string.IsNullOrEmpty(item.PurchaseOrderCode) ? x.POLine == item.POItem && x.PurchaseOrder.PurchaseOrderCodeInt == long.Parse(item.PurchaseOrderCode) : false);

                //var img = await _utilitiesService.UploadFile(item.Image, "NKMH");

                var imgPath = "";
                //Convert Base64 to Iformfile
                if (string.IsNullOrEmpty(item.Image))
                {
                    byte[] bytes = Convert.FromBase64String(item.Image);
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NKMHId.ToString(), item.NKMHId.ToString());
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKMH");
                }
                

                //Chưa có thì tạo mới
                if (nkmh == null)
                {
                    _nkmhRepo.Add(new GoodsReceiptModel
                    {
                        GoodsReceiptId = item.NKMHId,
                        PurchaseOrderDetailId = !string.IsNullOrEmpty(item.PurchaseOrderCode) ? detailPO.PurchaseOrderDetailId : null,
                        PlantCode = item.Plant,
                        MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                        MaterialCodeInt = long.Parse(item.Material),
                        WeitghtVote = item.WeightVote,
                        BagQuantity = item.BagQuantity,
                        SingleWeight = item.SingleWeight,
                        WeightHeadCode = item.WeightHeadCode,
                        Weight = item.Weight,
                        ConfirmQty = item.ConfirmQty,
                        QuantityWithPackaging = item.QuantityWithPackaging,
                        VehicleCode = item.VehicleCode,
                        QuantityWeitght = item.QuantityWeight,
                        TruckQuantity = item.TruckQty,
                        InputWeight = item.InputWeight,
                        OutputWeight = item.OutputWeight,
                        Description = item.Description,
                        Img = string.IsNullOrEmpty(imgPath) ? null : imgPath,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        SlocCode = item.StorageLocation,
                        SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation).StorageLocationName,
                        DocumentDate = item.DocumentDate,
                        DateKey = dimDate.FirstOrDefault(x => x.Date.Value.Date == item.DocumentDate.Value.Date && x.Date.Value.Month == item.DocumentDate.Value.Month && x.Date.Value.Year == item.DocumentDate.Value.Year).DateKey,
                        Batch = item.Batch,
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
                    //PODetailId
                    nkmh.PurchaseOrderDetailId = !string.IsNullOrEmpty(item.PurchaseOrderCode) ? detailPO.PurchaseOrderDetailId : null;
                    //Material Code
                    nkmh.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    //Material Code Int
                    nkmh.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    nkmh.SlocCode = item.StorageLocation;
                    //Sloc Name
                    nkmh.SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation).StorageLocationName;
                    //Confirm Quantity
                    nkmh.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nkmh.QuantityWithPackaging = item.QuantityWithPackaging;
                    //Số phương tiện
                    nkmh.VehicleCode = item.VehicleCode;
                    //Số cân đầu rea
                    nkmh.OutputWeight = item.OutputWeight;
                    //Số xe tải
                    nkmh.TruckQuantity = item.TruckQty;
                    //Ghi chú
                    nkmh.Description = item.Description;
                    //Hình ảnh
                    nkmh.Img = string.IsNullOrEmpty(imgPath) ? null : imgPath;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        nkmh.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nkmh.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
