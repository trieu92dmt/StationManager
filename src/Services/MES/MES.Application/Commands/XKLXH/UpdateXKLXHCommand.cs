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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XKLXH
{
    public class UpdateXKLXHCommand : IRequest<ApiResponse>
    {
        public List<UpdateXKLXH> UpdateXKLXHs { get; set; } = new List<UpdateXKLXH>();
    }

    public class UpdateXKLXH
    {
        //Id
        public Guid XKLXHId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Outbound delivery
        public string OutboundDelivery { get; set; }
        //Outbound delivery item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Batch
        public string Batch { get; set; }
        //Confirm Qty
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Unit
        public string Unit { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Trọng lượng hàng hóa
        public decimal? GoodsWeight { get; set; }
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

    public class UpdateXKLXHCommandHandler : IRequestHandler<UpdateXKLXHCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<OutboundDeliveryModel> _odRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailDORepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;
        public UpdateXKLXHCommandHandler(IUnitOfWork unitOfWork, IRepository<ExportByCommandModel> xklxhRepo, IRepository<OutboundDeliveryModel> odRepo,
                                         IRepository<DetailOutboundDeliveryModel> detailDORepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                                         IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService)
        {
            _unitOfWork = unitOfWork;
            _xklxhRepo = xklxhRepo;
            _odRepo = odRepo;
            _detailDORepo = detailDORepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
        }

        public async Task<ApiResponse> Handle(UpdateXKLXHCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật xuất kho theo lệnh xuất hàng")
            };

            //Data xuất kho theo lệnh xuất hàng
            var nkhts = _xklxhRepo.GetQuery();

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
            var weightVotes = request.UpdateXKLXHs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     XKLXHs = x.Value.Select(v => v.XKLXHId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nkhts.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nkhts.Where(x => x.WeightVote == item.WeightVote && !item.XKLXHs.Contains(x.ExportByCommandId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nkhts.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nkhts.Where(x => x.WeightVote == item.WeightVote && !item.XKLXHs.Contains(x.ExportByCommandId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateXKLXHs)
            {
                //Check trường hợp nhập od nhưng không có od item
                if (!string.IsNullOrEmpty(item.OutboundDelivery) && string.IsNullOrEmpty(item.OutboundDeliveryItem))
                {
                    response.IsSuccess = false;
                    response.Message = $"Vui lòng nhập Outbound Delivery Item";

                    return response;
                }

                //Check tồn tại xuất kho theo lxh
                var xklxh = await nkhts.FirstOrDefaultAsync(x => x.ExportByCommandId == item.XKLXHId);

                //Lấy ra podetail
                var detailOD = odDetails.FirstOrDefault(x => !string.IsNullOrEmpty(item.OutboundDelivery) ? x.OutboundDeliveryItem == item.OutboundDeliveryItem && x.OutboundDelivery.DeliveryCodeInt == long.Parse(item.OutboundDelivery) : false);

                //Check od, oditem có khớp với material
                if (detailOD != null && detailOD.ProductCodeInt != long.Parse(item.Material))
                {
                    response.IsSuccess = false;
                    response.Message = $"Od OdItem và Material Không mapping với nhau";

                    return response;
                }

                //var img = await _utilitiesService.UploadFile(item.Image, "NKMH");SS

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.XKLXHId.ToString(), $"{item.XKLXHId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XKLXH");
                }


                //Chưa có thì tạo mới
                if (xklxh == null)
                {
                    _xklxhRepo.Add(new ExportByCommandModel
                    {
                        //Id
                        ExportByCommandId = item.XKLXHId,
                        //Plant
                        PlantCode = item.Plant,
                        //Outbound delivery
                        DetailODId = !string.IsNullOrEmpty(item.OutboundDelivery) ? detailOD.DetailOutboundDeliveryId : null,
                        //Material
                        MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                        MaterialCodeInt = long.Parse(item.Material),
                        //Sloc
                        SlocCode = item.Sloc,
                        SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(s => s.StorageLocationCode == item.Sloc).StorageLocationName : "",
                        //Đầu cân
                        WeightHeadCode = item.WeightHeadCode,
                        //Trọng lượng cân
                        Weight = item.Weight,
                        //Batch
                        Batch = item.Batch,
                        //Confirm qty
                        ConfirmQty = item.ConfirmQty,
                        //SL kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackage,
                        //Số phương tiện
                        VehicleCode = item.VehicleCode,
                        //Image
                        Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                        //Số lần cân
                        QuantityWeight = item.QuantityWeight,
                        //Unit
                        UOM = item.Unit,
                        //Số phiếu cân
                        WeightVote = item.WeightVote,
                        //Thời gian bắt đầu
                        StartTime = item.StartTime,
                        //Thời gian kết thúc
                        EndTime = item.EndTime,
                        //Số cần đâu ra
                        OutputWeight = item.OutputWeight,
                        //Trọng lượng hàng hóa
                        GoodsWeight = item.GoodsWeight,
                        //Ghi chú
                        Description = item.Description,
                        //Trạng thái
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
                    //ODDetailID
                    xklxh.DetailODId = !string.IsNullOrEmpty(item.OutboundDelivery) ? detailOD.DetailOutboundDeliveryId : null;
                    //Material Code
                    xklxh.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    //Material Code Int
                    xklxh.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    xklxh.SlocCode = item.Sloc;
                    //Sloc Name
                    xklxh.SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "";
                    //Confirm Quantity
                    xklxh.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    xklxh.QuantityWithPackaging = item.QuantityWithPackage;
                    //Số phương tiện
                    xklxh.VehicleCode = item.VehicleCode;
                    //Số cân đầu rea
                    xklxh.OutputWeight = item.OutputWeight;
                    //Trọng lượng hàng hóa
                    xklxh.GoodsWeight = item.GoodsWeight;
                    //Ghi chú
                    xklxh.Description = item.Description;
                    //Hình ảnh
                    xklxh.Image = string.IsNullOrEmpty(imgPath) ? xklxh.Image : imgPath;
                    //Change by
                    xklxh.LastEditBy = TokenExtensions.GetAccountId();
                    xklxh.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        xklxh.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        xklxh.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
