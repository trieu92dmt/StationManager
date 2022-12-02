using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class UsageQuantityViewModel
    {
        public string ProductAttributes { get; set; }
        public string ProductName { get; set; }
        public string ITMNO { get; set; }
        public string KTEXT { get; set; }
        public string POT12 { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? BMSCHDC { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? BMSCH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Quantity { get; set; }
        public List<UsageQuantityViewModel> listITMNO { get; set; }
    }


}