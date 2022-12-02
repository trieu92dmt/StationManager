using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
   public class ReportProvideMaterialSearchViewModel
    {
        [Display(Name = "Nhà Máy")]
        public string NhaMay { set; get; }

        [Display(Name = "Lệnh Sản Xuất ĐT")]
        public string LSXDT { set; get; }

        [Display(Name = "Công Đoạn")]
        public string CongDoan { set; get; }

        [Display(Name = "TrangThai")]
        public string TrangThai { set; get; }

        [Display(Name = "Từ Ngày ")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "Đến Ngày ")]
        public DateTime? ToDate { get; set; }

        public bool IsView { set; get; }
    }
}
