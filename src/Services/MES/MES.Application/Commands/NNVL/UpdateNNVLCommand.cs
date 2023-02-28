using Core.Interfaces.Databases;
using Core.Models;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NNVL
{
    public class UpdateNNVLCommand : IRequest<ApiResponse>
    {
        public List<UpdateNNVL> UpdateNNVLs { get; set; } = new List<UpdateNNVL>();
    }

    public class UpdateNNVL
    {
        //Id
        public Guid NNVLGCId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Material
        public string Material { get; set; }
        //Sloc
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public decimal? QuantityWeight { get; set; }
        //Unit
        public string Unit { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Thời gian bd
        public DateTime? StartTime { get; set; }
        //Thời gian kt
        public DateTime? EndTime{ get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        public string NewImage { get; set; }
        //Đánh dấu xóa
        public bool? isDelete { get; set; }

    }

    public class UpdateNNVLCommandHandler : IRequestHandler<UpdateNNVLCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ComponentImportModel> _nnvlgcRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<PurchaseOrderDetailModel> _detailPoRepo;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;
        private readonly IRepository<PlantModel> _plantRepo;
        private readonly IRepository<TruckInfoModel> _truckRepo;
        public UpdateNNVLCommandHandler(IUnitOfWork unitOfWork, IRepository<ComponentImportModel> nnvlgcRepo, IRepository<WeighSessionModel> weightSsRepo,
                                        IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService, IRepository<PurchaseOrderDetailModel> detailPoRepo,
                                        IRepository<ProductModel> prodRepo, IRepository<StorageLocationModel> slocRepo, IRepository<PlantModel> plantRepo, IRepository<TruckInfoModel> truckRepo)
        {
            _unitOfWork = unitOfWork;
            _nnvlgcRepo = nnvlgcRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _detailPoRepo = detailPoRepo;
            _prodRepo = prodRepo;
            _slocRepo = slocRepo;
            _plantRepo = plantRepo;
            _truckRepo = truckRepo;
        }

        public Task<ApiResponse> Handle(UpdateNNVLCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
