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

namespace MES.Application.Commands.XNVLGC
{
    public class UpdateXNVLGCCommand : IRequest<ApiResponse>
    {
        public List<UpdateXNVLGC> UpdateXNVLGCs { get; set; } = new List<UpdateXNVLGC>();
    }

    public class UpdateXNVLGC
    {
        //Id
        public Guid XNVLGCId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Component
        public string Component { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Batch
        public string Batch { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Requirement unit
        public string RequirementUnit { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Thời gian bd
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
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
    }

    public class UpdateXNVLGCCommandHandler : IRequestHandler<UpdateXNVLGCCommand, ApiResponse>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ComponentExportModel> _xnvlgcRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<PlantModel> _plantRepo;

        public UpdateXNVLGCCommandHandler(IUnitOfWork unitOfWork, IRepository<ComponentExportModel> xnvlgcRepo, IRepository<ProductModel> prdRepo,
                                          IRepository<StorageLocationModel> slocRepo, IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService,
                                          IRepository<PlantModel> plantRepo)
        {
            _unitOfWork = unitOfWork;
            _xnvlgcRepo = xnvlgcRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
            _plantRepo = plantRepo;
        }

        public async Task<ApiResponse> Handle(UpdateXNVLGCCommand request, CancellationToken cancellationToken)
        {
            //var response = new ApiResponse
            //{
            //    IsSuccess = true,
            //    Message = string.Format(CommonResource.Msg_Success, "Cập nhật xuất nguyên vật liệu sản xuất")
            //};

            ////Data xuất nvl sản xuất
            //var xnvlgcs = _xnvlgcRepo.GetQuery();

            ////Data material
            //var material = _prdRepo.GetQuery().AsNoTracking();

            ////Query plant
            //var plants = _plantRepo.GetQuery().AsNoTracking();

            ////Data sloc
            //var slocs = _slocRepo.GetQuery().AsNoTracking();

            ////Data DimDate
            //var dimDate = _dimDateRepo.GetQuery().AsNoTracking();

            ////Check confirm quantity
            ////Lấy ra các phiếu cân cần update
            //var weightVotes = request.UpdateXNVLGCs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
            //                                     .Select(x => new
            //                                     {
            //                                         WeightVote = x.Key,
            //                                         XNVLGCIds = x.Value.Select(v => v.XNVLGCId).ToList(),
            //                                         ConfirmQty = x.Value.Sum(x => x.ConfirmQuantity),
            //                                         QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
            //                                     }).ToList();
            ////Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            //foreach (var item in weightVotes)
            //{
            //    //Tính tổng confirm quantity ban đầu
            //    var sumConfirmQty1 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
            //    //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
            //    var sumConfirmQty2 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote && !item.XNVLGCIds.Contains(x.ComponentExportId)).Sum(x => x.ConfirmQty);
            //    //So sánh
            //    if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
            //    {
            //        response.IsSuccess = false;
            //        response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
            //    }

            //    //Tính tổng SL kèm bao bì
            //    var sumQtyWithPackage1 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
            //    //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
            //    var sumQtyWithPackage2 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote && !item.XNVLGCIds.Contains(x.ComponentExportId)).Sum(x => x.QuantityWithPackaging);
            //    //So sánh
            //    if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
            //    {
            //        response.IsSuccess = false;
            //        response.Message = "Vui lòng xem lại số lượng kèm bao bì";
            //    }
            //}


            //foreach (var item in request.UpdateXNVLGCs)
            //{
            //    //Check tồn tại xnvlgc
            //    var xnvlgc = await xnvlgcs.FirstOrDefaultAsync(x => x.ComponentExportId == item.XNVLGCId);


            //    var imgPath = string.Empty;
            //    //Convert Base64 to Iformfile
            //    if (!string.IsNullOrEmpty(item.NewImage))
            //    {
            //        byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
            //        MemoryStream stream = new MemoryStream(bytes);

            //        IFormFile file = new FormFile(stream, 0, bytes.Length, item.XNVLGCId.ToString(), $"{item.XNVLGCId.ToString()}.jpg");
            //        //Save image to server
            //        imgPath = await _utilitiesService.UploadFile(file, "XNVLGC");
            //    }


            //    //Chưa có thì tạo mới
            //    if (xnvlgc == null)
            //    {
            //        _xnvlgcRepo.Add(new ComponentExportModel
            //        {
            //            //id
            //            ComponentExportId = item.XNVLGCId,
            //            //Plant
            //            PlantCode = item.Plant,
            //            PlantName = plants.FirstOrDefault(x => x.PlantCode == item.Plant).PlantName,
            //            //Component
            //            ComponentCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductCode,
            //            ComponentName = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductName,
            //            ComponentCodeInt = long.Parse(item.Component),
            //            //Batch
            //            Batch = item.Batch,
            //            //Weight Vote
            //            WeightVote = item.WeightVote,
            //            //Weight Head Code
            //            WeightHeadCode = item.WeightHeadCode,
            //            //Weight
            //            Weight = item.Weight,
            //            //Confirm quantity
            //            ConfirmQty = item.ConfirmQuantity,
            //            //SL bao
            //            BagQuantity = item.BagQuantity,
            //            //Đơn trọng
            //            SingleWeight = item.SingleWeight,
            //            //Sl kèm bao bì
            //            QuantityWithPackaging = item.QuantityWithPackage,
            //            //Số lần cân
            //            QuantityWeight = item.QuantityWeight,
            //            //RequirementUnit
            //            RequirementUnit = item.RequirementUnit,
            //            //Vehicle
            //            VehicleCode = item.VehicleCode,
            //            //Số cân đầu ra
            //            OutputWeight = item.OutputWeight,
            //            Description = item.Description,
            //            Image = string.IsNullOrEmpty(imgPath) ? null : imgPath,
            //            StartTime = item.StartTime,
            //            EndTime = item.EndTime,
            //            //Sloc
            //            SlocCode = item.Sloc,
            //            SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
            //            Status = item.isDelete == true ? "DEL" : "NOT"
            //        });;
            //    }
            //    //Tồn tại thì update
            //    else
            //    {
            //        //Cập nhật
            //        //Component Code
            //        xnvlgc.ComponentCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductCode;
            //        xnvlgc.ComponentName = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductName;
            //        //Component Code Int
            //        xnvlgc.ComponentCodeInt = long.Parse(item.Component);
            //        //Storage Location
            //        xnvlgc.SlocCode = item.Sloc;
            //        //Sloc Name
            //        xnvlgc.SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "";
            //        //Batch
            //        xnvlgc.Batch = item.Batch;
            //        //Sl bao
            //        xnvlgc.BagQuantity = item.BagQuantity;
            //        //Đơn trọng
            //        xnvlgc.SingleWeight = item.SingleWeight;
            //        //Confirm Quantity
            //        xnvlgc.ConfirmQty = item.ConfirmQuantity;
            //        //Sl kèm bao bì
            //        xnvlgc.QuantityWithPackaging = item.QuantityWithPackaging;
            //        //Ghi chú
            //        xthlsx.Description = item.Description;
            //        //Hình ảnh
            //        xthlsx.Image = string.IsNullOrEmpty(imgPath) ? xthlsx.Image : imgPath;
            //        //Đánh dấu xóa
            //        if (item.isDelete == true)
            //        {
            //            xthlsx.Status = "DEL";
            //        }
            //        //Hủy đánh dấu xóa
            //        else// if (item.isDelete == false)
            //        {
            //            xthlsx.Status = "NOT";
            //        }
            //    }
            //}

            //await _unitOfWork.SaveChangesAsync();

            //return response;

            return null;
        }
    }
}
