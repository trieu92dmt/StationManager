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

namespace MES.Application.Commands.NNVL
{
    public class UpdateNNVLCommand : IRequest<ApiResponse>
    {
        public List<UpdateNNVL> UpdateNNVLs { get; set; } = new List<UpdateNNVL>();
    }

    public class UpdateNNVL
    {
        //Id
        public Guid NNVLGCId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Material
        public string Material { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm quantity
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
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Thời gian bd
        public DateTime? StartTime { get; set; }
        //Thời gian kt
        public DateTime? EndTime{ get; set; }
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

    public class UpdateNNVLCommandHandler : IRequestHandler<UpdateNNVLCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ComponentImportModel> _nnvlgcRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<PurchaseOrderDetailModel> _detailPoRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        public UpdateNNVLCommandHandler(IUnitOfWork unitOfWork, IRepository<ComponentImportModel> nnvlgcRepo, IRepository<WeighSessionModel> weightSsRepo,
                                        IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService, IRepository<PurchaseOrderDetailModel> detailPoRepo,
                                        IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<TruckInfoModel> truckRepo)
        {
            _unitOfWork = unitOfWork;
            _nnvlgcRepo = nnvlgcRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _detailPoRepo = detailPoRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
            _truckRepo = truckRepo;
        }

        public async Task<ApiResponse> Handle(UpdateNNVLCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật xuất nguyên vật liệu gia công")
            };

            //Data nhập nvl sản xuất
            var xnvlgcs = _nnvlgcRepo.GetQuery();

            //Data material
            var material = _prodRepo.GetQuery().AsNoTracking();

            //Query plant
            var plants = _plantRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateNNVLs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NNVLGVIds = x.Value.Select(v => v.NNVLGCId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote && !item.NNVLGVIds.Contains(x.ComponentImportId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = xnvlgcs.Where(x => x.WeightVote == item.WeightVote && !item.NNVLGVIds.Contains(x.ComponentImportId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateNNVLs)
            {
                //Check tồn tại xnvlgc
                var nnvlgc = await xnvlgcs.FirstOrDefaultAsync(x => x.ComponentImportId == item.NNVLGCId);


                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NNVLGCId.ToString(), $"{item.NNVLGCId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NNVLGC");
                }


                //Chưa có thì tạo mới
                if (nnvlgc == null)
                {
                    _nnvlgcRepo.Add(new ComponentImportModel
                    {
                        //id
                        ComponentImportId = item.NNVLGCId,
                        //Plant
                        PlantCode = item.Plant,
                        PlantName = plants.FirstOrDefault(x => x.PlantCode == item.Plant).PlantName,
                        //Material
                        MaterialCode = !string.IsNullOrEmpty(item.Material) ? material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "",
                        MaterialName = !string.IsNullOrEmpty(item.Material) ? material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName : "",
                        MaterialCodeInt = !string.IsNullOrEmpty(item.Material) ? long.Parse(item.Material) : null,
                        //Batch
                        Batch = item.Batch,
                        //Weight Vote
                        WeightVote = item.WeightVote,
                        //Weight Head Code
                        WeightHeadCode = item.WeightHeadCode,
                        //Weight
                        Weight = item.Weight,
                        //Confirm quantity
                        ConfirmQty = item.ConfirmQty,
                        //Unit
                        UOM = item.Unit,
                        //Sl kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackage,
                        //Số lần cân
                        QuantityWeight = item.QuantityWeight,
                        //Vehicle
                        VehicleCode = item.VehicleCode,
                        //Số cân đầu ra
                        OutputWeight = item.OutputWeight,
                        Description = item.Description,
                        Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        //Sloc
                        SlocCode = item.Sloc,
                        SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                        Status = item.isDelete == true ? "DEL" : "NOT",
                        CreateBy = item.CreateBy,
                        CreateTime = item.CreateOn,
                        LastEditBy = TokenExtensions.GetAccountId(),
                        LastEditTime = DateTime.Now
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Material
                    nnvlgc.MaterialCode = !string.IsNullOrEmpty(item.Material) ? material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "";
                    nnvlgc.MaterialName = !string.IsNullOrEmpty(item.Material) ? material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName : "";
                    nnvlgc.MaterialCodeInt = !string.IsNullOrEmpty(item.Material) ? long.Parse(item.Material) : null;
                    //Storage Location
                    nnvlgc.SlocCode = item.Sloc;
                    //Sloc Name
                    nnvlgc.SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "";
                    //Batch
                    nnvlgc.Batch = item.Batch;
                    //Unit
                    nnvlgc.UOM = item.Unit;
                    //Confirm Quantity
                    nnvlgc.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nnvlgc.QuantityWithPackaging = item.QuantityWithPackage;
                    //Số phương tiện
                    nnvlgc.VehicleCode = item.VehicleCode;
                    //Số cân đầu ra
                    nnvlgc.OutputWeight = item.OutputWeight;
                    //Ghi chú
                    nnvlgc.Description = item.Description;
                    //Hình ảnh
                    nnvlgc.Image = string.IsNullOrEmpty(imgPath) ? nnvlgc.Image : imgPath;
                    nnvlgc.LastEditBy = TokenExtensions.GetAccountId();
                    nnvlgc.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                    {
                        nnvlgc.Status = "DEL";
                    }
                    //Hủy đánh dấu xóa
                    else// if (item.isDelete == false)
                    {
                        nnvlgc.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
            throw new NotImplementedException();
        }
    }
}
