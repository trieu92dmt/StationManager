namespace StationManager.Application.DTOs.CarCompany.Admin
{
    public class CarCompanyDetailResponse
    {
        public Guid CarCompanyId { get; set; }
        //Mã nhà xe
        public int CarCompanyCode { get; set; }
        //Tên nhà xe
        public string CarCompanyName { get; set; }
        //Tên đăng nhập
        public string Username { get; set; }
        //Email
        public string Email { get; set; }
        //Hotline
        public string Hotline { get; set; }
        //Số điện thoại
        public string PhoneNumber { get; set; }
        //Địa chỉ văn phòng
        public string OfficeAddress { get; set; }
        //Mô tả
        public string Description { get; set; }
        //Ngày tạo
        public DateTime? CreateTime { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Số lượng xe đang quản lý
        public int CarQuantity { get; set; }
        //Số lượng tuyến đang quản lý
        public int RouteQuantity { get; set; }
        //Số lượng chuyến xe cao nhất/ngày
        public int MaxTripQuantity { get; set; }
        //Ngày hết hạn
        public DateTime? ExpireDate { get; set; }
        //Các gói đã đăng ký
        public List<Package> Packages { get; set; } = new List<Package>();
        //Danh sách tuyến xe
        public List<Route> Routes { get; set; } = new List<Route>();
        //Danh sách các đánh giá
        public List<Rate> Rates { get; set; } = new List<Rate>();
    }

    public class Package
    {
        //Id gói
        public Guid PackageId { get; set; }
        //Mã gói
        public string PackageCode { get; set; }
        //Tên gói
        public string PackageName { get; set; }
        //Thời hạn
        public int Duration { get; set; }
        //Giá
        public decimal Price { get; set; }
        //Số lượng xe giới hạn
        public int CarLimit { get; set; }
        //Số lượng tuyến giới hạn
        public int RouteLimit { get; set; }
        //Số lượng chuyến/1 ngày
        public int TripPerDay { get; set; }
        //Ngày đăng ký
        public DateTime? CreateTime { get; set; }
    }

    public class Route
    {
        //Id tuyến
        public Guid RouteId { get; set; }
        //Mã tuyến
        public string RouteCode { get; set; }
        //Điểm đi
        public string StartPoint { get; set; }
        //Điểm đến
        public string EndPoint { get; set; }
        //Số chuyến đã thực hiện
        public int TripQuantity { get; set; }
        //Ngày tạo
        public DateTime? CreateTime { get; set; }
        //Trạng thái
        public bool? Active { get; set; }
    }

    public class Rate
    {
        //Id rate
        public Guid RateId { get; set; }
        //Điểm đánh giá
        public decimal RatePoint { get; set; }
        //Nội dung
        public string Content { get; set; }
        //Ngày đánh giá
        public DateTime? CreateTime { get; set; }
    }
}
