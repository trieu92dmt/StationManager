using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.Reports
{
   public class ReportProvideMaterialViewModel
    {
        [Display(Name = "Mã Nhiên Liệu")]
        public string MaNL { set; get; }

        [Display(Name = "Tên Nguyên Liệu")]
        public string TenNL { set; get; }

        [Display(Name = "DVT")]
        public string DVT { set; get; }

        [Display(Name = "Số Lượng Yêu Cầu ")]
        public int SLYC { set; get; }

        [Display(Name = "Số Lượng Tiêu Hao")]
        public int SLTH { set; get; }

        [Display(Name = "Tôn Kho Xưởng(TF)")]
        public int TonKhoXuong { set; get; }

        [Display(Name = "Đang Mua ")]
        public int DangMua { set; get; }

        [Display(Name = "Cần Thêm")]
        public int CanThem { set; get; }

    }
}
