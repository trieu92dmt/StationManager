using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class AppointmentWithPersonInChargeReportViewModel
    {
        [Display(Name = "Mã nhân viên")]
        public string SalesEmployeeCode { get; set; }

        [Display(Name = "NV Kinh doanh")]
        public string SalesEmployeeName { get; set; }

        [Display(Name = "Số lượng")]
        public int QtyAppointment { get; set; }

        [Display(Name = "Tỷ lệ")]
        public string Ratio { get; set; }
    }
    public class AppointmentWithPersonInChargeReportSearchViewModel
    {
        [Display(Name = "Nhân viên kinh doanh")]
        public List<string> SalesEmployeeCode { get; set; }

        //Ngày ghé thăm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CommonDate")]
        public string CommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate", Description = "Appointment_VisitDate")]
        public DateTime? FromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate", Description = "Appointment_VisitDate")]
        public DateTime? ToDate { get; set; }
        public bool IsView { get; set; }
    }
}
