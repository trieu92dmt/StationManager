using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
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

namespace MES.Application.Commands.NKDCNB
{
    public class SaveNKDCNBCommand : IRequest<bool>
    {
        public List<SaveNKDCNB> SaveNKDCNBs { get; set; } = new List<SaveNKDCNB>();
    }

    public class SaveNKDCNB
    {
        //Plant
        public string Plant { get; set; }
        //Outbound delivery
        public string OutboundDelivery { get; set; }
        //Outbound delivery Item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Batch
        public string Batch { get; set; }
        //UOM
        public string Unit { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //Số lượng kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Id - Số xe tải
        public Guid? TruckInfoId { get; set; }
        public string TruckNumber { get; set; }
        //Số cân đầu vào
        public decimal? IntputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
    }

    public class SaveNKDCNBCommandHandler : IRequestHandler<SaveNKDCNBCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<InhouseTransferModel> _nkdcnbRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailOdRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;

        public SaveNKDCNBCommandHandler(IUnitOfWork unitOfWork, IRepository<InhouseTransferModel> nkdcnbRepo, IUtilitiesService utilitiesService, IRepository<ScaleModel> scaleRepo,
                                        IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<WeighSessionModel> weightSsRepo,
                                        IRepository<DetailOutboundDeliveryModel> detailOdRepo, IRepository<TruckInfoModel> truckRepo)
        {
            _unitOfWork = unitOfWork;
            _nkdcnbRepo = nkdcnbRepo;
            _utilitiesService = utilitiesService;
            _scaleRepo = scaleRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _weightSsRepo = weightSsRepo;
            _detailOdRepo = detailOdRepo;
            _truckRepo = truckRepo;
        }

        public async Task<bool> Handle(SaveNKDCNBCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Get query od detail
            var detailOds = _detailOdRepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();

            //Get query truck info
            var truckInfos = _truckRepo.GetQuery().AsNoTracking();

            //Danh sách nhập kho tpsx
            var nkdcnbs = await _nkdcnbRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nkdcnbs.Count() > 0 ? nkdcnbs.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            var index = 1;
            foreach (var item in request.SaveNKDCNBs)
            {
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

                var InhouseTranferId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, InhouseTranferId.ToString(), $"{InhouseTranferId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKDCNB");
                }

                //Lấy od detail
                var detailOd = detailOds.FirstOrDefault(x => !string.IsNullOrEmpty(item.OutboundDelivery) && !string.IsNullOrEmpty(item.OutboundDeliveryItem) ?
                                                             x.OutboundDelivery.DeliveryCodeInt == long.Parse(item.OutboundDelivery) &&
                                                             x.OutboundDeliveryItem == item.OutboundDeliveryItem : false);

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);


                _nkdcnbRepo.Add(new InhouseTransferModel
                {
                    //1 NKDCNB Id
                    InhouseTransferId = InhouseTranferId,
                    //2 Detail outbound delivery id
                    DetailODId = detailOd != null ? detailOd.DetailOutboundDeliveryId : null,
                    //3 PlantCode
                    PlantCode = item.Plant,
                    //4   MaterialCode
                    MaterialCode = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Material) && x.PlantCode == item.Plant).ProductCode,
                    MaterialCodeInt = long.Parse(item.Material),
                    //Batch
                    Batch = item.Batch,
                    //5   WeightId
                    WeightSessionId = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.WeighSessionID : null,
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
                    //VehicleCode
                    VehicleCode = item.VehicleCode,
                    //Số lần cân
                    QuantityWeitght = item.QuantityWeight,
                    //UOM
                    UOM = item.Unit,
                    //Số cân đầu vào
                    InputWeight = item.IntputWeight,
                    //Số cân đầu ra
                    OutputWeight = item.OutputWeight,
                    //14  Description
                    Description = item.Description,
                    //15  Image
                    Image = !string.IsNullOrEmpty(imgPath) ? imgPath : null,
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.StartTime : null,
                    //18  EndTime
                    EndTime = DateTime.Now,
                    //21  SlocCode
                    SlocCode = item.Sloc,
                    //22  SlocName
                    SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.Sloc)?.StorageLocationName,
                    //Truckinfo
                    TruckInfoId = item.TruckInfoId,
                    //TruckNumber
                    TruckNumber = item.TruckInfoId.HasValue ? truckInfos.FirstOrDefault(t => t.TruckInfoId == item.TruckInfoId).TruckNumber : null,
                    //24  CreateTime
                    CreateTime = DateTime.Now,
                    //25  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //28  Actived
                    Actived = true

                });

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
