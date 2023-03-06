﻿using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Commands.XKLXH
{
    public class SaveXKLXHCommand : IRequest<bool>
    {
        public List<SaveXKLXH> SaveXKLXHs { get; set; } = new List<SaveXKLXH>();
    }

    public class SaveXKLXH
    {
        public Guid Id { get; set; }
        //Plant
        public string Plant { get; set; }
        //Outbound delivery
        public string OutboundDelivery { get; set; }
        //Outbound delivery item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Unit
        public string Unit { get; set; }
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
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Trọng lượng hàng hóa
        public decimal? GoodsWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạn thái
        public string Status { get; set; }
    }

    public class SaveXKLXHCommandHandler : IRequestHandler<SaveXKLXHCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ExportByCommandModel> _xklxhRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailODRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<OutboundDeliveryModel> _obRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<TruckInfoModel> _truckInfoRepo;
        private readonly IRepository<WeighSessionChoseModel> _weightSsChoseRepo;

        public SaveXKLXHCommandHandler(IUnitOfWork unitOfWork, IRepository<ExportByCommandModel> xklxhRepo, IRepository<WeighSessionModel> weightSsRepo,
                                       IRepository<ScaleModel> scaleRepo, IRepository<DetailOutboundDeliveryModel> detailODRepo, IRepository<StorageLocationModel> slocRepo,
                                       IRepository<OutboundDeliveryModel> obRepo, IUtilitiesService utilitiesService, IRepository<ProductModel> prodRepo,
                                       IRepository<TruckInfoModel> truckInfoRepo, IRepository<WeighSessionChoseModel> weightSsChoseRepo)
        {
            _unitOfWork = unitOfWork;
            _xklxhRepo = xklxhRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _detailODRepo = detailODRepo;
            _slocRepo = slocRepo;
            _obRepo = obRepo;
            _utilitiesService = utilitiesService;
            _prodRepo = prodRepo;
            _truckInfoRepo = truckInfoRepo;
            _weightSsChoseRepo = weightSsChoseRepo;
        }

        public async Task<bool> Handle(SaveXKLXHCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query truck info
            var truckInfos = _truckInfoRepo.GetQuery().AsNoTracking();

            //Danh sách xuất kho theo lxh
            var xklxhs = await _xklxhRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = xklxhs.Count() > 0 ? xklxhs.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            //Query od
            var detailODs = _detailODRepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();
            var ods = _obRepo.GetQuery().AsNoTracking();

            var index = 1;
            foreach (var item in request.SaveXKLXHs)
            {
                var ExportByCommandId = Guid.NewGuid();

                ////Lấy ra dòng dữ liệu đã lưu
                //var record = await _xklxhRepo.FindOneAsync(n => n.ExportByCommandId == item.Id);

                ////Lấy ra dòng dữ liệu mapping với đợt cân
                //var weightSsChose = await _weightSsChoseRepo.FindOneAsync(w => w.RecordId == item.Id);

                ////Check status
                //if (item.Status == "DAXOA")
                //{

                //    _weightSsChoseRepo.Remove(weightSsChose);

                //    _xklxhRepo.Remove(record);

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

                //var ExportByCommandÍd = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, ExportByCommandId.ToString(), $"{ExportByCommandId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "XKLXH");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra đợt cân
                var weightSession = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                 weightSs.Where(x => x.ScaleCode == scale.ScaleCode).OrderByDescending(x => x.OrderIndex).FirstOrDefault() : null;

                //Lấy ra outbound detail
                var detailOb = !string.IsNullOrEmpty(item.OutboundDelivery) ?
                               detailODs.FirstOrDefault(d => d.OutboundDelivery.DeliveryCodeInt == long.Parse(item.OutboundDelivery) && d.OutboundDeliveryItem == item.OutboundDeliveryItem) : null;

                //Nếu có đợt cân thì lưu vào bảng mapping
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


                _xklxhRepo.Add(new ExportByCommandModel
                {
                    //1 Export by command id
                    ExportByCommandId = ExportByCommandId,
                    //2 WeightSession
                    DateKey = weightSession != null ?
                               weightSession.DateKey : null,
                    OrderIndex = weightSession != null ?
                               weightSession.OrderIndex : null,
                    //3 WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,
                    StartTime = weightSession != null ?
                               weightSession.StartTime : null,
                    EndTime = DateTime.Now,
                    //Material
                    MaterialCode = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material)).ProductCode,
                    MaterialCodeInt = long.Parse(item.Material),
                    //Số phương tiện
                    VehicleCode = item.VehicleCode,
                    //UOM
                    UOM = item.Unit,
                    //Sloc
                    SlocCode = item.Sloc,
                    SlocName = !string.IsNullOrEmpty(item.Sloc) ? slocs.FirstOrDefault(s => s.StorageLocationCode == item.Sloc).StorageLocationName : null,
                    //Batch
                    Batch = item.Batch,
                    //SL bao
                    BagQuantity = item.BagQuantity,
                    //Đơn trọng
                    SingleWeight = item.SingleWeight,
                    //Trọng lượng cân
                    Weight = item.Weight,
                    //Confirm quantity
                    ConfirmQty = item.ConfirmQty,
                    //Sl kèm bao bì
                    QuantityWithPackaging = item.QuantityWithPackage,
                    //Số xe tải
                    TruckInfoId = item.TruckInfoId.HasValue ? item.TruckInfoId : null,
                    TruckNumber = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(t => t.TruckInfoId == item.TruckInfoId).TruckNumber : null,
                    //Số lần cân
                    QuantityWeight = item.QuantityWeight,
                    //4 WeightVote
                    WeightVote = $"X{long.Parse(lastIndex) + index}",
                    //5   DetailODId
                    DetailODId = !string.IsNullOrEmpty(item.OutboundDelivery) ?
                                 detailOb.DetailOutboundDeliveryId : null,
                    //6   PlantCode
                    PlantCode = item.Plant,
                    //Input weight
                    InputWeight = item.InputWeight,
                    //Output weight
                    OutputWeight = item.OutputWeight,
                    //Trọng lượng hàng hóa
                    GoodsWeight = item.GoodsWeight,
                    //40  Description
                    Description = item.Description,
                    //41  Image
                    Image = string.IsNullOrEmpty(imgPath) ? null : Path.Combine(new ConfigManager().DocumentDomainUpload + imgPath),
                    //42  Status
                    Status = "NOT",
                    //48  CreateTime
                    CreateTime = DateTime.Now,
                    //49  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //52  Actived
                    Actived = true

                });

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
