using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Role
{
    public class RoleDetailResponse
    {
        //Id
        public Guid RoleId { get; set; }
        //Mã nhóm
        public string RoleCode { get; set; }
        //Tên nhóm
        public string RoleName { get; set; }
        //Thứ tự
        public int OrderIndex { get; set; }
        //Trạng thái
        public bool Actived { get; set; }
        //Danh sách nhân viên thuộc role
        public List<User> Employees { get; set; } = new List<User>();
    }

    public class User
    {
        //Mã nhân viên
        public string EmployeeCode { get; set; }
        //Tên tài khoản
        public string UserName { get; set; }
        //Họ và tên
        public string FullName { get; set; }
        //Trạng thái
        public bool Actived { get; set; }
    }
}
