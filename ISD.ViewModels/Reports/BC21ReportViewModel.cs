using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC21ReportViewModel
    {
        //Ngày
        [Display(Name = "Ngày")]
        public DateTime? ReportDate { get; set; }
        //Phân xưởng
        [Display(Name = "Phân xưởng")]
        public List<Guid?> WorkShop { get; set; }
        //Tổ
        [Display(Name = "Tổ")]
        public List<string> Department { get; set; }
        public Guid? WorkShopId { get; set; }
        [Display(Name = "Xem theo")]
        public bool? IsViewAtCurrent { get; set; }
        //==========================================
        public List<string> TenDong { get; set; }
        public List<decimal?> SoPhutKeHoach { get; set; }
        public List<decimal?> SoPhutThucTe { get; set; }
        public List<BC21ReportDetailViewModel> ChiTiet { get; set; }
        public List<BC21ReportEmployeeDetailViewModel> NhanVien { get; set; }
    }

    //Kết quả
    public class BC21ReportDetailViewModel
    {
        public DateTime? NgayHienTai { get; set; }
        public string MaTo { get; set; }
        public string TenTo { get; set; }
        public string To { get; set; }
        public decimal? KeHoach { get; set; }
        public decimal? ThucTe { get; set; }
        public decimal? SoPhut { get; set; }
    }

    public class BC21ReportEmployeeDetailViewModel
    {
        public DateTime? ThoiGian { get; set; }
        public string ThoiGianDisplay
        {
            get
            {
                if (ThoiGian.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", ThoiGian.Value);
                }
                return string.Empty;
            }
        }
        public string PhanXuong { get; set; }
        public string To { get; set; }
        public string CongNhan { get; set; }
        //Tiền KH(vnđ)
        public decimal? TienKH { get; set; }
        public string TienKHDisplay
        {
            get
            {
                if (TienKH.HasValue)
                {
                    return string.Format("{0:n0}", TienKH.Value);
                }
                return string.Empty;
            }
        }
        //HSTN
        public decimal? HSTN { get; set; }
        public string HSTNDisplay
        {
            get
            {
                if (HSTN.HasValue)
                {
                    return string.Format("{0:n2}", HSTN.Value);
                }
                return string.Empty;
            }
        }
        //HSHTCV
        public decimal? HSHTCV { get; set; }
        public string HSHTCVDisplay
        {
            get
            {
                if (HSHTCV.HasValue)
                {
                    return string.Format("{0:n2}", HSHTCV.Value);
                }
                return string.Empty;
            }
        }
        //Tiền dự kiến RT(vnđ)
        public decimal? TienDuKienRouting { get; set; }
        public string TienDuKienRoutingDisplay
        {
            get
            {
                if (TienDuKienRouting.HasValue)
                {
                    return string.Format("{0:n0}", TienDuKienRouting.Value);
                }
                return string.Empty;
            }
        }
        //Phần trăm HT
        public decimal? PhanTramHT { get; set; }
        public string PhanTramHTDisplay
        {
            get
            {
                if (PhanTramHT.HasValue)
                {
                    return string.Format("{0:n0} %", PhanTramHT.Value);
                }
                return string.Empty;
            }
        }
        //Tổng lũy kế (vnđ)
        public decimal? TongLuyKe { get; set; }
        public string TongLuyKeDisplay
        {
            get
            {
                if (TongLuyKe.HasValue)
                {
                    return string.Format("{0:n0}", TongLuyKe.Value);
                }
                return string.Empty;
            }
        }
        public string MaTo { get; set; }
        public string MaPhanXuong { get; set; }
    }
}
