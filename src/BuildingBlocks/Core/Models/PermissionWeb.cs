using Core.Models.Permissions;

namespace Core.Models
{
    public class PermissionWeb
    {
        public List<PageResponse> PageModel { get; set; }
        public List<Menu> MenuModel { get; set; }
        public List<PagePermissionResponse> PagePermissionModel { get; set; }
        public List<ModuleResponse> ModuleModel { get; set; }
    }
}
