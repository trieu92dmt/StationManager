using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class RequirementReportViewModel
    {
        [Display(Name = "Trạng thái")]
        public string TaskStatusName { get; set; }
        [Display(Name = "Yêu cầu")]
        public string Requirement { get; set; }
        [Display(Name = "Mã khách")]
        public int? ProfileCode { get; set; }
        [Display(Name = "Tên Khách")]
        public string ProfileName { get; set; }
        [Display(Name = "Địa chỉ ")]
        public string Address { get; set; }
        [Display(Name = "SĐT ")]
        public string Phone { get; set; }
        [Display(Name = "Nguồn KH")]
        public string ShowroomCode { get; set; }
        [Display(Name = "Chi Nhánh")]
        public string StoreName { get; set; }
        [Display(Name = "NV Kinh doanh")]
        public string SalesSupervisorName { get; set; }
        [Display(Name = "Thời gian")]
        public DateTime? VisitDate { get; set; }
    }
}
