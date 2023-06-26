using Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Request
{
    public class SearchTripRequest
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        //Điểm đi
        public string StartPoint { get; set; }
        //Điểm đến
        public string EndPoint { get; set; }
        //Ngày đi
        public DateTime? StartDate { get; set; }
        //Giá vé từ
        public decimal? PriceFrom { get; set; }
        //Giá vé đến
        public decimal? PriceTo { get; set; }
        //Số ghế trống
        public int? EmptySeat { get; set; }
        //Hàng ghế đầu
        public bool? isFirstRow { get; set; }
        //Hàng ghế giữa
        public bool? isMiddleRow { get; set; }
        //Hàng ghế cuối
        public bool? isLastRow { get; set; }
        //Nhà xe
        public List<int> ListCarCompany { get; set; } = new List<int>();
        //Đánh giá
        public decimal? RatePointFrom { get; set; }
    }
}
