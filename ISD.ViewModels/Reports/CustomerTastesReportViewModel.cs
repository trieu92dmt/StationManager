using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class CustomerTastesReportViewModel
    {
        [Display(Name = "STT")]
        public int? STT { get; set; }

        [Display(Name = "Mã SAP")]
        public string MaSAP { get; set; }

        [Display(Name = "Mã thương mại")]
        public string MaSP { get; set; }
        [Display(Name = "Tên sản phẩm")]
        public string TenSP { get; set; }
        [Display(Name = "Phân loại vật tư")]
        public string PLoaiVT { get; set; }
        [Display(Name = "Nhóm sản phẩm")]
        public string NhomVT { get; set; }
        [Display(Name = "Số lượt liked")]
        public int? SoLuotLiked { get; set; }
    }
}
