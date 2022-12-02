using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class StockAllocationReportViewModel
    {
        [Display(Name = "Tên công ty")]
        public string CompanyName { get; set; }
        [Display(Name = "Tên chi nhánh")]
        public string StoreName { get; set; }
        [Display(Name = "Nhóm vật tư")]
        public string CategoryName { get; set; }
        [Display(Name = "Số lượng đã nhập")]
        public decimal ReceiveQuantity { get; set; }
        [Display(Name = "Số lượng nhập dự kiến")]
        public decimal ExpectedQuantity { get; set; }
        [Display(Name = "Tỉ lệ")]
        public string Ratio { get; set; }
        
    }
}
