using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany
{
    public class TripSearchResponse
    {
        public int STT { get; set; }
        //Id chuyến
        public Guid TripId { get; set; }
        public Guid? CompanyId { get; set; }
        //Tên nhà xe
        public string CompanyName { get; set; }
        //Điểm đánh giá
        public decimal RatePoint { get; set; }
        //Số lượt đánh giá
        public int RateCount { get; set; }
        //Giá vé
        public decimal TicketPrice { get; set; }
        //Số chỗ ngồi
        public int SeatTotal { get; set; }
        //Số chỗ trống
        public int EmptySeat { get; set; }
        //Loại xe
        public string CarType { get; set; }
        //Ngày đi
        public DateTime? StartDate { get; set; }
        //Giờ đi
        public string StartTime { get; set; }
        //Điểm đi
        public string StartPoint { get; set; }
        //Giờ đến
        public string EndTime { get; set; }
        //Điểm đến
        public string EndPoint { get; set; }
        //Ảnh nhà xe
        public string Image { get; set; }
    }
}
