using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany
{
    public class DetailCarCompanyResponse
    {
        //Id
        public Guid CarCompanyId { get; set; }
        //Mã nhà xe
        public int CarCompanyCode { get; set; }
        //Tên nhà xe
        public string CarCompanyName { get; set; }
        //Email
        public string Email { get; set; }
        //Hotline
        public string Hotline { get; set; }
        //Số điện thoại
        public string PhoneNumber { get; set; }
        //Địa chỉ văn phòng
        public string OfficeAddress { get; set; }
        //Ảnh đại diện
        public string Image { get; set; }
        //Thumnail
        public string Thumnail { get; set; }
        //Mô tả
        public string Description { get; set; }
        //Đánh giá 
        public decimal? Rate { get; set; }
        //Số lượng đánh giá
        public int RateCount { get; set; }
        //Danh sách social media
        public List<SocialMediaResponse> SocialMediaResponses { get; set; } = new List<SocialMediaResponse>();
        //Danh sách rating
        public List<RatingResponse> RatingList { get; set; } = new List<RatingResponse>();
    }

    public class SocialMediaResponse
    {
        //Mã
        public string SocialMediaCode { get; set; }
        //Tên
        public string SocialMediaName { get; set; }
        //Link
        public string Link { get; set; }
    }

    public class RatingResponse
    {
        public Guid RateId { get; set; }
        public Guid? SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderImage { get; set; }
        public decimal Rate { get; set; }
        public string Content { get; set; }
        public DateTime? CreateTime { get; set; }
    }    
}
