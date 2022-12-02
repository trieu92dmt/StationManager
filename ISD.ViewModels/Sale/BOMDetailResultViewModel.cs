using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BOMDetailResultViewModel
    {
        public int STT { get; set; }
        [Display(Name = "Material Number")]
        public string MATNR { get; set; }
        [Display(Name = "Decscription")]
        public string MAKTX { get; set; }
        [Display(Name = "Plant")]
        public string WERKS { get; set; }
        [Display(Name = "Quantity")]
        public Nullable<decimal> MENGE { get; set; }
        [Display(Name = "Unit")]
        public string MEINS { get; set; }





    }
}
