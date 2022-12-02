using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ProductionManagementViewModel:ThucThiLenhSanXuatModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepCode")]
        public string StepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TTLSX_StepName")]
        public string StepName { get; set; }
        public string ToStockName { get; set; }
        //Công đoạn hoàn tất
        public Guid? StepId { get; set; }
        //Mã thành phẩm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductCode")]
        public string ProductCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductName")]
        public string ProductName { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXDT")]
        public string ProductionOrder { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXC")]
        public string ProductionOrder_SAP { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_StartDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ProductionOrder_StartDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_EstimatedDate_tooltip")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ProductionOrder_EstimateEndDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Department_DepartmentName")]
        public Guid? DepartmentId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_PlanQty")]
        public int? PlanQty { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_WorkShop")]
        public string WorkShop { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_WorkShop")]
        public string WorkShopId { get; set; }
        public string WorkShopCode { get; set; }
        public string WorkShopName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_WorkDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? WorkDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")] 
        public DateTime? FromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ToDate { get; set; }
        public string LoaiGiaoDich { get; set; }
        public string radio { get; set; }
        public int? SoLuongDat { get; set; }
        public int? SoLuongKhongDat { get; set; }
        public bool? isComplete { get; set; }
        public int? SoLuongChuyen { get; set; }
        public List<ProductionOrderDetailViewModel> productionOrderDetailListView { get; set; }
        public Guid? StockReceivingId { get; set; }

        //LSX ĐT
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")]
        public string PPO_LSXDT { get; set; }

        //LSX SAP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXSAP")]
        public string PPO_LSXSAP { get; set; }

        //Sản phẩm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductCode")]
        public string PPO_ProductCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductName")]
        public string PPO_ProductName { set; get; }


        //Chi tiết
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductAttributes")]
        public string ProductAttributes { set; get; }
        //Tên chi tiết
        public string ProductAttributesName { set; get; }
        //Đơn vị tính của chi tiết
        public string ProductAttributesUnit { set; get; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? ProductAttributesQtyD { set; get; }
        //Quy cách nguyên liệu chính của chi tiết
        public string POT12 { set; get; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Stock_Phase")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public int? Phase { set; get; }
        public bool? IsWorkCenterCompleted { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string ConfirmWorkCenter { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkCenterConfirmTime")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? WorkCenterConfirmTime { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? CreatedFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? CreatedToDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_DLD { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_D { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_DLKD { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_KD { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")]
        public string LSXDT_Summary { get; set; }
        //Search
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public string CreatedCommonDate { get; set; }

        //LSX ĐT
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")]
        public string LSXDT { get; set; }

        //Đợt SX
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Summary_DSX")]
        public string DSX { get; set; }

        //LSX SAP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXSAP")]
        public string LSXSAP { get; set; }

        public string CreateByFullName { get; set; }
        public string LastEditByFullName { get; set; }
        public string TaskStatusName { get; set; }

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
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MaterialsRequired")]
        public int? MaterialsRequired { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MaterialsReceived")]
        public int? MaterialsReceived { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ActualNumber")]
        public int? ActualNumber { get; set; }
        public Guid? CompanyId { get; set; }
        public bool? isHasRouting { get; set; }
    }
}
