using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class AllotmentResultViewModel
    {
        public string LSXSAP { get; set; }
        public string ProductCode { get; set; }
        public string ProductDetailCode { get; set; }
        public string ProductDetailName { get; set; }
        public decimal? ProductDetailActualQty { get; set; }
    
        public string BOM { get; set; }
        public string BOMCode { get; set; }
        public string BOMUnit { get; set; }
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? BOMQty { get; set; }
        public string StepName { get; set; }
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? ActualQty { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DocumentDate { get; set; }
    }
}
