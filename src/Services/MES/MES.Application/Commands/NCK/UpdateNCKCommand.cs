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
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NCK
{
    public class UpdateNCKCommand : IRequest<ApiResponse>
    {
        public List<UpdateNCK> UpdateNCKs { get; set; } = new List<UpdateNCK>();
    }

    public class UpdateNCK
    {
        //1. NCK ID
        public Guid NCKId { get; set; }
        //2. Plant
        public string Plant { get; set; }
        //Material Doc
        public string MaterialDoc { get; set; }
        //Material Doc Item
        public string MaterialDocItem { get; set; }
        //3. Reservation
        public string Reservation { get; set; }
        //5. Material
        public string Material { get; set; }
        //6. Sloc
        public string Sloc { get; set; }
        //7. Batch
        public string Batch { get; set; }
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
    }

    public class UpdateNCKCommandHandler : IRequestHandler<UpdateNCKCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WarehouseImportTransferModel> _nckRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<MaterialDocumentModel> _matDocRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        public UpdateNCKCommandHandler(IUnitOfWork unitOfWork, IRepository<WarehouseImportTransferModel> nckRepo, IRepository<WeighSessionModel> weightSsRepo,
                                     IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService, IRepository<MaterialDocumentModel> matDocRepo,
                                     IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo,
                                     IRepository<TruckInfoModel> truckRepo)
        {
            _unitOfWork = unitOfWork;
            _nckRepo = nckRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _matDocRepo = matDocRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
            _truckRepo = truckRepo;
        }

        public async Task<ApiResponse> Handle(UpdateNCKCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập chuyển kho")
            };

            //Data nhập chuyển kho
            var ncks = _nckRepo.GetQuery();

            //Data reservation detail
            var matdocs = _matDocRepo.GetQuery().AsNoTracking();

            //Data material
            var material = _prodRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();
            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateNCKs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NCKIds = x.Value.Select(v => v.NCKId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = ncks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = ncks.Where(x => x.WeightVote == item.WeightVote && !item.NCKIds.Contains(x.WarehouseImportTransferId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = ncks.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = ncks.Where(x => x.WeightVote == item.WeightVote && !item.NCKIds.Contains(x.WarehouseImportTransferId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateNCKs)
            {
                //Check tồn tại xck
                var nck = await ncks.FirstOrDefaultAsync(x => x.WarehouseImportTransferId == item.NCKId);

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NCKId.ToString(), $"{item.NCKId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NCK");
                }

                //Lấy ra mat doc
                var matdoc = !string.IsNullOrEmpty(item.MaterialDoc) && !string.IsNullOrEmpty(item.MaterialDocItem) ?
                               matdocs.FirstOrDefault(x => x.MaterialDocCode == item.MaterialDoc && x.MaterialDocItem == item.MaterialDocItem &&
                                                                x.PlantCode == item.Plant) : null;

                //Check material doc có khớp với material
                if (matdoc != null && matdoc.MaterialCodeInt != long.Parse(item.Material))
                {
                    response.IsSuccess = false;
                    response.Message = $"Reservation và Material Không mapping với nhau";

                    return response;
                }

                //Chưa có thì tạo mới
                if (nck == null)
                {
                    _nckRepo.Add(new WarehouseImportTransferModel
                    {
                        //Id
                        WarehouseImportTransferId = item.NCKId,
                        //Reservation - reservation item
                        MaterialDocId = matdoc != null ? matdoc.MaterialDocId : null,
                        //Plant
                        PlantCode = item.Plant,
                        //Material
                        MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                        MaterialCodeInt = long.Parse(item.Material),
                        MaterialName = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName,
                        //Reservation
                        Reservation = item.Reservation,
                        //Batch
                        Batch = item.Batch,
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
                        Status = item.isDelete == true ? "DEL" : "NOT"
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Mat doc id
                    nck.MaterialDocId = matdoc != null ? matdoc.MaterialDocId : null;
                    //Material 
                    nck.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    nck.MaterialCodeInt = long.Parse(item.Material);
                    nck.MaterialName = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductName;
                    //Reservation
                    nck.Reservation = item.Reservation;
                    //Storage Location
                    nck.SlocCode = item.Sloc;
                    //Sloc Name
                    nck.SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc)?.StorageLocationName;
                    //Batch
                    nck.Batch = item.Batch;
                    //Số lượng bao
                    nck.BagQuantity = item.BagQuantity;
                    //Đơn trọng
                    nck.SingleWeight = item.SingleWeight;
                    //Trọng lượng cân
                    nck.Weight = item.Weight;
                    //Confirm Quantity
                    nck.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nck.QuantityWithPackaging = item.QuantityWithPackage;
                    //Ghi chú
                    nck.Description = item.Description;
                    //Hình ảnh
                    nck.Image = string.IsNullOrEmpty(imgPath) ? nck.Image : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath);
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        nck.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nck.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
