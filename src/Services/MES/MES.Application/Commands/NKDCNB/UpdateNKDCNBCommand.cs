using Core.Extensions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace MES.Application.Commands.NKDCNB
{
    public class UpdateNKDCNBCommand : IRequest<ApiResponse>
    {
        public List<UpdateNKDCNB> UpdateNKDCNBs { get; set; } = new List<UpdateNKDCNB>();
    }

    public class UpdateNKDCNB
    {
        //Id
        public Guid NKDCNBId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Shipping point
        public string ShippingPoint { get; set; }
        //OD
        public string OutboundDelivery { get; set; }
        //OD item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
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

    public class UpdateNKDCNBCommandHandler : IRequestHandler<UpdateNKDCNBCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<InhouseTransferModel> _nkdcnbRepo;
        private readonly IRepository<OutboundDeliveryModel> _odRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailDORepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;

        public UpdateNKDCNBCommandHandler(IUnitOfWork unitOfWork, IRepository<InhouseTransferModel> nkdcnbRepo, IRepository<OutboundDeliveryModel> odRepo,
                                          IRepository<DetailOutboundDeliveryModel> detailDORepo, IRepository<ProductModel> prdRepo, IRepository<StorageLocationModel> slocRepo,
                                          IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService)
        {
            _unitOfWork = unitOfWork;
            _nkdcnbRepo = nkdcnbRepo;
            _odRepo = odRepo;
            _detailDORepo = detailDORepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
        }

        public async Task<ApiResponse> Handle(UpdateNKDCNBCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập kho điều chuyển nội bộ")
            };

            //Data nhập kho điều chuyển nội bộ
            var nkdcnbs = _nkdcnbRepo.GetQuery();

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
            var weightVotes = request.UpdateNKDCNBs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NKDCNBIds = x.Value.Select(v => v.NKDCNBId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nkdcnbs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nkdcnbs.Where(x => x.WeightVote == item.WeightVote && !item.NKDCNBIds.Contains(x.InhouseTransferId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nkdcnbs.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nkdcnbs.Where(x => x.WeightVote == item.WeightVote && !item.NKDCNBIds.Contains(x.InhouseTransferId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateNKDCNBs)
            {
                //Check trường hợp nhập od nhưng không có od item
                if (!string.IsNullOrEmpty(item.OutboundDelivery) && string.IsNullOrEmpty(item.OutboundDeliveryItem))
                {
                    response.IsSuccess = false;
                    response.Message = $"Vui lòng nhập Outbound Delivery Item";

                    return response;
                }

                //Check tồn tại nkdcnb
                var nkdcnb = await nkdcnbs.FirstOrDefaultAsync(x => x.InhouseTransferId == item.NKDCNBId);

                //Lấy ra podetail
                var detailOD = odDetails.FirstOrDefault(x => !string.IsNullOrEmpty(item.OutboundDelivery) ? x.OutboundDeliveryItem == item.OutboundDeliveryItem && x.OutboundDelivery.DeliveryCodeInt == long.Parse(item.OutboundDelivery) : false);

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NKDCNBId.ToString(), $"{item.NKDCNBId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKDCNB");
                }

                //Check od có mapping với material
                if (detailOD != null && detailOD.ProductCodeInt != long.Parse(item.Material))
                {
                    response.IsSuccess = false;
                    response.Message = $"Outbound delivery và Material Không mapping với nhau";

                    return response;
                }


                //Chưa có thì tạo mới
                if (nkdcnb == null)
                {
                    _nkdcnbRepo.Add(new InhouseTransferModel
                    {
                        //Id
                        InhouseTransferId = item.NKDCNBId,
                        //Od - Od item
                        DetailODId = detailOD != null ? detailOD.DetailOutboundDeliveryId : null,
                        //Receving Plant
                        PlantCode = item.Plant,
                        //Material
                        MaterialCode = !string.IsNullOrEmpty(item.ShippingPoint) ? material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode: "",
                        MaterialCodeInt = !string.IsNullOrEmpty(item.ShippingPoint) ? long.Parse(item.Material) : null,
                        //Đầu cân
                        WeightHeadCode = item.WeightHeadCode,
                        //Số phiếu cân
                        WeightVote = item.WeightVote,
                        //Trọng lượng cân
                        Weight = item.Weight,
                        //Confỉm quantity
                        ConfirmQty = item.ConfirmQty,
                        //Số lượng kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackage,
                        //Số phương tiện
                        VehicleCode = item.VehicleCode,
                        //Đơn vị vận chuyển
                        TransportUnit = detailOD != null ? detailOD.OutboundDelivery.TransportUnit : "",
                        //Số lần cân
                        QuantityWeitght = item.QuantityWeight,
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
                    //Outbound delivery
                    nkdcnb.DetailODId = detailOD != null ? detailOD.DetailOutboundDeliveryId : null;
                    //Material Code
                    nkdcnb.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    //Material Code Int
                    nkdcnb.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    nkdcnb.SlocCode = item.Sloc;
                    //Sloc Name
                    nkdcnb.SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "";
                    //Confirm Quantity
                    nkdcnb.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nkdcnb.QuantityWithPackaging = item.QuantityWithPackage;
                    //Số phương tiện
                    nkdcnb.VehicleCode = item.VehicleCode;
                    //Đơn vị vận chuyển
                    nkdcnb.TransportUnit = detailOD != null ? detailOD.OutboundDelivery.TransportUnit : "";
                    //Số cân đầu ra
                    nkdcnb.OutputWeight = item.OutputWeight;
                    //Ghi chú
                    nkdcnb.Description = item.Description;
                    //Hình ảnh
                    nkdcnb.Image = string.IsNullOrEmpty(imgPath) ? nkdcnb.Image : imgPath;
                    nkdcnb.LastEditBy = TokenExtensions.GetAccountId();
                    nkdcnb.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                    {
                        nkdcnb.Status = "DEL";
                    }    
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nkdcnb.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
