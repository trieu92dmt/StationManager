namespace Shared.Identity.Permissions
{
    public class PermissionMobileResponse
    {
        public List<MobileScreenResponse> MobileScreens { get; set; } = new List<MobileScreenResponse>();

        public List<MenuResponse> Menus { get; set; } = new List<MenuResponse>();

        public List<MobileScreenPermissionResponse> MobileScreenPermissions { get; set; } = new List<MobileScreenPermissionResponse>();
    }
}
