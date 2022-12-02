using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class SummaryProgressOfOrderViewModel
    {
        [Display(Name = "Lệnh Sản Xuất")]
        public string LSX{set;get;}

        [Display(Name = "Ngày Hoàn Thành Theo Kế Hoạch")]
        public DateTime NgayHT { set; get; }

        [Display(Name = "Khối Lượng Tính Theo Lệnh Sản Xuất")]
        public double KLTLSX { set; get; }

        [Display(Name = "KL Thô Ván Theo LSX")]
        public double KLTVLSX { set; get; }

        [Display(Name = "Ván NL(m3 thô)")]
        public double VANNL_HT { set; get; }

        [Display(Name = "Sơ Chế")]
        public double SOCHE_HT { set; get; }

        [Display(Name = "Ráp")]
        public double RAP_HT { set; get; }

        [Display(Name = "Hoàn Thiện")]
        public double HT_HT { set; get; }

        [Display(Name = "Ván NL(m3 thô)")]
        public double VANNL_T { set; get; }

        [Display(Name = "Sơ Chế")]
        public double SOCHE_T { set; get; }

        [Display(Name = "Ráp")]
        public double RAP_T { set; get; }

        [Display(Name = "Hoàn Thiện")]
        public double HT_T { set; get; }

        [Display(Name = "Ván NL(m3 thô)")]
        public double PT_VANNL { set; get; }

        [Display(Name = "Sơ Chế")]
        public double PT_SOCHE { set; get; }

        [Display(Name = "Ráp")]
        public double PT_RAP { set; get; }

        [Display(Name = "Hoàn Thiện")]
        public double PT_HT { set; get; }

        [Display(Name = "Ghi Chú")]
        public string GhiChu { set; get; }
    }
}
