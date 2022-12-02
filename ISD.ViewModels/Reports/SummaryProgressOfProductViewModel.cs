using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
   public class SummaryProgressOfProductsViewModel
    {
        
        [Display(Name = "Lô Sản Xuất SAP")]
        public string LSXSAP { get; set; }

        [Display(Name = "Lô Sản Xuất")]
        public string LSX { get; set; }

        [Display(Name = "Mã Sản Phẩm")]
        public string MaSP { get; set; }

        [Display(Name = "Sản Phẩm")]
        public string SP { get; set; }

        [Display(Name = "WBS/SO")]
        public string WBS_SO { get; set; }

        [Display(Name = "Số PO")]
        public string SOPO { get; set; }

        [Display(Name = "Số Lượng Khách Hàng")]
        public int SLKH { get; set; }
        [Display(Name = "Loại Nhiên Liệu")]
        public string LoaiNL{ get; set; }
        [Display(Name = "Khối lượng tinh KH")]
        public string KLTinhKH { get; set; }

        [Display(Name = "KL tinh GỖ KH")]
        public string KLTinhGoKH { get; set; }

        [Display(Name = "KL tinh VÁN KH")]
        public string KLTinhVanKH { get; set; }
        
    }
}
