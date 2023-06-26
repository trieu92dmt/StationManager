using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Permission
{
    public class AccountResponse
    {
        //Account Id
        public Guid AccountId { get; set; }
        //Tên tài khoản
        public string Username { get; set; }
        //Tên đầy đủ
        public string FullName { get; set; }
        //Trạng thái
        public bool? Active { get; set; }
        //Nhóm chức năng
        public string Role { get; set; }
    }
}
