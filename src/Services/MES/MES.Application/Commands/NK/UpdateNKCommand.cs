using Core.Extensions;
using Core.Implements;
using Core.Interfaces.Databases;
using Core.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NK
{
    public class UpdateNKCommand : IRequest<ApiResponse>
    {
        public List<UpdateNKData> UpdateNKDatas { get; set; } = new List<UpdateNKData>();
    }

    public class UpdateNKData
    {
        //NKID 
        public Guid NKId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Material
        public string Material { get; set; }
        //Sloc
        public string Sloc { get; set; }
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

    public class UpdateNKCommandHandler : IRequestHandler<UpdateNKCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OtherImportModel> _nkRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        public UpdateNKCommandHandler(IUnitOfWork unitOfWork, IRepository<OtherImportModel> nkRepo, IUtilitiesService utilitiesService,
                                    IRepository<ScaleModel> scaleRepo, IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo,
                                    IRepository<WeighSessionModel> weightSsRepo, IRepository<TruckInfoModel> truckRepo)
        {
            _unitOfWork = unitOfWork;
            _nkRepo = nkRepo;
            _utilitiesService = utilitiesService;
            _scaleRepo = scaleRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _weightSsRepo = weightSsRepo;
            _truckRepo = truckRepo;
        }

        public async Task<ApiResponse> Handle(UpdateNKCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật dữ liệu nhập khác")
            };

            //Data nhập khác
            var nks = _nkRepo.GetQuery();

            //Data material
            var material = _prodRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateNKDatas.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NKDCNBIds = x.Value.Select(v => v.NKId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQuantity),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nks.Where(x => x.WeightVote == item.WeightVote && !item.NKDCNBIds.Contains(x.OtherImportId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nks.Where(x => x.WeightVote == item.WeightVote && !item.NKDCNBIds.Contains(x.OtherImportId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateNKDatas)
            {

                //Check tồn tại nk
                var nk = await nks.FirstOrDefaultAsync(x => x.OtherImportId == item.NKId);

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NKId.ToString(), $"{item.NKId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NK");
                }

                //Dữ liệu cân
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Chưa có thì tạo mới
                if (nk == null)
                {
                    _nkRepo.Add(new OtherImportModel
                    {
                        //Id
                        OtherImportId = item.NKId,
                        //Receving Plant
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
                        //Special Stock
                        SpecialStock = item.SpecialStock,
                        //Số lượng kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackage,
                        //Số phương tiện
                        VehicleCode = item.VehicleCode,
                        //Số lần cân
                        QuantityWeight = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleCode == scale.ScaleCode && x.Status == "DANGCAN")?.TotalNumberOfWeigh : null,
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
                        CreateBy = TokenExtensions.GetAccountId(),
                        CreateTime = DateTime.Now,
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Material Code
                    nk.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    //Material Code Int
                    nk.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    nk.SlocCode = item.Sloc;
                    //Sloc Name
                    nk.SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "";
                    //Special Stock
                    nk.SpecialStock = item.SpecialStock;
                    //Confirm Quantity
                    nk.ConfirmQty = item.ConfirmQuantity;
                    //Sl kèm bao bì
                    nk.QuantityWithPackaging = item.QuantityWithPackage;
                    //Số phương tiện
                    nk.VehicleCode = item.VehicleCode;
                    //Số cân đầu ra
                    nk.OutputWeight = item.OutputWeight;
                    //Ghi chú
                    nk.Description = item.Description;
                    //Hình ảnh
                    nk.Image = string.IsNullOrEmpty(imgPath) ? nk.Image : imgPath;
                    nk.LastEditBy = TokenExtensions.GetAccountId();
                    nk.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                    {
                        nk.Status = "DEL";
                    }
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nk.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
