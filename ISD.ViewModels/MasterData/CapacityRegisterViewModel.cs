using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class CapacityRegisterViewModel : CapacityRegisterModel
    {
        [Display(Name = "Phân xưởng")]
        public string WorkShopName { get; set; }
    }
}
