using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC12ReportViewModel
    {
        //STT
        public long OderIndex { get; set; }
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
        //Mã chi tiết
        public string ProductAttributes { get; set; }
        //Tên chi tiết
        public string KTEXT { get; set; }   
        //Công đoạn lớn
        public string WorkCenterName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string WorkCenterCode { get; set; }
        //IDNRK : Mã nguyên liệu
        public string IDNRK_MES { get; set; }
        //Tên nguyên liệu
        public string MAKTX { get; set; }
        //Đơn vị tính
        public string BMEIN { get; set; }

        //Quy cách tinh
        //Dày (mm)
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? P1 { get; set; }
        //Rộng (mm)
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? P2 { get; set; }
        //Dài (mm)
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? P3 { get; set; }
        //Công đoạn nhỏ
        public Guid? StockId { get; set; }
        public string StockName { get; set; }
        //Số lượng SP theo đợt
        public decimal? TaskQuantity { get; set; }
        //Số lượng chi tiết theo routing
        public decimal? RoutingQuantity { get; set; }
        //Số lượng kế hoạch
        public decimal? PlanQuantity { get; set; }
        // Số lượng thực tế
        public decimal? Quantity { get; set; }



        #region Tổ
        //Tên tổ
        public string DepartmentName { get; set; }
        //Mã tổ
        public string DepartmentCode { get; set; }
        #endregion
        #region Phân xưởng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public Guid? WorkShopId { get; set; }
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

        #endregion



        public bool IsView { get; set; }

        #region Ngày Ghi nhận
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedFromDate")]
        public DateTime? FromTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedToDate")]
        public DateTime? ToTime { get; set; }

        public string CommonDate { get; set; }
        #endregion

        public string WorkShop { get; set; }
        public decimal? VOLUM { get; set; }
        public decimal? KLTinhHT { get; set; }
        public decimal? Difference { get; set; }
        public decimal? SoPhutNC { get; set; }
        public DateTime? NgayHTKH { get; set; }
        public DateTime? NgayHTTT { get; set; }
    }
}
