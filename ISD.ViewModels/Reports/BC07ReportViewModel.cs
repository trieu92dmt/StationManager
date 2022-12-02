using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC07ReportViewModel
    {
        public Guid? BC01Id { get; set; }
        public Int64? STT { get; set; }
        //LSX ĐT  + Đợt
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXDT")]
        public string LSX { get; set; }
        [Display(Name = "LSX đại trà")]
        public string LSXDT { get; set; }
        //SO Number
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
        public string VBELN { get; set; }
        //SO Line
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_POSNR")]
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
        [Display(Name = "Phân xưởng")]
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
        public string WorkShopId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkShopCode")]
        public string WorkshopCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkshopName")]
        public string WorkshopName { get; set; }
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
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? DeliveryFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? DeliveryToDate { get; set; }
        [Display(Name = "Đợt sản xuất")]
        public string DSX { get; set; }
        public string WorkShopData { get; set; }
        public decimal? RemainingData { get; set; }
        public DateTime? FinishDateChangeData { get; set; }
        public string PBL01 { get; set; }
        public string PBL02 { get; set; }
        public string PBL03 { get; set; }
        public string PBL04 { get; set; }
        public string PBL05 { get; set; }
        public string PBL06 { get; set; }
        public string PBL07 { get; set; }
        public string PBL08 { get; set; }
        public string PBL09 { get; set; }
        public string PBL10 { get; set; }
        public string PBL11 { get; set; }
        [Display(Name = "Trạng thái LSX SAP")]
        public bool? isOpen { get; set; }
        [Display(Name = "Khách hàng")]
        public string Customer { get; set; }
        public string ProductName { get; set; }
        public string PONoiBo { get; set; }
        public string SoPOKhach { get; set; }
        public string MaSKU { get; set; }
        public string WBS { get; set; }
        public string TinhTrangMT { get; set; }
        public string NguyenLieu { get; set; }
        public string HoanThien { get; set; }
        public string HinhAnh { get; set; }
        public string DVT { get; set; }
        public decimal? SLDaNK { get; set; }
        public decimal? SLChuaNK { get; set; }
        public decimal? SLDaXuat { get; set; }
        public decimal? SLChuaXuat { get; set; }
        public decimal? SLTonKho { get; set; }
        public decimal? SoContKH { get; set; }
        public decimal? SoContDaNK { get; set; }
        public decimal? SoContChuaNK { get; set; }
        public decimal? SoContDaXuat { get; set; }
        public decimal? SoContChuaXuat { get; set; }
        public decimal? SoContTonKho { get; set; }
        public DateTime? NgayKTDKTheoLSXSAP { get; set; }
        public DateTime? NgayKTDKTheoDSX { get; set; }
        public DateTime? NgayKTDCTheoDSX { get; set; }
        public decimal? SLSP { get; set; }
        public decimal? CBM { get; set; }
        public decimal? TongCBMChuaXuat { get; set; }
        public string CangDen { get; set; }
        public string LoaiCont { get; set; }
        public string NgayTauChay { get; set; }
        public string StartSWDate { get; set; }
        public string EndSWDate { get; set; }
        public string AustinDirect { get; set; }
        public string TuanXuatHang { get; set; }
        public string ThangXuatHang { get; set; }
        public decimal? CompletedPercentGo { get; set; }
        public decimal? CompletedPercentVan { get; set; }
        public decimal? CompletedPercentGiaCong { get; set; }
        public decimal? CompletedPercentVatTu { get; set; }
        public decimal? CompletedPercentHoaChat { get; set; }
        public decimal? CompletedPercentBBPL { get; set; }
        public decimal? CompletedPercentBangMau { get; set; }
        public decimal? KLTGo { get; set; }
        public DateTime? NgayYeuCauXuatHangSO { get; set; }

        #region Thông tin phân xưởng phôi
        public decimal? PXP_SoContKH { get; set; }
        public decimal? PXP_SoContHT { get; set; }
        public decimal? PXP_SoContConLai { get; set; }
        public DateTime? PXP_NgayYCHT { get; set; }
        public decimal? PXP_TinhTrang { get; set; }
        #endregion

        #region Thông tin phân xưởng mẫu
        public decimal? PXM_SoContKH { get; set; }
        public decimal? PXM_SoContHT { get; set; }
        public decimal? PXM_SoContConLai { get; set; }
        public DateTime? PXM_NgayYCHT { get; set; }
        public decimal? PXM_TinhTrang { get; set; }
        #endregion

        #region Thông tin phân xưởng 4
        public decimal? PX4_SoContKH { get; set; }
        public decimal? PX4_SoContHT { get; set; }
        public decimal? PX4_SoContConLai { get; set; }
        public DateTime? PX4_NgayYCHT { get; set; }
        public decimal? PX4_TinhTrang { get; set; }
        #endregion

        #region Thông tin phân xưởng 5
        public decimal? PX5_SoContKH { get; set; }
        public decimal? PX5_SoContHT { get; set; }
        public decimal? PX5_SoContConLai { get; set; }
        public DateTime? PX5_NgayYCHT { get; set; }
        public decimal? PX5_TinhTrang { get; set; }
        #endregion

        #region Thông tin phân xưởng 3
        public decimal? PX3_SoContKH { get; set; }
        public decimal? PX3_SoContHT { get; set; }
        public decimal? PX3_SoContConLai { get; set; }
        public DateTime? PX3_NgayYCHT { get; set; }
        public decimal? PX3_TinhTrang { get; set; }
        #endregion

        #region Thông tin phân xưởng 6
        public decimal? PX6_SoContKH { get; set; }
        public decimal? PX6_SoContHT { get; set; }
        public decimal? PX6_SoContConLai { get; set; }
        public DateTime? PX6_NgayYCHT { get; set; }
        public decimal? PX6_TinhTrang { get; set; }
        #endregion

        #region Thông tin phân xưởng Cửa
        public decimal? PXC_SoContKH { get; set; }
        public decimal? PXC_SoContHT { get; set; }
        public decimal? PXC_SoContConLai { get; set; }
        public DateTime? PXC_NgayYCHT { get; set; }
        public decimal? PXC_TinhTrang { get; set; }
        #endregion

        #region Thông tin phân xưởng 1
        public decimal? PX1_SoContKH { get; set; }
        public decimal? PX1_SoContHT { get; set; }
        public decimal? PX1_SoContConLai { get; set; }
        public DateTime? PX1_NgayYCHT { get; set; }
        public decimal? PX1_TinhTrang { get; set; }
        #endregion
        public DateTime? CreateTime { get; set; }
    }
}
