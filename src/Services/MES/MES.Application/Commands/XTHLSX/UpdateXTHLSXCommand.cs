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
using System.Threading.Tasks.Dataflow;

namespace MES.Application.Commands.XTHLSX
{
    public class UpdateXTHLSXCommand : IRequest<ApiResponse>
    {
        public List<UpdateXTHLSX> UpdateXTHLSXs { get; set; } = new List<UpdateXTHLSX>();
    }

    public class UpdateXTHLSX
    {
        //Id nhập kho phụ phẩm phế phẩm
        public Guid XTHLSXId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Lệnh sản xuất
        public string WorkOrder { get; set; }
        //Component
        public string Component { get; set; }
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

    public class UpdateXTHLSXCommandHandler : IRequestHandler<UpdateXTHLSXCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IssueForProductionModel> _xthlsxRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<DetailWorkOrderModel> _woRepo;

        public UpdateXTHLSXCommandHandler(IUnitOfWork unitOfWork, IRepository<IssueForProductionModel> xthlsxRepo, IRepository<ProductModel> prdRepo,
                                          IRepository<StorageLocationModel> slocRepo, IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService,
                                          IRepository<DetailWorkOrderModel> woRepo)
        {
            _unitOfWork = unitOfWork;
            _xthlsxRepo = xthlsxRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
            _woRepo = woRepo;
        }

        public async Task<ApiResponse> Handle(UpdateXTHLSXCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập kho phụ phẩm phế phẩm")
            };

            //Data nhập kho TP sản xuất
            var xthlsxs = _xthlsxRepo.GetQuery();

            //Data work order
            var wos = _woRepo.GetQuery().Include(x => x.WorkOrder).AsNoTracking();

            //Data material
            var material = _prdRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Data DimDate
            var dimDate = _dimDateRepo.GetQuery().AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateXTHLSXs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     XTHLSXs = x.Value.Select(v => v.XTHLSXId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackaging)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = xthlsxs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = xthlsxs.Where(x => x.WeightVote == item.WeightVote && !item.XTHLSXs.Contains(x.IssForProductiontId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = xthlsxs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = xthlsxs.Where(x => x.WeightVote == item.WeightVote && !item.XTHLSXs.Contains(x.IssForProductiontId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateXTHLSXs)
            {
                //Check tồn tại nktpsx
                var xthlsx = await xthlsxs.FirstOrDefaultAsync(x => x.IssForProductiontId == item.XTHLSXId);

                //Lấy ra workorder detail
                var wo = !string.IsNullOrEmpty(item.WorkOrder) && !string.IsNullOrEmpty(item.Component) ? 
                         wos.FirstOrDefault(x => x.WorkOrder.WorkOrderCodeInt == long.Parse(item.WorkOrder) && x.ProductCodeInt == long.Parse(item.Component)) : null;

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.XTHLSXId.ToString(), $"{item.XTHLSXId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XTHLSX");
                }


                //Chưa có thì tạo mới
                if (xthlsx == null)
                {
                    _xthlsxRepo.Add(new IssueForProductionModel
                    {
                        IssForProductiontId = item.XTHLSXId,
                        DetailWorkOrderId = wo != null ? wo.DetailWorkOrderId : null,
                        PlantCode = item.Plant,
                        ComponentCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductCode,
                        ComponentCodeInt = long.Parse(item.Component),
                        Batch = item.Batch,
                        BagQuantity = item.BagQuantity,
                        SingleWeight = item.SingleWeight,
                        WeightVote = item.WeightVote,
                        WeightHeadCode = item.WeightHeadCode,
                        Weight = item.Weight,
                        ConfirmQty = item.ConfirmQty,
                        QuantityWithPackaging = item.QuantityWithPackaging,
                        QuantityWeitght = item.QuantityWeight,
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
                    //Detail wo id
                    xthlsx.DetailWorkOrderId = wo != null ? wo.DetailWorkOrderId : null;
                    //Component Code
                    xthlsx.ComponentCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductCode;
                    //Component Code Int
                    xthlsx.ComponentCodeInt = long.Parse(item.Component);
                    //Storage Location
                    xthlsx.SlocCode = item.StorageLocation;
                    //Sloc Name
                    xthlsx.SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation)?.StorageLocationName;
                    //Batch
                    xthlsx.Batch = item.Batch;
                    //Sl bao
                    xthlsx.BagQuantity = item.BagQuantity;
                    //Đơn trọng
                    xthlsx.SingleWeight = item.SingleWeight;
                    //Confirm Quantity
                    xthlsx.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    xthlsx.QuantityWithPackaging = item.QuantityWithPackaging;
                    //Ghi chú
                    xthlsx.Description = item.Description;
                    //Hình ảnh
                    xthlsx.Image = string.IsNullOrEmpty(imgPath) ? xthlsx.Image : imgPath;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                    {
                        xthlsx.Status = "DEL";
                    }    
                    //Hủy đánh dấu xóa
                    else// if (item.isDelete == false)
                    {
                        xthlsx.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
