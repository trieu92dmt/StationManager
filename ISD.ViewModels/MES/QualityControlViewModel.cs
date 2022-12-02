using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ISD.ViewModels
{
    public class QualityControlViewModel : QualityControlModel
    {
        public string DSX { get; set; }


        //Search
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        //Ngày confirm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_ConfirmDate")]
        public string ConfirmCommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? ConfirmFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? ConfirmToDate { get; set; }

        //Ngày QC
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_QualityDate")]
        public string QualityCommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? QualityFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? QualityToDate { get; set; }

        //Khách hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_ProfileCode")]
        public string ProfileCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_ProfileName")]
        public string ProfileName { get; set; }

        //Nhà máy
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Store")]
        public string StoreName { get; set; }
        //Phân xưởng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_WorkShop")]
        public string WorkShopName { get; set; }
        //Công đoạn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_WorkCenter")]
        public string WorkCenterName { get; set; }
        //Nhân viên QC
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_QualityChecker")]
        public string QCSaleEmployee { get; set; }
        //Nhân viên cập nhật cuối cùng
        public string LastEditByName { get; set; }
        //Số lượng yêu cầu
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Qty { get; set; }
        //Đơn vị tính
        public string Unit { get; set; }
        public int? STT { get; set; }
        //Mã màu theo trạng thái phiếu QC
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public Guid? QRCode { get; set; }
 
        public List<HttpPostedFileBase> File { get; set; }
        public List<QualityControlErrorViewModel> ErrorViewModel { get; set; }
        public List<QualityControlInformationMappingViewModel> QualityControlInformationViewModel { get; set; }
        public List<FileAttachmentViewModel> FileViewModel { get; set; }
        public QualityControlDetailViewModel QualityControlDetailViewModel { get; set; }
    }
}
