using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NHLT
{
    public class SaveNHLTCommand : IRequest<bool>
    {
        public List<SaveNHLT> SaveNHLTs { get; set; } = new List<SaveNHLT>();
    }

    public class SaveNHLT
    {
        //public Guid Id { get; set; }
        //Plant
        public string Plant { get; set; }
        //Od
        public string OutboundDelivery { get; set; }
        //Od item
        public string OutboundDeliveryItem { get; set; }
        //Customer
        public string Customer { get; set; }
        //Material
        public string Material { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //Sl kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Unit
        public string Unit { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
        //Số cân đầu vào 
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }    
    }

    public class SaveNHLTCommandHandler : IRequestHandler<SaveNHLTCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<GoodsReceiptTypeTModel> _nhltRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _odDetailRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<WeighSessionChoseModel> _weightSsChoseRepo;

        public SaveNHLTCommandHandler(IUnitOfWork unitOfWork, IRepository<WeighSessionModel> weightSsRepo, IRepository<TruckInfoModel> truckRepo,
                                      IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService,
                                      IRepository<ProductModel> prodRepo, IRepository<GoodsReceiptTypeTModel> nhltRepo,
                                      IRepository<DetailOutboundDeliveryModel> odDetailRepo, IRepository<StorageLocationModel> slocRepo,
                                      IRepository<WeighSessionChoseModel> weightSsChoseRepo)
        {
            _unitOfWork = unitOfWork;
            _weightSsRepo = weightSsRepo;
            _truckRepo = truckRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _prodRepo = prodRepo;
            _nhltRepo = nhltRepo;
            _odDetailRepo = odDetailRepo;
            _slocRepo = slocRepo;
            _weightSsChoseRepo = weightSsChoseRepo;
        }

        public async Task<bool> Handle(SaveNHLTCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query truck info
            var truckInfos = _truckRepo.GetQuery().AsNoTracking();

            //Danh sách nhập hàng loại T
            var nhlts = await _nhltRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nhlts.Count() > 0 ? nhlts.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            //Query od
            var detailOds = _odDetailRepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();

            var index = 1;
            foreach (var item in request.SaveNHLTs)
            {

                ////Lấy ra dòng dữ liệu đã lưu
                //var record = await _nhltRepo.FindOneAsync(n => n.GoodsReceiptTypeTId == item.Id);

                ////Lấy ra dòng dữ liệu mapping với đợt cân
                //var weightSsChose = await _weightSsChoseRepo.FindOneAsync(w => w.RecordId == item.Id);

                ////Check status
                //if (item.Status == "DAXOA")
                //{

                //    _weightSsChoseRepo.Remove(weightSsChose);

                //    _nhltRepo.Remove(record);

                //    await _unitOfWork.SaveChangesAsync();

                //    continue;
                //}

                //Check điều kiện lưu
                #region Check điều kiện lưu

                if (!item.ConfirmQty.HasValue || item.ConfirmQty <= 0)
                {
                    throw new ISDException("Confirm Quantity phải lớn hơn 0");
                }

                if (string.IsNullOrEmpty(item.WeightHeadCode))
                {
                    if (!item.BagQuantity.HasValue || item.BagQuantity <= 0)
                    {
                        throw new ISDException("Số lượng bao phải lớn hơn 0");
                    }
                    if (!item.SingleWeight.HasValue || item.SingleWeight <= 0)
                    {
                        throw new ISDException("Đơn trọng phải lớn hơn 0");
                    }
                }
                #endregion

                var GoodsReceiptTypeTId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, GoodsReceiptTypeTId.ToString(), $"{GoodsReceiptTypeTId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NHLT");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra đợt cân
                var weightSession = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                 weightSs.Where(x => x.ScaleCode == scale.ScaleCode).OrderByDescending(x => x.OrderIndex).FirstOrDefault() : null;

                //Lấy ra component
                //Lấy ra outbound detail
                var detailOd = !string.IsNullOrEmpty(item.OutboundDelivery) ?
                               detailOds.FirstOrDefault(d => d.OutboundDelivery.DeliveryCodeInt == long.Parse(item.OutboundDelivery) && d.OutboundDeliveryItem == item.OutboundDeliveryItem) : null;

                ////Nếu có đợt cân thì lưu vào bảng mapping
                //if (weightSession != null)
                //{
                //    if (weightSsChose != null)
                //        _weightSsChoseRepo.Add(new WeighSessionChoseModel
                //        {
                //            Id = Guid.NewGuid(),
                //            DateKey = weightSession.DateKey,
                //            OrderIndex = weightSession.OrderIndex,
                //            ScaleCode = weightSession.ScaleCode,
                //            RecordId = item.Id
                //        });
                //}

                _nhltRepo.Add(new GoodsReceiptTypeTModel
                {
                    //1 GoodsReceiptTypeTId
                    GoodsReceiptTypeTId = GoodsReceiptTypeTId,
                    //2 Detail Od
                    DetailODId = detailOd != null ? detailOd.DetailOutboundDeliveryId : null,
                    //3 PlantCode
                    PlantCode = item.Plant,
                    //4   MaterialCode
                    MaterialCode = !string.IsNullOrEmpty(item.Material) ? prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode : "",
                    MaterialCodeInt = !string.IsNullOrEmpty(item.Material) ? long.Parse(item.Material) : null,
                    //Batch
                    Batch = item.Batch,
                    //Customer
                    Customer = item.Customer,
                    //2 WeightSession
                    DateKey = weightSession != null ?
                               weightSession.DateKey : null,
                    OrderIndex = weightSession != null ?
                               weightSession.OrderIndex : null,
                    //6 WeightVote
                    WeightVote = $"N{long.Parse(lastIndex) + index}",
                    //7   BagQuantity
                    BagQuantity = item.BagQuantity,
                    //8   SingleWeight
                    SingleWeight = item.SingleWeight,
                    //9  WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,
                    //10  Weight
                    Weight = item.Weight,
                    //11  ConfirmQty
                    ConfirmQty = item.ConfirmQty,
                    //12  QuantityWithPackaging
                    QuantityWithPackaging = item.QuantityWithPackage,
                    //13  QuantityWeitght
                    QuantityWeight = item.QuantityWeight,
                    //14  Description
                    Description = item.Description,
                    //15  Image
                    Image = string.IsNullOrEmpty(imgPath) ? null : imgPath,
                    //16  Status
                    Status = "NOT",
                    //Unit
                    UOM = item.Unit,
                    //17  StartTime
                    StartTime = weightSession != null ?
                               weightSession.StartTime : null,
                    //18  EndTime
                    EndTime = item.BagQuantity.HasValue && item.BagQuantity > 0 && item.SingleWeight.HasValue && item.SingleWeight > 0 ? null : DateTime.Now,
                    //số cân đầu vào
                    InputWeight = item.InputWeight,
                    //số cân đầu ra
                    OutputWeight = item.OutputWeight,
                    //Số phương tiện
                    VehicleCode = item.VehicleCode,
                    //Đơn vị vận chuyển
                    TransportUnit = detailOd != null ? detailOd.OutboundDelivery.TransportUnit : null,
                    //21  SlocCode
                    SlocCode = item.Sloc,
                    //Số xe tải
                    TruckInfoId = item.TruckInfoId,
                    TruckNumber = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(t => t.TruckInfoId == item.TruckInfoId).TruckNumber : null,
                    //22  SlocName
                    SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc).StorageLocationName : "",
                    //24  CreateTime
                    CreateTime = DateTime.Now,
                    //25  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //28  Actived
                    Actived = true

                });;

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
