using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class WorkShopSearchViewModel
    {
        public string WorkShopName { get; set; }
        public string WorkShopCode { get; set; }
        public Guid? CompanyId { get; set; }
        public bool? Actived { get; set; }
    }
}
