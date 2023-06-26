using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Admin
{
    public class CarCompanyResponse
    {
        //STT
        public int STT { get; set; }
        //Id nhà xe
        public Guid CarCompanyId { get; set; }
        //Mã nhà xe
        public int CarCompanyCode { get; set; }
        //Tên nhà xe
        public string CarCompanyName { get; set; }
        //Username
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
    }
}
