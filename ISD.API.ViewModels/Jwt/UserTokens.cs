using System;
using System.Globalization;

namespace ISD.API.ViewModels
{
    public class UserTokens
    {
        public Guid AccountId { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public TimeSpan Validaty { get; set; }
        public string RefreshToken { get; set; }
        public Guid CompanyId { get; set; }
        public string EmailId { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string Roles { get; set; }
        public DateTime ExpiredTime { get; set; }
        public PermissionMobileViewModel Permission { get; set; }
        public PermissionViewModel WebPermission { get; set; }
    }
}
