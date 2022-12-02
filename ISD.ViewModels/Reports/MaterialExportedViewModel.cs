using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class MaterialExportedViewModel
    {
        public string SaleOfficeCode { get; set; }
        [Display(Name = "Khu vực")]
        public string AreaName { get; set; }
        [Display(Name = "Phòng ban")]
        public string DepartmentName { get; set; }
        [Display(Name = "Số GVL")]
        public int? QtyGVL { get; set; }
        [Display(Name = "Kệ CTL")]
        public decimal? QtyKeCTL { get; set; }
        [Display(Name = "Kệ Mẫu")]
        public decimal? QtyKeMau { get; set; }
        [Display(Name = "Bas Inox")]
        public decimal? QtyBasInox { get; set; }
        [Display(Name = "Bảng Hiệu")]
        public decimal? QtyBangHieu { get; set; }
        [Display(Name = "Khay A5")]
        public decimal? QtyKhayA5 { get; set; }
        public Guid? ProfileId { get; set; }
        

    }
    public class MaterialExportedSearchModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CommonReceiveDate")]
        public string CommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate", Description = "Appointment_VisitDate")]
        public DateTime? FromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate", Description = "Appointment_VisitDate")]
        public DateTime? ToDate { get; set; }
        public bool IsView { get; set; }
    }
}
