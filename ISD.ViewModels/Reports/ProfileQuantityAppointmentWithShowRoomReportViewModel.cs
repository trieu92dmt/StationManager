using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ProfileQuantityAppointmentWithShowRoomReportViewModel
    {
        [Display(Name = "Nguồn KH")]
        public string ShowroomName { get; set; }

        [Display(Name = "Chi nhánh")]
        public string StoreName { get; set; }

        [Display(Name = "Số lượng")]
        public int ProfileCount { get; set; }

        [Display(Name = "Tỷ lệ")]
        public string Ratio { get; set; }
    }

    public class ProfileQuantityAppointmentWithShowRoomReportSearchViewModel
    {

        //Showroom
        [Display(Name = "Chi nhánh")]
        public List<string> CreateAtSaleOrg { get; set; }

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
