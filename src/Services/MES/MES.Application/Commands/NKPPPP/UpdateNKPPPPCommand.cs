using Core.Interfaces.Databases;
using Core.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Commands.NKPPPP
{
    public class UpdateNKPPPPCommand : IRequest<ApiResponse>
    {
        public List<UpdateNKPPPP> UpdateUpdateNKPPPPs { get; set; } = new List<UpdateNKPPPP>();
    }

    public class UpdateNKPPPP
    {
        //Id nhập kho phụ phẩm phế phẩm
        public Guid NKPPPPId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Lệnh sản xuất
        public string WorkOrder { get; set; }
        //Item component
        public string ItemComponent { get; set; }
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

    public class UpdateNKPPPPCommandHandler : IRequestHandler<UpdateNKPPPPCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ScrapFromProductionModel> _nkppppRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<DetailWorkOrderModel> _woRepo;

        public UpdateNKPPPPCommandHandler(IUnitOfWork unitOfWork, IRepository<ScrapFromProductionModel> nkppppRepo, IRepository<ProductModel> prdRepo,
                                          IRepository<StorageLocationModel> slocRepo, IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService,
                                          IRepository<DetailWorkOrderModel> woRepo)
        {
            _unitOfWork = unitOfWork;
            _nkppppRepo = nkppppRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
            _woRepo = woRepo;
        }

        public async Task<ApiResponse> Handle(UpdateNKPPPPCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập kho phụ phẩm phế phẩm")
            };

            //Data nhập kho TP sản xuất
            var nkpppps = _nkppppRepo.GetQuery();

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
            var weightVotes = request.UpdateUpdateNKPPPPs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NKHTIDs = x.Value.Select(v => v.NKPPPPId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackaging)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nkpppps.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nkpppps.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.ScFromProductiontId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nkpppps.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nkpppps.Where(x => x.WeightVote == item.WeightVote && !item.NKHTIDs.Contains(x.ScFromProductiontId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateUpdateNKPPPPs)
            {
                //Check tồn tại nktpsx
                var nkpppp = await nkpppps.FirstOrDefaultAsync(x => x.ScFromProductiontId == item.NKPPPPId);

                //Lấy ra workorder detail
                var wo = !string.IsNullOrEmpty(item.WorkOrder) && !string.IsNullOrEmpty(item.Component) ?
                                    wos.FirstOrDefault(d => d.WorkOrder.WorkOrderCodeInt == long.Parse(item.WorkOrder) &&
                                                             d.WorkOrderItem == item.ItemComponent &&
                                                             d.ProductCodeInt == long.Parse(item.Component)) : null;

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NKPPPPId.ToString(), $"{item.NKPPPPId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKPPPP");
                }


                //Chưa có thì tạo mới
                if (nkpppp == null)
                {
                    _nkppppRepo.Add(new ScrapFromProductionModel
                    {
                        ScFromProductiontId = item.NKPPPPId,
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
                        SlocName = !string.IsNullOrEmpty(item.StorageLocation) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation).StorageLocationName : "",
                        Status = item.isDelete == true ? "DEL" : "NOT"
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Detail wo id
                    nkpppp.DetailWorkOrderId = wo != null ? wo.DetailWorkOrderId : null;
                    //Component Code
                    nkpppp.ComponentCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component)).ProductCode;
                    //Component Code Int
                    nkpppp.ComponentCodeInt = long.Parse(item.Component);
                    //Storage Location
                    nkpppp.SlocCode = item.StorageLocation;
                    //Sloc Name
                    nkpppp.SlocName = !string.IsNullOrEmpty(item.StorageLocation) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.StorageLocation).StorageLocationName : "";
                    //Batch
                    nkpppp.Batch = item.Batch;
                    //Sl bao
                    nkpppp.BagQuantity = item.BagQuantity;
                    //Đơn trọng
                    nkpppp.SingleWeight = item.SingleWeight;
                    //Confirm Quantity
                    nkpppp.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nkpppp.QuantityWithPackaging = item.QuantityWithPackaging;
                    //Ghi chú
                    nkpppp.Description = item.Description;
                    //Hình ảnh
                    nkpppp.Image = string.IsNullOrEmpty(imgPath) ? nkpppp.Image : imgPath;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        nkpppp.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nkpppp.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
