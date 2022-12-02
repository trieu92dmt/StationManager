using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC20ReportViewModel
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
        public List<BC20ReportDetailViewModel> ChiTiet { get; set; }
        public List<BC20ReportEmployeeDetailViewModel> NhanVien { get; set; }
    }

    //Kết quả
    public class BC20ReportDetailViewModel
    {
        public DateTime? NgayHienTai { get; set; }
        public string MaTo { get; set; }
        public string TenTo { get; set; }
        public string To { get; set; }
        public decimal? KeHoach { get; set; }
        public decimal? ThucTe { get; set; }
        public decimal? SoPhut { get; set; }
    }

    public class BC20ReportEmployeeDetailViewModel
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
        public decimal? NangSuatDuKienTheoRouting { get; set; }
        public string NangSuatDuKienTheoRoutingDisplay
        {
            get
            {
                if (NangSuatDuKienTheoRouting.HasValue)
                {
                    return string.Format("{0:n2}", NangSuatDuKienTheoRouting.Value);
                }
                return string.Empty;
            }
        }
        public decimal? NangSuatTheoSoNLD { get; set; }
        public string NangSuatTheoSoNLDDisplay
        {
            get
            {
                if (NangSuatTheoSoNLD.HasValue)
                {
                    return string.Format("{0:n2}", NangSuatTheoSoNLD.Value);
                }
                return string.Empty;
            }
        }
        public decimal? NangSuatLuyKeDenHienTai { get; set; }
        public string NangSuatLuyKeDenHienTaiDisplay
        {
            get
            {
                if (NangSuatLuyKeDenHienTai.HasValue)
                {
                    return string.Format("{0:n2}", NangSuatLuyKeDenHienTai.Value);
                }
                return string.Empty;
            }
        }
        public string MaTo { get; set; }
        public string MaPhanXuong { get; set; }
    }
}
