using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC11ReportViewModel
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
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string WorkCenterCode { get; set; }
        public string WorkCenterName { get; set; }
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

        public decimal? Difference
        {
            get
            {
                //Chênh lệch = SL chi tiết KH - SL chi tiết hoàn thành
                if (PlanQuantity.HasValue && Quantity.HasValue)
                {
                    return PlanQuantity - Quantity;
                }
                return null;
            }
        }
        public decimal? SoPhutRT { get; set; }
    }
}
