using Shared.Identity.Permissions;

namespace Shared.Identity
{
    public class TokenResponse
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
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string PlantCode { get; set; }
        public string PlantName { get; set; }
        public string SaleOrgCode { get; set; }
        public string SaleOrgName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public DateTime ExpiredTime { get; set; }
        public PermissionMobileResponse Permission { get; set; } = new PermissionMobileResponse();
        public PermissionWebResponse WebPermission { get; set; } = new PermissionWebResponse();
    }
}
