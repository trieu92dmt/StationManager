using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ReportOfExpectedMaterialViewModel
    {
        [Display(Name = "Mã Nhiên Liệu")]
        public string MaNL { set; get; }

        [Display(Name = "MType")]
        public string MType { set; get; }

        [Display(Name = "Tên Nguyên Liệu")]
        public string TenNL { set; get; }

        [Display(Name = "DVT")]
        public string DVT { set; get; }

        [Display(Name = "Số Lượng Yêu Cầu Theo BOM")]
        public int SLYCTheoBom {set;get;}

        [Display(Name = "Số Lượng Sản Xuất Vào Lệnh")]
        public int SLXuatVaoLenh { set; get; }

        [Display(Name = "Số Lượng TF Theo SO")]
        public int  SLTFTheoSO { set;get; }

        [Display(Name = "Số Lượng TF")]
        public int SLTF { set; get; }

        [Display(Name = "Số Lượng Yêu Cầu")]
        public int SLYC { set; get; }

        [Display(Name = "Đang Mua ")]
        public int DangMua { set; get; }

        [Display(Name = "Nhập Kho")]
        public int NhapKho { set; get; }

        [Display(Name = "Cần Thêm")]
        public int CanThem { set; get; }

    }
}
