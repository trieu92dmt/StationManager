using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;

namespace MES.Application.Commands.XK
{
    public class SaveXKCommand : IRequest<bool>
    {
        public List<DataSaveNK> DataSaveNKs { get; set; } = new List<DataSaveNK>();
    }

    public class DataSaveNK 
    {
        //Plant
        public string Plant { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Reservation item
        public string ReservationItem { get; set; }
        //Material
        public string Material { get; set; }
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
        //Mã đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm qty
        public decimal? ConfirmQty { get; set; }
        //Số lượng kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Customer
        public string Customer { get; set; }
        //Special Stock
        public string SpecialStock { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Số xe tải
        public Guid TruckInfoId { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
    }

    //public class SaveXKCommandHandler : IRequestHandler<SaveXKCommand, bool>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IRepository<WeighSessionModel> _weightSsRepo;
    //    private readonly IRepository<ScaleModel> _scaleRepo;
    //    private readonly IUtilitiesService _utilitiesService;
    //    private readonly IRepository<ProductModel> _prodRepo;
    //    private readonly IRepository<IssueForProductionModel> _xkRepo;
    //    private readonly IRepository<DetailOutboundDeliveryModel> _detalOd;
    //    private readonly IRepository<StorageLocationModel> _slocRepo;
    //    public SaveXKCommandHandler(IUnitOfWork unitOfWork, IRepository<WeighSessionModel> weightSsRepo,
    //                                IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService,
    //                                IRepository<ProductModel> prodRepo, IRepository<OtherExportModel> xkRepo,
    //                                IRepository<DetailOutboundDeliveryModel> detalOd, IRepository<StorageLocationModel> slocRepo)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _weightSsRepo = weightSsRepo;
    //        _scaleRepo = scaleRepo;
    //        _utilitiesService = utilitiesService;
    //        _prodRepo = prodRepo;
    //        _xkRepo = xkRepo;
    //        _detalOd = detalOd;
    //        _slocRepo = slocRepo;
    //    }

    //    public Task<bool> Handle(SaveXKCommand request, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
