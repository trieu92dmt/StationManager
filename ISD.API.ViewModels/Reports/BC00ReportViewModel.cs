using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.ViewModels
{
    public class BC00ReportViewModel
    {
        public Guid? BC01Id { get; set; }
        public Guid? DSXId { get; set; }
        public Int64? STT { get; set; }
        //LSX ĐT  + Đợt
        public string LSX { get; set; }
        [Display(Name = "Đợt LSXĐT")]
        public string DSX { get; set; }
        //SO Number
        public string VBELN { get; set; }
        //SO Line
        public string POSNR { get; set; }
        public int? POSNR_Display
        {
            get
            {
                if (!string.IsNullOrEmpty(POSNR))
                {
                    int x = 0;
                    if (int.TryParse(POSNR, out x))
                    {
                        // you know that the parsing attempt
                        // was successful
                        return x;
                    }
                }
                return null;
            }
        }
        //PP Order
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXC")]
        public string LSXSAP { get; set; }
        //Mã sản phẩm
        public string ProductCode { get; set; }
        //Material Type
        public string MType { get; set; }
        //Quantity
        public decimal? Quantity { get; set; }
        // Unit
        public string Unit { get; set; }
        //Volumn
        public decimal? Volumn { get; set; }
        //Số lượng SP/Thùng
        public decimal? QuantityProduct { get; set; }
        //Số lượng NK theo đợt
        public decimal? LSXDNK { get; set; }
        //PYCSXDT
        public string PYCSXDT { get; set; }
        //Nhân sự phụ trách TDSX + XH
        public string AssignResponsibility { get; set; }
        //Nhân sự cấp phát NVL
        public string NSCPNVL { get; set; }

        //SLKH
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SLKH { get; set; }

        //Đã NK
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? NK { get; set; }

        //SỐ mã SP
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? NumberSP { get; set; }

        //SLKH ( cái / Bộ )
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SLKHB { get; set; }

        //Số lượng còn lại
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Remaining { get; set; }
        //Số Cont/Ngày
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? ContDate { get; set; }
        //Ngày yêu cầu giao hàng
        public DateTime? DeliveryDate { get; set; }

        //Kho
        public string Stock { get; set; }
        //Phân xưởng
        public string WorkShop { get; set; }
        //Leadtime
        public decimal? Leadtime { get; set; }
        //SOCreateOn
        public DateTime? SOCreateOn { get; set; }
        //Ngày dự kiến giao hàng
        public DateTime? SchedulelineDeliveryDate { get; set; }

        //Ngày về trên PR
        public DateTime? PRDeliveryDate { get; set; }

        //Ngày bắt đầu dự kiến Sơ Chế
        public DateTime? SchedulelineStartDateSC { get; set; }

        //Ngày bắt TT Sơ Chế
        public DateTime? StartDateSC { get; set; }

        //Ngày bắt đầu dự kiến Tinh Chế
        public DateTime? SchedulelineStartDateTC { get; set; }

        //Ngày bắt TT Tinh Chế
        public DateTime? StartDateTC { get; set; }

        //Ngày bắt đầu dự kiến Lắp Ráp
        public DateTime? SchedulelineStartDateLR { get; set; }

        //Ngày bắt TT Lắp Ráp
        public DateTime? StartDateLR { get; set; }

        //Ngày bắt đầu dự kiến Hoàn Thiện
        public DateTime? SchedulelineStartDateHT { get; set; }

        //Ngày bắt TT Hoàn Thiện
        public DateTime? StartDateHT { get; set; }

        //Bắt đầu
        public DateTime? StartDate { get; set; }

        //Kết thúc
        public DateTime? FinishDate { get; set; }

        //Bắt đầu điều chỉnh
        public DateTime? StartDateChange { get; set; }

        //Kết thúc điều chỉnh
        public DateTime? FinishDateChange { get; set; }

        //Số Ngày trễ
        public decimal? NumberDaysDelay { get; set; }

        //% Hoàn thành
        public decimal? CompletedPercent { get; set; }
        //Tình trạng
        public string Status { get; set; }



        #region Phân xưởng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public Guid? WorkShopId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkShopCode")]
        public string WorkshopCode { get; set; }
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkshopName")]
        //public string WorkshopName { get; set; }
        #endregion
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



        public bool IsView { get; set; }

        #region Ngày hoàn thành
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedFromDate")]
        public DateTime? CompletedFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedToDate")]
        public DateTime? CompletedToDate { get; set; }

        public string CreatedCommonDate { get; set; }
        #endregion

        public DateTime? SchedulelineFinishDateSC { get; set; }
        public DateTime? SchedulelineFinishDateTC { get; set; }
        public DateTime? SchedulelineFinishDateLR { get; set; }
        public DateTime? SchedulelineFinishDateHT { get; set; }
        public string Plant { get; set; }
        public decimal? SLTD { get; set; }
        public decimal? SLCARTON { get; set; }
        public int? WorkCenterIndex { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? LSXSAPId { get; set; }
        public int? StockCode { get; set; }

        public Guid? ErrorDepartment { get; set; }
        public int? TopRow { get; set; }
        public int? FreezeColumn { get; set; }
        public string ErrorDepartmentName { get; set; }
        public string WorkShopName { get; set; }
        public string WBS { get; set; }
        public decimal? SLCL
        {
            get
            {
                //Số lượng còn lại = SLKH Sản phẩm - Số lượng nhập kho lũy kế theo đợt
                if (Quantity.HasValue)
                {
                    decimal? slcl = Quantity;
                    if (LSXDNK.HasValue)
                    {
                        slcl = Quantity - LSXDNK;
                    }
                    return slcl;
                }
                return null;
            }
        }
        public DateTime? SchedulelineDeliveryDateUpdate { get; set; }
        public Guid? ErrorGroup { get; set; }
        public string ErrorGroupName { get; set; }
        public string ErrorDetail { get; set; }
        public decimal? SLYCBOMScrap { get; set; }
        public decimal? PRActualQty { get; set; }
        public decimal? MIGOActualQty { get; set; }
    }

    public class BC00ReportFormViewModel
    {
        public Guid? BC01Id { get; set; }
        public int? STT { get; set; }
        public Guid? ErrorDepartment { get; set; }
        public string Status { get; set; }
        public string LSXSAP { get; set; }
        public int? StockCode { get; set; }
        public decimal? Leadtime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? CompletedPercent { get; set; }
        public string WorkShop { get; set; }
        public DateTime? StartDateChange { get; set; }
        public DateTime? EndDateChange { get; set; }
        public Guid? ErrorGroup { get; set; }
        public string ErrorDetail { get; set; }
    }
}
