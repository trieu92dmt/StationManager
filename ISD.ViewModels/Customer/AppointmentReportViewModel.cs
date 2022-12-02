using ISD.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class AppointmentReportViewModel
    {
        //public Guid TaskId { get; set; }
        [Display(ResourceType = typeof(LanguageResource), Name = "Appointment_VisitDate")]
        public DateTime VisitDate { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Task_Summary")]
        public string Summary { get; set; }
        public Guid ProfileId { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Customer_CustomerCode")]
        public int ProfileCode { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Customer_CustomerName")]
        public string ProfileName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Profile_Address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Profile_Phone")]
        public string Phone { get; set; }

        public string Email { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Profile_Age")]
        public string AgeText { get; set; }

        [Display(Name = "Loại KH")]
        public string CustomerClassName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Profile_IsForeignCustomer")]
        public string ForeignCustomerName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "ShowroomCode")]
        public string ShowroomName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "StoreId")]
        public string StoreName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Profile_CustomerTypeCode")]
        public string CustomerTypeName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Profile_CustomerCategoryCode")]
        public string CustomerGroupName { get; set; }

        [Display(Name = "Liên hệ chính")]
        public string ContactName { get; set; }

        [Display(Name = "SDT liên hệ")]
        public string ContactPhone { get; set; }

        [Display(Name = "Email liên hệ")]
        public string ContactEmail { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Customer_CustomerTaste")]
        public string customerTasteLst { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Customer_Catalogue")]
        public string customerCatalogueLst { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Appointment_ChannelCode")]
        public string ChannelName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Requirement")]
        public string Requirement { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Note")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "PersonInCharge")]
        public string SalesSupervisorName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Appointment_SaleEmployeeCode")]
        public string SaleEmployeeName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "Profile_Department")]
        public string DepartmentName { get; set; }

        [Display(ResourceType = typeof(LanguageResource), Name = "CreateTime")]
        public DateTime CreateTime { get; set; }


        public string TaskStatusName { get; set; }
    }
}