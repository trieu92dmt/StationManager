namespace StationManager.Application.DTOs.CarCompany
{
    public class TripDataResponse
    {
        //Seat Diagram
        public SeatDiagramResponse SeatDiagram { get; set; }
        //Danh sách vé đã bán
        public List<TicketData> TicketDatas { get; set; } = new List<TicketData>();

    }

    public class TicketData
    {
        //Mã vé
        public int TicketCode { get; set; }
        //Tên người đặt/Mua
        public string Name { get; set; }
        //Số điện thoại
        public string PhoneNumber { get; set; }
        //Email
        public string Email { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Ngày đặt/mua
        public DateTime? CreateTime { get; set; }
        //Thành tiền
        public decimal? Price { get; set; }
        //Danh sách ghế
        public string Seats { get; set; }
    }
}
