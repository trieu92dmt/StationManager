using ISD.Core.Interfaces.Databases;
using ISD.Core.Models;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Core.Utilities;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                                                     WeightVote = x.Key,
                                                     NKHTIDs = x.Value.Select(v => v.NKTPSXId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackaging)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nktpsxs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nktpsxs.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.RcFromProductiontId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nktpsxs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nktpsxs.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.RcFromProductiontId)).Sum(x => x.QuantityWithPackaging);
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
                        WorkOrderId = wo != null ? wo.WorkOrderId : null,
                        PlantCode = item.Plant,
                        MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                        MaterialCodeInt = long.Parse(item.Material),
                        WeightVote = item.WeightVote,
                        WeightHeadCode = item.WeightHeadCode,
                        Weight = item.Weight,
                        ConfirmQty = item.ConfirmQty,
                        QuantityWithPackaging = item.QuantityWithPackaging,
                        QuantityWeitght = item.QuantityWeight,
                        Batch = item.Batch,
                        BagQuantity= item.BagQuantity,
                        SingleWeight = item.SingleWeight,
                        Description = item.Description,
                        Image = string.IsNullOrEmpty(imgPath) ? null : imgPath,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        SlocCode = item.StorageLocation,
                        SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation)?.StorageLocationName,
                        Status = item.isDelete == true ? "DEL" : "NOT"
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
                    nktpsx.SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation)?.StorageLocationName;
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
