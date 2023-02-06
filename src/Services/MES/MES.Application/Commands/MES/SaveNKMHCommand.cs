using ISD.Core.Extensions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Core.Utilities;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Net.WebSockets;

namespace MES.Application.Commands.MES
{
    public class SaveNKMHCommand : IRequest<bool>
    {
        public List<NKMHRequest> NKMHRequests { get; set; } = new List<NKMHRequest>();
    }

    public class NKMHRequest
    {
        //Plant
        public string PlantCode { get; set; }
        //Material
        public string MaterialCode { get; set; }
        //Sloc
        public string SlocCode { get; set; }
        //SL bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confỉm Qty
        public decimal? ConfirmQty { get; set; }
        //Số lần cân
        public int QuantityWeight { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackaging { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Id cân xe tải
        public Guid? TruckInfoId { get; set; }
        //Số xe tải
        public string TruckQuantity { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        public Guid? PoDetailId { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //số lô
        public string Batch { get; set; }
    }

    public class SaveNKMHCommandHandler : IRequestHandler<SaveNKMHCommand, bool>
    {
        private readonly IRepository<GoodsReceiptModel> _nkRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<ProductModel> _prdRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IUtilitiesService _utilitiesService;

        public SaveNKMHCommandHandler(IRepository<GoodsReceiptModel> nkRep, IUnitOfWork unitOfWork,
                                      IRepository<PurchaseOrderDetailModel> poDetailRep, IRepository<StorageLocationModel> slocRepo,
                                      IRepository<ProductModel> prdRepo, IRepository<ScaleModel> scaleRepo, IRepository<WeighSessionModel> weightSsRepo,
                                      IUtilitiesService utilitiesService)
        {
            _nkRep = nkRep;
            _unitOfWork = unitOfWork;
            _poDetailRep = poDetailRep;
            _slocRepo = slocRepo;
            _prdRepo = prdRepo;
            _scaleRepo = scaleRepo;
            _weightSsRepo = weightSsRepo;
            _utilitiesService = utilitiesService;
        }
        public async Task<bool> Handle(SaveNKMHCommand request, CancellationToken cancellationToken)
        {

            //Danh sách storage location
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Danh sách nhập kho mua hàng
            var nkmh = await _nkRep.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nkmh.OrderBy(x => x.WeitghtVote).LastOrDefault();

            //Dữ liệu đợt cân
            var weightSs = _weightSsRepo.GetQuery().Include(x => x.Scale).AsNoTracking();

            //Danh sách material
            var materials = _prdRepo.GetQuery().AsNoTracking();

            int index = 1;

            foreach (var x in request.NKMHRequests)
            {

                var poLine = await _poDetailRep.GetQuery(p => p.PurchaseOrderDetailId == x.PoDetailId)
                                               .Include(x => x.PurchaseOrder)
                                               .FirstOrDefaultAsync();

                var imgPath = "";
                if (!string.IsNullOrEmpty(x.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(x.Image.Substring(x.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, poLine.PurchaseOrderDetailId.ToString(), $"{poLine.PurchaseOrderDetailId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKMH");
                }
                

                //Save data nhập kho mua hàng
                _nkRep.Add(new GoodsReceiptModel
                {
                    //Số lô
                    Batch = x.Batch,
                    GoodsReceiptId = Guid.NewGuid(),
                    //POLine
                    PurchaseOrderDetailId = poLine?.PurchaseOrderDetailId,
                    //Mã đầu cân
                    WeightHeadCode = x.WeightHeadCode,
                    //PlantCode
                    PlantCode = x.PlantCode,
                    //Material Desc
                    MaterialCode = materials.FirstOrDefault(m => m.ProductCodeInt == long.Parse(x.MaterialCode)).ProductCode,
                    MaterialCodeInt = long.Parse(x.MaterialCode),
                    //Sloc code
                    SlocCode = x.SlocCode,
                    //Sloc Name
                    SlocName = !x.SlocCode.IsNullOrEmpty() ? slocs.FirstOrDefault(s => s.StorageLocationCode == x.SlocCode).StorageLocationName : null,
                    //SL bao
                    BagQuantity = x.BagQuantity,
                    //Đơn trọng
                    SingleWeight = x.SingleWeight,
                    //Trọng lượng cân
                    Weight = x.Weight,
                    //Confirm Qty
                    ConfirmQty = x.ConfirmQty,
                    //Số lần cân
                    QuantityWeitght = x.QuantityWeight,
                    //Sl kèm bao bì
                    QuantityWithPackaging = x.QuantityWithPackaging,
                    //Số phương tiện
                    VehicleCode = x.VehicleCode,
                    //Id cân xe tải
                    TruckInfoId = x.TruckInfoId.HasValue ? x.TruckInfoId : null,    
                    //Số xe tải 
                    TruckQuantity = x.TruckQuantity,
                    //Số cân đầu vào
                    InputWeight = x.InputWeight,
                    OutputWeight = x.OutputWeight,
                    //Ghi chú
                    Description = x.Description,
                    //Hình ảnh
                    //Img = !string.IsNullOrEmpty(x.Image) ? System.Convert.FromBase64String(x.Image.Substring(x.Image.IndexOf(',')+1)) : null,
                    Img = string.IsNullOrEmpty(imgPath) ? "" : imgPath,
                    //document date = document date po
                    DocumentDate = x.PoDetailId.HasValue ? poLine.PurchaseOrder.DocumentDate : null,
                    //Số phiếu cân
                    WeitghtVote = $"N{ long.Parse(lastIndex.WeitghtVote.Substring(1)) + index}",
                    //Common
                    DateKey = int.Parse(DateTime.Now.ToString(DateTimeFormat.DateKey)),

                    CreateTime = DateTime.Now,
                    CreateBy = TokenExtensions.GetAccountId(),
                    Actived = true,
                    //Status
                    Status = "NOT",
                    //Start Time - End Time
                    StartTime = !string.IsNullOrEmpty(x.WeightHeadCode) ? weightSs.FirstOrDefault(w => w.Scale.ScaleCode == x.WeightHeadCode).StartTime : DateTime.Now,
                    EndTime = DateTime.Now,
                });

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
