using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC18_1ReportViewModel
    {
        //STT
        public long OrderIndex { get; set; }
        //LSX ĐT
        public string LSXDT { get; set; }
        //Đợt sản xuất
        public string DSX { get; set; }
        //LSX SAP
        public string LSXSAP { get; set; }
        //Mã sản phẩm
        public string ERPProductCode { get; set; }
        //Tên sản phẩm
        public string ProductName { get; set; }
        //SL KH
        public int? SLKH { get; set; }
        //Số lượng hoàn thành
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? CompletedQuantity { get; set; }
        //Ngày hoàn thành
        public DateTime? CompletedDateTime { get; set; }

        //Số lượng còn thiếu
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? RemainingQuantity { get; set; }

        //% hoàn thành
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? PercentCompleted { get; set; }

        #region Nhà máy        
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public string StoreId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_SaleOrgCode")]
        public string SaleOrgCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Store_StoreCode")]
        public string StoreCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Store_StoreName")]
        public string StoreName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string WorkCenterCode { get; set; }
        #endregion

        #region Phân xưởng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public Guid? WorkShopId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkShopCode")]
        public string WorkshopCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkshopName")]
        public string WorkshopName { get; set; }
        #endregion

        public bool IsView { get; set; }

        #region Ngày hoàn thành
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedFromDate")]
        public DateTime? CompletedFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedToDate")]
        public DateTime? CompletedToDate { get; set; }

        public string CreatedCommonDate { get; set; }
        #endregion
        public string WorkCenter { get; set; }
        public decimal? SLSPTon { get; set; }
        public decimal? TuoiTon { get; set; }
        public string ImageUrl { get; set; }
        public string TuoiTonWarning { get; set; }

        //Số thứ tự ưu tiên: lấy từ BC01
        public int? STT { get; set; }
    }
}
