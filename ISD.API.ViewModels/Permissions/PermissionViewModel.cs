using System.Collections.Generic;

namespace ISD.API.ViewModels
{
    public class PermissionViewModel
    {
        public List<PageViewModel> PageModel { get; set; }
        public List<MenuViewModel> MenuModel { get; set; }
        public List<PagePermissionViewModel> PagePermissionModel { get; set; }
        public List<ModuleViewModel> ModuleModel { get; set; }
    }
}
