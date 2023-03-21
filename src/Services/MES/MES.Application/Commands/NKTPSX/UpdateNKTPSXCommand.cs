using Core.Extensions;
using Core.Interfaces.Databases;
using DTOs.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Commands.NKTPSX
{
    public class UpdateNKTPSXCommand : IRequest<ApiResponse>
    {
        public List<UpdateNKTPSX> UpdateUpdateNKTPSXs { get; set; } = new List<UpdateNKTPSX>();
    }

    public class UpdateNKTPSX
    {
        //Id nhập kho hàng trả
        public Guid NKTPSXId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Lệnh sản xuất
        public string WorkOrder { get; set; }
        //Material
        public string Material { get; set; }
        //Storage Location
        public string StorageLocation { get; set; }
        //Batch
        public string Batch { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackaging { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //UOM
        public string Unit { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
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

    public class UpdateNKTPSXCommandHandler : IRequestHandler<UpdateNKTPSXCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ReceiptFromProductionModel> _nktpsxRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<WorkOrderModel> _woRepo;

        public UpdateNKTPSXCommandHandler(IUnitOfWork unitOfWork, IRepository<ReceiptFromProductionModel> nktpsxRepo, IRepository<ProductModel> prdRepo, 
                                          IRepository<StorageLocationModel> slocRepo, IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService,
                                          IRepository<WorkOrderModel> woRepo)
        {
            _unitOfWork = unitOfWork;
            _nktpsxRepo = nktpsxRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
            _woRepo = woRepo;
        }

        public async Task<ApiResponse> Handle(UpdateNKTPSXCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập kho TP sản xuất")
            };

            //Data nhập kho TP sản xuất
            var nktpsxs = _nktpsxRepo.GetQuery();

            //Data work order
            var wos = _woRepo.GetQuery().AsNoTracking();

            //Data material
            var material = _prdRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Data DimDate
            var dimDate = _dimDateRepo.GetQuery().AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateUpdateNKTPSXs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     //Số phiếu cân
                                                     WeightVote = x.Key,
                                                     //Danh sách id các dòng
                                                     NKTPSXIDs = x.Value.Select(v => v.NKTPSXId).ToList(),
                                                     //Confirm quantity
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     //SL kèm bao bì
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackaging)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nktpsxs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nktpsxs.Where(x => x.WeightVote == item.WeightVote && !item.NKTPSXIDs.Contains(x.RcFromProductiontId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nktpsxs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nktpsxs.Where(x => x.WeightVote == item.WeightVote && !item.NKTPSXIDs.Contains(x.RcFromProductiontId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateUpdateNKTPSXs)
            {
                //Check tồn tại nktpsx
                var nktpsx = await nktpsxs.FirstOrDefaultAsync(x => x.RcFromProductiontId == item.NKTPSXId);

                //Lấy ra workorder
                var wo = !string.IsNullOrEmpty(item.WorkOrder) ? wos.FirstOrDefault(x => x.WorkOrderCodeInt == long.Parse(item.WorkOrder)) : null;

                //Check wo có khớp với material
                if (wo!= null && wo.ProductCodeInt != long.Parse(item.Material))
                {
                    response.IsSuccess = false;
                    response.Message = $"WorkOrder và Material Không mapping với nhau";

                    return response;
                }

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NKTPSXId.ToString(), $"{item.NKTPSXId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKTPSX");
                }


                //Chưa có thì tạo mới
                if (nktpsx == null)
                {
                    _nktpsxRepo.Add(new ReceiptFromProductionModel
                    {
                        RcFromProductiontId = item.NKTPSXId,
                        //Workorder id
                        WorkOrderId = wo != null ? wo.WorkOrderId : null,
                        //Mã nhà máy
                        PlantCode = item.Plant,
                        //Material Code
                        MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                        //Material Code Int
                        MaterialCodeInt = long.Parse(item.Material),
                        //Số phiếu cân
                        WeightVote = item.WeightVote,
                        //Mã đầu cân
                        WeightHeadCode = item.WeightHeadCode,
                        //Trọng lượng cân
                        Weight = item.Weight,
                        //Confirm quantity
                        ConfirmQty = item.ConfirmQty,
                        //SL kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackaging,
                        //Số lần cân
                        QuantityWeitght = item.QuantityWeight,
                        //Số lô
                        Batch = item.Batch,
                        //Số lượng bao
                        BagQuantity= item.BagQuantity,
                        //Đơn trọng
                        SingleWeight = item.SingleWeight,
                        //Ghi chú
                        Description = item.Description,
                        //Hình ảnh
                        Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                        //TG bắt đầu
                        StartTime = item.StartTime,
                        //TG kết thúc
                        EndTime = item.EndTime,
                        //Storage location
                        SlocCode = item.StorageLocation,
                        SlocName = !string.IsNullOrEmpty(item.StorageLocation) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation).StorageLocationName : "",
                        //Trạng thái
                        Status = item.isDelete == true ? "DEL" : "NOT",
                        //Người tạo
                        CreateBy = TokenExtensions.GetAccountId(),
                        CreateTime = DateTime.Now,
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Workorder
                    nktpsx.WorkOrderId = wo != null ? wo.WorkOrderId : null;
                    //Material Code
                    nktpsx.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    //Material Code Int
                    nktpsx.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    nktpsx.SlocCode = item.StorageLocation;
                    //Sloc Name
                    nktpsx.SlocName = !string.IsNullOrEmpty(item.StorageLocation) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation).StorageLocationName : "";
                    //Số lượng bao
                    nktpsx.BagQuantity = item.BagQuantity;
                    //Đơn trọng
                    nktpsx.SingleWeight = item.SingleWeight;
                    //Batch
                    nktpsx.Batch = item.Batch;
                    //Confirm Quantity
                    nktpsx.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nktpsx.QuantityWithPackaging = item.QuantityWithPackaging;
                    //Ghi chú
                    nktpsx.Description = item.Description;
                    //Hình ảnh
                    nktpsx.Image = string.IsNullOrEmpty(imgPath) ? nktpsx.Image : imgPath;
                    nktpsx.LastEditBy = TokenExtensions.GetAccountId();
                    nktpsx.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        nktpsx.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nktpsx.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
