using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.ViewModels
{
    public class PermissionMobileViewModel
    {
        public List<MobileScreenViewModel> MobileScreenModel { get; set; }

        public List<MenuViewModel> MenuModel { get; set; }

        public List<MobileScreenPermissionViewModel> MobileScreenPermissionModel { get; set; }
    }
}
