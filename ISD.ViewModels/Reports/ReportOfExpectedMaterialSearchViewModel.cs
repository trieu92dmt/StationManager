using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
   public class ReportOfExpectedMaterialSearchViewModel
    {
        [Display(Name = "Mã Nhiên Liệu")]
        public string MaNL { set; get; }

        [Display(Name = "MType")]
        public string MType { set; get; }

        [Display(Name = "Tên Nguyên Liệu")]
        public string TenNL { set; get; }

        [Display(Name = "DVT")]
        public string DVT { set; get; }
        public string BatchSAP { set; get; }
        [Display(Name = "Từ Ngày ")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "Đến Ngày ")]
        public DateTime? ToDate { get; set; }
        public bool IsView { set; get; }
    }
}
