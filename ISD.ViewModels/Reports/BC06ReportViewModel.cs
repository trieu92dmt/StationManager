using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC06ReportViewModel
    {
        public Guid? BC01Id { get; set; }
        //LSX ĐT  + Đợt
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXDT")]
        public string LSX { get; set; }
        public string Plant { get; set; }
        public string WorkShop { get; set; }
        public string VBELN { get; set; }
        public string POSNR { get; set; }
        public string LSXSAP { get; set; }

        public bool IsView { get; set; }

        #region Ngày hoàn thành
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedFromDate")]
        public DateTime? CompletedFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedToDate")]
        public DateTime? CompletedToDate { get; set; }

        public string CreatedCommonDate { get; set; }
        #endregion
        public int? TopRow { get; set; }
        public int? FreezeColumn { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? DeliveryFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? DeliveryToDate { get; set; }
        [Display(Name = "Đợt sản xuất")]
        public string DSX { get; set; }

        //Phân xưởng
        public string WorkShopName { get; set; }
        //Capacity
        public decimal? Capacity { get; set; }
        public decimal? CapacityTotal { get; set; }
        public int? Ngay { get; set; }
        public int? Nam { get; set; }
        public int? Thang { get; set; }
        public string ThangText { get; set; }
        public string TuanText { get; set; }
        public decimal? CapacityDetail { get; set; }
        public string Warning { get; set; }
        [Display(Name = "Trạng thái LSX")]
        public bool? isOpen { get; set; }
    }
}
