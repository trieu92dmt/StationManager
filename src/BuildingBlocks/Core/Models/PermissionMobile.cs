namespace Core.Models
{
    public class PermissionMobile
    {
        public List<MobileScreen> MobileScreens { get; set; } = new List<MobileScreen>();

        public List<Menu> Menus { get; set; } = new List<Menu>();

        public List<MobileScreenPermission> MobileScreenPermissions { get; set; } = new List<MobileScreenPermission>(); 
    }
}
