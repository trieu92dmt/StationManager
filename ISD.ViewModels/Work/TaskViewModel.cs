using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ISD.ViewModels
{
    public class TaskViewModel : TaskModel
    {
        public Int64? STT { get; set; }
        public string ProfileCode { get; set; }
        public string ProfileForeignCode { get; set; }
        public string ProfileName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductName")]
        public string ProductName { set; get; }
        public string ProductSOName { set; get; }
        public string ProductSOCode { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ContactAddress")]
        public string ContactAddress { get; set; }
        public string PriorityText_vi { get; set; }
        public string WorkFlowCode { get; set; }
        public string WorkFlowName { get; set; }
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "Trạng thái")]
        //public string TaskStatusName { get; set; }
        public string CompanyName { get; set; }
        public string StoreName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskAssign_SalesEmployeeCode")]
        public string SalesEmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PersonInCharge_RoleCode")]
        public string TaskAssignTypeCode { get; set; }

        public List<TaskAssignViewModel> taskAssignList { get; set; }
        public List<TaskReporterViewModel> taskReporterList { get; set; }
        public List<TaskAssignViewModel> taskAssignGroupList { get; set; }
        public List<TaskAssignViewModel> taskAssignPersonGroupList { get; set; }

        public string ReporterName { get; set; }
        public string DistrictName { get; set; }
        public string ProvinceName { get; set; }
        public string WardName { get; set; }
        public string AssigneeName { get; set; }
        public string WorkFlowImageUrl { get; set; }
        public List<TaskContactViewModel> taskContactList { get; set; }
        public TaskContactViewModel taskContact { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Phone")]
        public string Phone { get; set; }

        public string Email { get; set; }

        //Profile: Sản phẩm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product")]
        public string Text3 { get; set; }

        public TaskProductWarrantyViewModel productWarranty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateByName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditBy")]
        public string LastEditByName { get; set; }

        public Guid? ContactId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Comment")]
        public string Comment { get; set; }

        public Guid? FromStatus { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskStatus")]
        public Guid? ToStatus { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignee")]
        public string Assignee { get; set; }

        public List<TaskCommentViewModel> taskCommentList { get; set; }
        public int? NumberOfComments { get; set; }
        public List<FileAttachmentViewModel> taskFileList { get; set; }
        public int? NumberOfFiles { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_FileUrl")]
        public string CommentFileUrl { get; set; }

        public string ProcessCode { get; set; }
        public int? ProcessCodeIndex { get; set; }
        public string TaskStatusBackgroundColor { get; set; }
        public string TaskStatusColor { get; set; }
        //public Nullable<bool> IsAssignGroup { get; set; }

        //Appointment
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerClass")]
        [RequiredIf("WorkFlowCode", "GT", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string CustomerClassCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category")]
        //[RequiredIf("WorkFlowCode", "GT", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string CategoryCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ShowroomCode")]
        public string ShowroomCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Appointment_SaleEmployeeCode")]
        public string SaleEmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Appointment_ChannelCode")]
        public string ChannelCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Appointment_VisitDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> VisitDate { get; set; }

        //Hitory
        public List<TaskHistoryViewModel> taskHistoryList { get; set; }

        public List<TaskProductViewModel> taskProductList { get; set; }

        public string Type { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_AccErrorTypeCode")]
        public string AccErrorTypeCode { get; set; }
        //Task Processing
        public List<TaskProcessingViewModel> taskProcessingList { get; set; }

        //Thăm hỏi khách hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_VisitAddress")]
        [RequiredIf("WorkFlowCode", "THKH", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public new string VisitAddress { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_VisitTypeCode")]
        [RequiredIf("WorkFlowCode", "THKH", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public new string VisitTypeCode { get; set; }

        //Kanban
        public Guid? KanbanId { get; set; }
        public string id { get; set; }
        public string state { get; set; }

        [AllowHtml]
        public string label { get; set; }

        [AllowHtml]
        public string tags { get; set; }
        public string hex { get; set; }
        public string code { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerSatisfactionCode")]
        public string CustomerSatisfactionCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerSatisfactionReviews")]
        public string CustomerSatisfactionReviews { get; set; }

        //Có ghé thăm cabinet pro
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isVisitCabinetPro")]
        public bool? isVisitCabinetPro { get; set; }

        //Nhu cầu KH khi ghé thăm cabinet pro
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "VisitCabinetProRequest")]
        public string VisitCabinetProRequest { get; set; }

        public string CustomerRatings { get; set; }
        //Đánh giá theo sao
        public string Ticket_CustomerReviews_Product { get; set; }
        public string Ticket_CustomerReviews_Service { get; set; }

        //Mobile additional field
        public string Avatar { get; set; }
        public string ReporterLogoName { get; set; }
        public string PriorityName { get; set; }
        public string CreateByFullName { get; set; }
        public string LastEditByFullName { get; set; }
        public string ReceiveDate_String { get; set; }
        public string StartDate_String { get; set; }
        public string RemindStartDate_String { get; set; }
        public string RemindStartTime_String { get; set; }
        public TimeSpan? RemindStartTime { get; set; }
        public string EstimateEndDate_String { get; set; }
        public string EndDate_String { get; set; }
        public string Date1_String { get; set; }
        public string VisitDate_String { get; set; }
        public int? TaskStatusOrderIndex { get; set; }
        public bool? isCheckInOutComplete { get; set; }
        public string CompanyCode { get; set; }
        //public string Property6WithFormat
        //{
        //    get
        //    {
        //        if (Property6.HasValue)
        //        {
        //            return string.Format("{0:#0}", Property6);
        //        }
        //        return string.Empty;
        //    }
        //}

        public string ReceiveDate_StringDisplay
        {
            get
            {
                if (ReceiveDate.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", ReceiveDate);
                }
                return string.Empty;
            }
        }
        public string ReceiveDate_StringFormat
        {
            get
            {
                if (ReceiveDate.HasValue)
                {
                    return string.Format("{0:yyyy-MM-dd}", ReceiveDate);
                }
                return string.Empty;
            }
        }
        public string StartDate_StringFormat
        {
            get
            {
                if (StartDate.HasValue)
                {
                    return string.Format("{0:yyyy-MM-dd}", StartDate);
                }
                return string.Empty;
            }
        }

        public string EndDate_StringFormat
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return string.Format("{0:yyyy-MM-dd}", EndDate);
                }
                return string.Empty;
            }
        }
        public string StartDate_StringDisplay
        {
            get
            {
                if (StartDate.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", StartDate);
                }
                return string.Empty;
            }
        }
        public string EstimateEndDate_StringDisplay
        {
            get
            {
                if (EstimateEndDate.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", EstimateEndDate);
                }
                return string.Empty;
            }
        }
        public string EndDate_StringDisplay
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", EndDate);
                }
                return string.Empty;
            }
        }
        public string VisitDate_StringDisplay
        {
            get
            {
                if (VisitDate.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", VisitDate);
                }
                return string.Empty;
            }
        }
        public string RemindStartDate_StringDisplay
        {
            get
            {
                if (RemindStartDate.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", RemindStartDate);
                }
                return string.Empty;
            }
        }
        public string RemindStartTime_StringDisplay
        {
            get
            {
                //Trên mobile hiển thị giờ riêng nên phải tách giờ từ RemindStartDate
                if (RemindStartDate.HasValue)
                {
                    return string.Format("{0:HH:mm}", RemindStartDate);
                }
                return string.Empty;
            }
        }
        public string Date1_StringDisplay
        {
            get
            {
                if (Date1.HasValue)
                {
                    return string.Format("{0:dd/MM/yyyy}", Date1);
                }
                return string.Empty;
            }
        }

        //Check in out
        public Guid? CheckInOutId { get; set; }
        public Guid? CheckInBy { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckInBy")]
        public string CheckInByName { get; set; }
        public Guid? CheckOutBy { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckOutBy")]
        public string CheckOutByName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckInLat")]
        public string CheckInLat { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckInLng")]
        public string CheckInLng { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckOutLat")]
        public string CheckOutLat { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckOutLng")]
        public string CheckOutLng { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckInTime")]
        public DateTime? CheckInTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CheckOutTime")]
        public DateTime? CheckOutTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Contact")]
        public string ContactName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Requirement")]
        public string Requirement { get; set; }

        public DateTime? RemindDate { get; set; }

        public List<SubtaskViewModel> subtaskList { get; set; }

        public string SalesSupervisorName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ConstructionUnit")]
        public string ConstructionUnitName { get; set; }

        public string ContactShortName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PersonInCharge_RoleCode")]
        public string RoleName { get; set; }
        public string DepartmentName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskSource")]
        public string TaskSourceCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "HasRequest")]
        public string HasRequest { get; set; }

        public int? TodoSubtask { get; set; }
        public int? ProcessingSubtask { get; set; }
        public int? CompletedSubtask { get; set; }

        public bool? IsChangeAssignGroup { get; set; }

        //Tab names
        public string Tab_FileUrl { get; set; }
        public string Tab_Product { get; set; }
        public string Tab_Catalogue { get; set; }

        [Display(Name = "SLĐC theo đợt")]
        public int? Qty { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SL
        {
            get
            {
                decimal? ret;
                if (Number2.HasValue)
                {
                    ret = Number2.Value;
                }
                else
                {
                    ret = 0;
                }
                return ret;
            }
        }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int? SLKH
        {
            get
            {
                int? ret;
                if (Qty.HasValue)
                {
                    ret = Qty.Value;
                }
                else
                {
                    ret = 0;
                }
                return ret;
            }
        }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductLevelCode")]
        public string ProductLevelCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductColorCode")]
        public string ProductColorCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UsualErrorCode")]
        public string UsualErrorCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Category")]
        public string ProductCategoryCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleOrder_Accessory")]
        public string Accessory { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Warranty_Value")]
        public decimal? WarrantyValue { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PersonInCharge")]
        public string Construction_SalesSupervisorName { get; set; }

        public string Construction_DepartmentName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Contact")]
        public string Construction_ContactName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Phone")]
        public string Construction_Phone { get; set; }
        public string Construction_Email { get; set; }

        public string DetailSummary { get; set; }
        public string TaskStatusCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_TaskSummary")]
        public List<string> TaskSummary { get; set; }

        //Đề xuất của nhân viên tiếp khách
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleEmployeeOffer")]
        public string SaleEmployeeOffer { get; set; }

        //Ý kiến/ phản hồi của khách hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_Reviews")]
        public string Reviews { get; set; }
        public string ShortCustomerReviews { get; set; }
        //Đánh giá
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_Ratings")]
        public string Ratings { get; set; }

        //Nơi tham quan
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_VisitPlace")]
        public string VisitPlace { get; set; }

        //Mã thành phẩm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductCode")]
        public string ProductCode { get; set; }

        //Update LSX Con

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Duration")]
        public string Duration
        {
            get
            {
                if (StartDate.HasValue && EstimateEndDate.HasValue)
                {
                    if (StartDate.Value == EstimateEndDate.Value)
                    {
                        return "1";
                    }
                    else
                    {
                        return Convert.ToString((EstimateEndDate - StartDate).Value.TotalDays + 1) ;
                    }
                }
                return string.Empty;
            }
        }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ActualNumber")]
        public int? ActualNumber { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PercentComplete")]
        public int? PercentComplete
        {
            get
            {
                if (Qty == 0)
                {
                    return 0;
                }
                else
                {
                    return (ActualNumber != null ? ActualNumber : 0) / Qty * 100;

                }
            }
        }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MaterialsRequired")]
        public int? MaterialsRequired { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MaterialsReceived")]
        public int? MaterialsReceived { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_StartDate")]
        public DateTime? StartDate_LSXDT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_EstimatedDate")]
        public DateTime? EstimateEndDate_LSXDT { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Duration")]
        public string Duration_LSXDT
        {
            get
            {
                if (StartDate_LSXDT.HasValue && EstimateEndDate_LSXDT.HasValue)
                {
                    if (StartDate_LSXDT.Value == EstimateEndDate_LSXDT.Value)
                    {
                        return "1";
                    }
                    else
                    {
                        return Convert.ToString((EstimateEndDate_LSXDT - StartDate_LSXDT).Value.TotalDays + 1);
                    }
                }
                return "0";
            }
        }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")]
        public string LSXDT_Summary { get; set; }

        //LSX ĐT
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")]
        public string LSXDT { get; set; } 
        
        //LSX ĐT
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDTSAP_Summary")]
        public string LSXDTSAP { get; set; }

        //Đợt SX
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Summary_DSX")]
        public string DSX { get; set; }

        //LSX SAP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXSAP")]
        public string LSXSAP { get; set; }

        public Guid? StepId { get; set; }
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepCode")]
        //public string StepCode { get; set; }
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepName")]
        //public string StepName { get; set; }
       

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_StartDate")]
        public DateTime? Dot_StartDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_EstimatedDate")]
        public DateTime? Dot_EstimateEndDate { get; set; }
        public string Dot_Duration
        {
            get
            {
                if (Dot_StartDate.HasValue && Dot_EstimateEndDate.HasValue)
                {
                    if (Dot_StartDate.Value == Dot_EstimateEndDate.Value)
                    {
                        return "1";
                    }
                    else
                    {
                        return Convert.ToString((Dot_EstimateEndDate - Dot_StartDate).Value.TotalDays + 1);
                    }
                }
                return string.Empty;
            }
        }
        public string SONumber { get; set; }
        public string SOLineNumber { get; set; }

        public string TaskStatusName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component")]
        public string Component { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_PrintMold")]
        public string PrintMold { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Serial")]
        public string Serial { get; set; }
    }

    public class TaskStatusDropdownList
    {
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
    }

    public class TaskContactViewModel
    {
        public Guid? ProfileId { get; set; }
        public Guid? ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactShortName { get; set; }
        public List<string> AddressList { get; set; }
        public string SalesSupervisorCode { get; set; }
        public string SalesSupervisorName { get; set; }
        public string DepartmentName { get; set; }
        public string ExistProfileAddress { get; set; }

        public string MainContactName { get; set; }
        public string MainContactPhone { get; set; }
        public string MainContactEmail { get; set; }
    }

    public class TaskHistoryViewModel
    {
        public string FieldName { get; set; }
        public string DisplayFieldName { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public Guid? LastEditBy { get; set; }
        public string LastEditByName { get; set; }
        public DateTime? LastEditTime { get; set; }
    }

    public class TaskPopupViewModel
    {
        public Guid? TaskId { get; set; }
        public string Title { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskStatus")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public Guid TaskStatusId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Comment")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string Comment { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_FileUrl")]
        public string FileUrl { get; set; }

    }

    public class SubtaskViewModel
    {
        public System.Guid? TaskId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskCode")]
        public int TaskCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_Summary")]
        public string Summary { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_PriorityCode")]
        public string PriorityCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Type")]
        public System.Guid WorkFlowId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskStatus")]
        public System.Guid TaskStatusId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

        public string ProcessCode { get; set; }

        public string TaskStatusName { get; set; }

        public int? ProcessCodeIndex { get; set; }

        public string WorkFlowCode { get; set; }
        public string WorkFlowName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignee")]
        public string AssigneeName { get; set; }

        public string SubtaskCode { get; set; }
        public string VisitPlace { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? EstimatedEndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ActualStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ActualEstimatedEndDate { get; set; }
    }

    public class ConstructionUnitInfoViewModel
    {
        public string SalesSupervisorCode { get; set; }
        public string SalesSupervisorName { get; set; }

        public List<ConstructionUnitContactViewModel> ContactList { get; set; }
    }

    public class ConstructionUnitContactViewModel
    {
        public Guid? ContactId { get; set; }
        public string ContactName { get; set; }
        public bool? IsMain { get; set; }
    }
}
