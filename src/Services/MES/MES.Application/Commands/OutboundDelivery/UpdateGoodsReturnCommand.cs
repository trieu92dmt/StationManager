//using ISD.Core.Interfaces.Databases;
//using ISD.Core.SeedWork.Repositories;
//using ISD.Infrastructure.Models;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MES.Application.Commands.OutboundDelivery
//{
//    public class UpdateGoodsReturnCommand : IRequest<bool>
//    {
//        public List<UpdateGoodsReturn> UpdateGoodsReturns { get; set; } = new List<UpdateGoodsReturn>();
//    }

//    public class UpdateGoodsReturn
//    {
//        //Id nhập kho hàng trả
//        public Guid NKHTId { get; set; }
//        //Plant
//        public string Plant { get; set; }
//        //Purchase Order
//        public string PurchaseOrderCode { get; set; }
//        //PO Item
//        public string POItem { get; set; }
//        //Material
//        public string Material { get; set; }
//        //Storage Location
//        public string StorageLocation { get; set; }
//        //Batch
//        public string Batch { get; set; }
//        //Sl bao
//        public decimal? BagQuantity { get; set; }
//        //Đơn trọng
//        public decimal? SingleWeight { get; set; }
//        //Đầu cân
//        public string WeightHeadCode { get; set; }
//        //Trọng lượng cân
//        public decimal? Weight { get; set; }
//        //Confirm Quantity
//        public decimal? ConfirmQty { get; set; }
//        //SL kèm bao bì
//        public decimal? QuantityWithPackaging { get; set; }
//        //Số phương tiện
//        public string VehicleCode { get; set; }
//        //Số lần cân
//        public int? QuantityWeight { get; set; }
//        //Id cân xe tải
//        public Guid? TruckInfoId { get; set; }
//        //Số xe tải
//        public string TruckQty { get; set; }
//        //Số cân đầu vào
//        public decimal? InputWeight { get; set; }
//        //Số cân đầu ra
//        public decimal? OutputWeight { get; set; }
//        //Số phiếu cân
//        public string WeightVote { get; set; }
//        //DocumentDate
//        public DateTime? DocumentDate { get; set; }
//        //Thời gian bắt đầu
//        public DateTime? StartTime { get; set; }
//        //Thời gian kết thúc
//        public DateTime? EndTime { get; set; }
//        //Create By
//        public Guid? CreateBy { get; set; }
//        //Create On
//        public DateTime? CreateOn { get; set; }
//        //Change By
//        public Guid? ChangeBy { get; set; }
//        //Ghi chú
//        public string Description { get; set; }
//        //Hình ảnh
//        public string Image { get; set; }
//        public string NewImage { get; set; }
//        //Đánh dấu xóa
//        public bool? isDelete { get; set; }
//    }

//    public class UpdateGoodsReturnCommandHandler : IRequestHandler<UpdateGoodsReturnCommand, bool>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IRepository<GoodsReturnModel> _nkhtRepo;

//        public UpdateGoodsReturnCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReturnModel> nkhtRepo)
//        {
//            _unitOfWork = unitOfWork;
//            _nkhtRepo = nkhtRepo;
//        }

//        public Task<bool> Handle(UpdateGoodsReturnCommand request, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
