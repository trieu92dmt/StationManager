using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class WorkShopProductionProgressReportSearchViewModel
    {
        [Display(Name = "Tên Sản Phẩm")]
        public string ProductName { set; get; }
        [Display(Name = "Mã SAP")]
        public string CodeSAP { set; get; }
        public string Batch { set; get; }
        public string BatchSAP { set; get; }
        [Display(Name = "Từ Ngày ")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "Đến Ngày ")]
        public DateTime? ToDate { get; set; }
        public bool IsView { set; get; }
    }
}
