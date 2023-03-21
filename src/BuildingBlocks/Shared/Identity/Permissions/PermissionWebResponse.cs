namespace Shared.Identity.Permissions
{
    public class PermissionWebResponse
    {
        public List<PageResponse> PageModel { get; set; }
        public List<MenuResponse> MenuModel { get; set; }
        public List<PagePermissionResponse> PagePermissionModel { get; set; }
        public List<ModuleResponse> ModuleModel { get; set; }
    }
}
