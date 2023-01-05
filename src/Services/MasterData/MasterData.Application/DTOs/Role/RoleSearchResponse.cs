using ISD.Core.Properties;

namespace MasterData.Application.DTOs
{
    public class RoleSearchResponse
    {
        //STT
        public int STT { get; set; }
        //Id
        public Guid RolesId { get; set; }
        //Mã nhóm
        public string RolesCode { get; set; }
        //Tên nhóm
        public string RolesName { get; set; }
        //Phân nhóm
        public bool IsEmployeeGroup { get; set; }
        public string EmployeeGroup => IsEmployeeGroup == true ? LanguageResource.DepartmentId : LanguageResource.Actions;
        //Thứ tự
        public int OrderIndex { get; set; }
        //Trạng thái
        public bool Actived { get; set; }
    }
}