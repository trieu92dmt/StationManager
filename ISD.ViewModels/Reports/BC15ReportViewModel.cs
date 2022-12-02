using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC15ReportViewModel
    {
        public Guid? BC15Id { get; set; }
        public bool IsView { get; set; }
        public int? TopRow { get; set; }
        public int? FreezeColumn { get; set; }
        public string Plant { get; set; }
        //LSX ĐT  + Đợt
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXDT")]
        public string LSX { get; set; }
        [Display(Name = "Đợt sản xuất")]
        public string DSX { get; set; }
        //SO Number
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
        public string VBELN { get; set; }
        //SO Line
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_POSNR")]
        public string POSNR { get; set; }
        //PP Order
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXC")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string LSXSAP { get; set; }
        #region Ngày hoàn thành
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedFromDate")]
        public DateTime? CompletedFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedToDate")]
        public DateTime? CompletedToDate { get; set; }

        public string CreatedCommonDate { get; set; }
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

        #region Công đoạn lớn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string WorkCenterCode { get; set; }
        #endregion

        //Phân xưởng vật lý
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_PhysicsWorkShop")]
        public string PhysicsWorkShopCode { get; set; }
        //Phân xưởng
        public Guid? WorkShopId { get; set; }
        public string WorkShopName { get; set; }
        //Mã SP
        public string ProductCode { get; set; }
        //Tên SP
        public string ProductName { get; set; }
        //STT(Mã chi tiết)
        public string ProductAttributes { get; set; }
        //Tên chi tiết
        public string ProductAttributesName { get; set; }
        //Mã nguyên liệu
        public string MaterialCode { get; set; }
        //Tên nguyên liệu
        public string MaterialName { get; set; }
        //ĐVT
        public string DVT { get; set; }
        //SLCTKH
        public decimal? SLCTKH { get; set; }
        //Quy cách tinh(mm)
        public decimal? P1 { get; set; }
        public decimal? P2 { get; set; }
        public decimal? P3 { get; set; }
        //Thể tích
        public decimal? TheTich { get; set; }
        //Khối lượng tinh gỗ
        public decimal? KLTGo { get; set; }
        //Khối lượng tinh ván
        public decimal? KLTVan { get; set; }
        //SLKH
        public decimal? SLKH { get; set; }
        //SLCTKH theo sản phẩm
        public decimal? SLCTKHTheoSP { get; set; }

        #region Thông tin phân xưởng phôi
        public decimal? PXP_SLHT_SC { get; set; }
        public decimal? PXP_SLCL_SC { get; set; }
        public decimal? PXP_SoPhutNCTT_SC { get; set; }
        public decimal? PXP_SoContKH_SC { get; set; }
        public decimal? PXP_SoContConLai_SC { get; set; }
        public DateTime? PXP_NgayYCHT_SC { get; set; }
        public string PXP_TinhTrang_SC { get; set; }
        #endregion

        #region Thông tin phân xưởng 3
        public decimal? PX3_SLHT_TC { get; set; }
        public decimal? PX3_SLCL_TC { get; set; }
        public decimal? PX3_SLHT_LR { get; set; }
        public decimal? PX3_SLCL_LR { get; set; }
        public decimal? PX3_SoPhutNCTT_LR { get; set; }
        public decimal? PX3_SoContKH_LR { get; set; }
        public decimal? PX3_SoContConLai_LR { get; set; }
        public DateTime? PX3_NgayYCHT_LR { get; set; }
        public string PX3_TinhTrang_LR { get; set; } 
        #endregion

        #region Thông tin phân xưởng 6
        public decimal? PX6_SLHT_HT { get; set; }
        public decimal? PX6_SLCL_HT { get; set; }
        public decimal? PX6_SoPhutNCTT_HT { get; set; }
        public decimal? PX6_SoContKH_HT { get; set; }
        public decimal? PX6_SoContConLai_HT { get; set; }
        public DateTime? PX6_NgayYCHT_HT { get; set; }
        public string PX6_TinhTrang_HT { get; set; }
        #endregion

        #region Thông tin phân xưởng 4
        public decimal? PX4_SLHT_TC { get; set; }
        public decimal? PX4_SLCL_TC { get; set; }
        public decimal? PX4_SLHT_LR { get; set; }
        public decimal? PX4_SLCL_LR { get; set; }
        public decimal? PX4_SoPhutNCTT_LR { get; set; }
        public decimal? PX4_SoContKH_LR { get; set; }
        public decimal? PX4_SoContConLai_LR { get; set; }
        public DateTime? PX4_NgayYCHT_LR { get; set; }
        public string PX4_TinhTrang_LR { get; set; }
        #endregion

        #region Thông tin phân xưởng 5
        public decimal? PX5_SLHT_HT { get; set; }
        public decimal? PX5_SLCL_HT { get; set; }
        public decimal? PX5_SoPhutNCTT_HT { get; set; }
        public decimal? PX5_SoContKH_HT { get; set; }
        public decimal? PX5_SoContConLai_HT { get; set; }
        public DateTime? PX5_NgayYCHT_HT { get; set; }
        public string PX5_TinhTrang_HT { get; set; }
        #endregion

        #region Thông tin phân xưởng Cửa
        public decimal? PXCua_SLHT_TC { get; set; }
        public decimal? PXCua_SLCL_TC { get; set; }
        public decimal? PXCua_SLHT_LR { get; set; }
        public decimal? PXCua_SLCL_LR { get; set; }
        public decimal? PXCua_SLHT_HT { get; set; }
        public decimal? PXCua_SLCL_HT { get; set; }
        public decimal? PXCua_SoPhutNCTT_HT { get; set; }
        public decimal? PXCua_SoContKH_HT { get; set; }
        public decimal? PXCua_SoContConLai_HT { get; set; }
        public DateTime? PXCua_NgayYCHT_HT { get; set; }
        public string PXCua_TinhTrang_HT { get; set; }
        #endregion

        #region Thông tin phân xưởng Mẫu
        public decimal? PXMau_SLHT_SC { get; set; }
        public decimal? PXMau_SLCL_SC { get; set; }
        public decimal? PXMau_SLHT_TC { get; set; }
        public decimal? PXMau_SLCL_TC { get; set; }
        public decimal? PXMau_SLHT_LR { get; set; }
        public decimal? PXMau_SLCL_LR { get; set; }
        public decimal? PXMau_SLHT_HT { get; set; }
        public decimal? PXMau_SLCL_HT { get; set; }
        public decimal? PXMau_SoPhutNCTT_HT { get; set; }
        public decimal? PXMau_SoContKH_HT { get; set; }
        public decimal? PXMau_SoContConLai_HT { get; set; }
        public DateTime? PXMau_NgayYCHT_HT { get; set; }
        public string PXMau_TinhTrang_HT { get; set; }
        #endregion
        public Guid? LSXSAPId { get; set; }
        public DateTime? NgayHTDC { get; set; }
        public string LSXDT { get; set; }
    }
}
