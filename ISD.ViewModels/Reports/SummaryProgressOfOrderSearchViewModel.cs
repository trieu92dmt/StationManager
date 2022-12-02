using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
   public class SummaryProgressOfOrderSearchViewModel
    {
        [Display(Name = "Lệnh Sản Xuất")]
        public string LSX { set; get; }

        [Display(Name = "Ngày Hoàn Thành Theo Kế Hoạch")]
        public DateTime NgayHT { set; get; }

        [Display(Name = "Từ Ngày ")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "Đến Ngày ")]
        public DateTime? ToDate { get; set; }

        public bool IsView { set; get; }
    }
}
