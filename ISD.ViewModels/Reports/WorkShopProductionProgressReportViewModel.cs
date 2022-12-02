using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
   public class WorkShopProductionProgressReportViewModel
    {
        [Display(Name = "STT")]
        public int? STT { get; set; }

        [Display(Name = "Lô Sản Xuất SAP")]
        public string LSXSAP { get; set; }

        [Display(Name = "Lô Sản Xuất")]
        public string LSX { get; set; }

        [Display(Name = "Mã Sản Phẩm")]
        public string MaSP { get; set; }

        [Display(Name = "Tên Sản Phẩm")]
        public string TenSP { get; set; }

        [Display(Name = "Tên Chi Tiết")]
        public string TenCT { get; set; }

        [Display(Name = "Mã NVL Theo SAP")]
        public string MaNVLSAP { get; set; }

        [Display(Name = "Nguyên Liệu")]
        public string NL { get; set; }
        [Display(Name = "Đơn Vị")]
        public string DonVi { get; set; }
        [Display(Name = "Số Lượng Chi Tiết")]
        public string SLCT { get; set; }

        [Display(Name = "Dày")]
        public string Day { get; set; }

        [Display(Name = "Rộng")]
        public string Rong { get; set; }

        [Display(Name = "Dài")]
        public string Dai { get; set; }

        [Display(Name = "SLChi Tiết Khách Hàng")]
        public string SLCTKH { get; set; }

        [Display(Name = "Mã Nhóm Nguyên Liệu")]
        public string MaNhomNL { get; set; }

        [Display(Name = "Khối Lượng Tinh KH")]
        public string KLTinhKH { get; set; }


        [Display(Name = "Sơ Chế")]
        public string SoChe { get; set; }

        [Display(Name = "Tinh Chế")]
        public string TinhChe { get; set; }

        [Display(Name = "Rap")]
        public string Rap { get; set; }


        [Display(Name = "Sơ Chế")]
        public string SoCheHT { get; set; }

        [Display(Name = "Tinh Chế")]
        public string TinhCheHT { get; set; }

        [Display(Name = "Ráp")]
        public string RapHT { get; set; }

        [Display(Name = "Hoàn Thiện")]
        public string HThien { get; set; }
    }
}
