using ISD.API.Constant.Common;
using ISD.API.EntityModels.Models;
using ISD.API.Extensions.Jwt;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.Applications.Commands.MES
{
    public class SaveNKMHCommand : IRequest<bool>
    {
        public string POCode { get; set; }
        public string POLine { get; set; }
        public string WeightId { get; set; }
        public string WeitghtVote { get; set; }
        public decimal? BagQuantity { get; set; }
        public decimal? SingleWeight { get; set; }
        public string WeightHeadCode { get; set; }
        public decimal? Weight { get; set; }
        public decimal? ConfirmQty { get; set; }
        public string VehicleCode { get; set; }
        public decimal? QuantityWeitght { get; set; }
        public decimal? QuantityWithPackaging { get; set; }
        public decimal? TotalQuantity { get; set; }
        public decimal? DeliveredQuantity { get; set; }
        public int? TruckQuantity { get; set; }
        public decimal? InputWeight { get; set; }
        public decimal? OutputWeight { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class SaveNKMHCommandHandler : IRequestHandler<SaveNKMHCommand, bool>
    {
        private readonly IRepository<GoodsReceiptModel> _nkRep;
        private readonly IISDUnitOfWork _unitOfWork;
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;

        public SaveNKMHCommandHandler(IRepository<GoodsReceiptModel> nkRep, IISDUnitOfWork unitOfWork, 
                                      IRepository<PurchaseOrderMasterModel> poRep)
        {
            _nkRep = nkRep;
            _unitOfWork = unitOfWork;
            _poRep = poRep;
        }
        public async Task<bool> Handle(SaveNKMHCommand request, CancellationToken cancellationToken)
        {
            var po = await _poRep.GetQuery(x => x.PurchaseOrderCode == request.POCode).Include(x => x.PurchaseOrderDetailModel).FirstOrDefaultAsync();

            var poLine = po?.PurchaseOrderDetailModel.FirstOrDefault(x => x.POLine == request.POLine);

            //Save data nhập kho mua hàng
            _nkRep.Add(new GoodsReceiptModel
            {
                GoodsReceiptId = Guid.NewGuid(),

                //Số phiếu cân
                WeitghtVote = request.WeitghtVote,

                //POLine
                PurchaseOrderDetailId = poLine?.PurchaseOrderDetailId,
                //Số lượng bao
                BagQuantity = request.BagQuantity,
                //Đơn trọng
                SingleWeight = request.SingleWeight,
                //ID đợt cân
                WeightId = request.WeightId,
                //Mã đầu cân
                WeightHeadCode = request.WeightHeadCode,
                //Trọng lượng cân
                Weight = request.Weight,
                //Confirm Quantity
                ConfirmQty = request.ConfirmQty,
                //Số phương tiện
                VehicleCode = request.VehicleCode,
                //Số lần cân
                QuantityWeitght = request.QuantityWeitght,

                TotalQuantity = request.TotalQuantity,
                DeliveredQuantity = request.DeliveredQuantity,

                //Số lượng kèm bao bì
                QuantityWithPackaging = request.QuantityWithPackaging,

                //Số lượng xe tải
                TruckQuantity = request.TruckQuantity,

                //Số cân đầu vào
                InputWeight = request.InputWeight,
                //Sô cân đầu ra
                OutputWeight = request.OutputWeight,
                //Ghi chú
                Description = request.Description,
                Image = request.Image,
                Status = request.Status,

                //Thời gian bắt đầu & kết thúc
                StartTime = request.StartTime,
                EndTime = request.EndTime,

                DocumentDate = DateTime.Now,

                //Common
                DateKey = int.Parse(DateTime.Now.ToString(ISDDateTimeFormat.DateKey)),

                CreateTime = DateTime.Now,
                CreateBy = TokenExtensions.GetAccountId(),
                Actived = true

            });

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
