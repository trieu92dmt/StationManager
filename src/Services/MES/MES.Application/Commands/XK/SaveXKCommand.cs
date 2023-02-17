using MediatR;

namespace MES.Application.Commands.XK
{
    public class SaveXKCommand : IRequest<bool>
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
        //Số lần cân
        //Unit
        //Hình ảnh
        //Trạng thái
        //Số xe tải
        //Số cân đầu ra
    }
}
