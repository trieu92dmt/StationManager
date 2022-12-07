using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.ViewModels
{
    public class MobileScreenViewModel
    {
        public System.Guid MobileScreenId { get; set; }

        public string ScreenName { get; set; }

        public string ScreenCode { get; set; }

        public System.Guid MenuId { get; set; }

        public string IconType { get; set; }
        public string IconName { get; set; }

        public Nullable<int> OrderIndex { get; set; }

        public List<FunctionViewModel> FunctionViewModels { get; set; }
    }
}
