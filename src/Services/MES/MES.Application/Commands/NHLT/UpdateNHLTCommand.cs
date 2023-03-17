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

namespace MES.Application.Commands.NHLT
{
    public class UpdateNHLTCommand : IRequest<ApiResponse>
    {
        public List<UpdateNHLT> UpdateNHLTs { get; set; } = new List<UpdateNHLT>();
    }

    public class UpdateNHLT
    {
        //Id
        public Guid NHLTId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Od
        public string OutboundDelivery { get; set; }
        //Od Item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Số batch
        public string Batch { get; set; }
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
        //Thời gian bd
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc 
        public DateTime? EndTime { get; set; }
        //Customer
        public string Customer { get; set; }
        //confỉm quantity
        public decimal? ConfirmQty { get; set; }
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

    public class UpdateNHLTCommandHandler : IRequestHandler<UpdateNHLTCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoodsReceiptTypeTModel> _nhltRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<DimDateModel> _dimDateRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<DetailOutboundDeliveryModel> _odDetailRepo;
        public UpdateNHLTCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReceiptTypeTModel> nhltRepo, IRepository<ProductModel> prdRepo,
                                        IRepository<StorageLocationModel> slocRepo, IRepository<DimDateModel> dimDateRepo, IUtilitiesService utilitiesService,
                                        IRepository<DetailOutboundDeliveryModel> odDetailRepo)
        {
            _unitOfWork = unitOfWork;
            _nhltRepo = nhltRepo;
            _prdRepo = prdRepo;
            _slocRepo = slocRepo;
            _dimDateRepo = dimDateRepo;
            _utilitiesService = utilitiesService;
            _odDetailRepo = odDetailRepo;
        }

        public async Task<ApiResponse> Handle(UpdateNHLTCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = string.Format(CommonResource.Msg_Success, "Cập nhật nhập hàng loại T")
            };

            //Data nhập hàng loại T
            var nhlts = _nhltRepo.GetQuery();

            //Data detail od
            var odDetails = _odDetailRepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();

            //Data material
            var material = _prdRepo.GetQuery().AsNoTracking();

            //Data sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Data DimDate
            var dimDate = _dimDateRepo.GetQuery().AsNoTracking();

            //Check confirm quantity
            //Lấy ra các phiếu cân cần update
            var weightVotes = request.UpdateNHLTs.GroupBy(x => x.WeightVote, (k, v) => new { Key = k, Value = v.ToList() })
                                                 .Select(x => new
                                                 {
                                                     WeightVote = x.Key,
                                                     NHLTIDs = x.Value.Select(v => v.NHLTId).ToList(),
                                                     ConfirmQty = x.Value.Sum(x => x.ConfirmQty),
                                                     QuantityWithPackage = x.Value.Sum(x => x.QuantityWithPackage)
                                                 }).ToList();
            //Duyệt phiếu cân kiểm tra confirm quantity và SL kèm bao bì
            foreach (var item in weightVotes)
            {
                //Tính tổng confirm quantity ban đầu
                var sumConfirmQty1 = nhlts.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.ConfirmQty);
                //Tính tổng confirm quantity khác các dòng data gửi lên từ FE
                var sumConfirmQty2 = nhlts.Where(x => x.WeightVote == item.WeightVote && !item.NHLTIDs.Contains(x.GoodsReceiptTypeTId)).Sum(x => x.ConfirmQty);
                //So sánh
                if (item.ConfirmQty + sumConfirmQty2 > sumConfirmQty1)
                {
                    response.IsSuccess = false;
                    response.Message = $"Confirm Quantity ban đầu là {sumConfirmQty1}";
                }

                //Tính tổng SL kèm bao bì
                var sumQtyWithPackage1 = nhlts.Where(x => x.WeightVote == item.WeightVote).Sum(x => x.QuantityWithPackaging);
                //Tính tổng SL kèm bao bì khác các dòng data gửi lên từ FE
                var sumQtyWithPackage2 = nhlts.Where(x => x.WeightVote == item.WeightVote && !item.NHLTIDs.Contains(x.GoodsReceiptTypeTId)).Sum(x => x.QuantityWithPackaging);
                //So sánh
                if (item.QuantityWithPackage + sumQtyWithPackage2 > sumQtyWithPackage1)
                {
                    response.IsSuccess = false;
                    response.Message = "Vui lòng xem lại số lượng kèm bao bì";
                }
            }


            foreach (var item in request.UpdateNHLTs)
            {
                //Check tồn tại nhlt
                var nhlt = await nhlts.FirstOrDefaultAsync(x => x.GoodsReceiptTypeTId == item.NHLTId);

                //Lấy ra od detail
                var odDetail = !string.IsNullOrEmpty(item.OutboundDelivery) && !string.IsNullOrEmpty(item.OutboundDeliveryItem) ?
                                    odDetails.FirstOrDefault(d => d.OutboundDelivery.DeliveryCodeInt == long.Parse(item.OutboundDelivery) &&
                                                             d.OutboundDeliveryItem == item.OutboundDeliveryItem) : null;

                var imgPath = string.Empty;
                //Convert Base64 to Iformfile
                if (!string.IsNullOrEmpty(item.NewImage))
                {
                    byte[] bytes = Convert.FromBase64String(item.NewImage.Substring(item.NewImage.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, item.NHLTId.ToString(), $"{item.NHLTId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NHLT");
                }


                //Chưa có thì tạo mới
                if (nhlt == null)
                {
                    _nhltRepo.Add(new GoodsReceiptTypeTModel
                    {
                        //Id
                        GoodsReceiptTypeTId = item.NHLTId,
                        //Detail od id
                        DetailODId = odDetail != null ? odDetail.DetailOutboundDeliveryId : null,
                        //Plant
                        PlantCode = item.Plant,
                        //Material
                        MaterialCode = !string.IsNullOrEmpty(item.Material) ? material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "",
                        MaterialCodeInt = !string.IsNullOrEmpty(item.Material) ? long.Parse(item.Material) : null,
                        //Batch
                        Batch = item.Batch,
                        //Số phiếu cân
                        WeightVote = item.WeightVote,
                        //Đầu cân
                        WeightHeadCode = item.WeightHeadCode,
                        //Trọng lượng cân
                        Weight = item.Weight,
                        //Confirm quantity
                        ConfirmQty = item.ConfirmQty,
                        //Unit 
                        UOM = item.Unit,
                        //SL kèm bao bì
                        QuantityWithPackaging = item.QuantityWithPackage,
                        //Số phương tiện
                        VehicleCode = item.VehicleCode,
                        //Đơn vị vận chuyển
                        TransportUnit = odDetail != null ? odDetail.OutboundDelivery.TransportUnit : "",
                        //Số lần cân
                        QuantityWeight = item.QuantityWeight,
                        Description = item.Description,
                        Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                        //Thời gian bd
                        StartTime = item.StartTime,
                        //Thời gian kết thúc
                        EndTime = item.EndTime,
                        //Sloc
                        SlocCode = item.Sloc,
                        //Sloc
                        SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                        Status = item.isDelete == true ? "DEL" : "NOT",
                        CreateBy = TokenExtensions.GetAccountId(),
                        CreateTime = DateTime.Now,
                    });
                }
                //Tồn tại thì update
                else
                {
                    //Cập nhật
                    //Detail od id
                    nhlt.DetailODId = odDetail != null ? odDetail.DetailOutboundDeliveryId : null;
                    //Material Code
                    nhlt.MaterialCode = material.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode;
                    nhlt.MaterialCodeInt = long.Parse(item.Material);
                    //Storage Location
                    nhlt.SlocCode = item.Sloc;
                    //Sloc Name
                    nhlt.SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "";
                    //Số phương tiện
                    nhlt.VehicleCode = item.VehicleCode;
                    //Đơn vị vận chuyển
                    nhlt.TransportUnit = odDetail != null ? odDetail.OutboundDelivery.TransportUnit : "";
                    //Batch
                    nhlt.Batch = item.Batch;
                    //Số cân đầu ra
                    nhlt.OutputWeight = item.OutputWeight;
                    //Confirm Quantity
                    nhlt.ConfirmQty = item.ConfirmQty;
                    //Sl kèm bao bì
                    nhlt.QuantityWithPackaging = item.QuantityWithPackage;
                    //Ghi chú
                    nhlt.Description = item.Description;
                    //Hình ảnh
                    nhlt.Image = string.IsNullOrEmpty(imgPath) ? nhlt.Image : imgPath;
                    nhlt.LastEditBy = TokenExtensions.GetAccountId();
                    nhlt.LastEditTime = DateTime.Now;
                    //Đánh dấu xóa
                    if (item.isDelete == true)
                        nhlt.Status = "DEL";
                    //Hủy đánh dấu xóa
                    if (item.isDelete == false)
                    {
                        nhlt.Status = "NOT";
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
