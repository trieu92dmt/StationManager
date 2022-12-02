using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC16ReportViewModel
    {
        public Guid? BC16Id { get; set; }
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
        public Guid? LSXSAPId { get; set; }
        public DateTime? NgayHTDC { get; set; }
        public string LSXDT { get; set; }
        public string StepCode { get; set; }
        public int? StepIndex { get; set; }
        public decimal? Step_SLKH { get; set; }
        public decimal? Step_SLHT { get; set; }
        public decimal? Step_SLCL { get; set; }
        public decimal? Step_NCTT { get; set; }
        #region Công đoạn
        //SLKH
        public decimal? CD_SLKH01 { get; set; }
        public decimal? CD_SLKH02 { get; set; }
        public decimal? CD_SLKH03 { get; set; }
        public decimal? CD_SLKH04 { get; set; }
        public decimal? CD_SLKH05 { get; set; }
        public decimal? CD_SLKH06 { get; set; }
        public decimal? CD_SLKH07 { get; set; }
        public decimal? CD_SLKH08 { get; set; }
        public decimal? CD_SLKH09 { get; set; }
        public decimal? CD_SLKH10 { get; set; }
        public decimal? CD_SLKH11 { get; set; }
        public decimal? CD_SLKH12 { get; set; }
        public decimal? CD_SLKH13 { get; set; }
        public decimal? CD_SLKH14 { get; set; }
        public decimal? CD_SLKH15 { get; set; }
        public decimal? CD_SLKH16 { get; set; }
        public decimal? CD_SLKH17 { get; set; }
        public decimal? CD_SLKH18 { get; set; }
        public decimal? CD_SLKH19 { get; set; }
        public decimal? CD_SLKH20 { get; set; }
        public decimal? CD_SLKH21 { get; set; }
        public decimal? CD_SLKH22 { get; set; }
        public decimal? CD_SLKH23 { get; set; }
        public decimal? CD_SLKH24 { get; set; }
        public decimal? CD_SLKH25 { get; set; }
        public decimal? CD_SLKH26 { get; set; }
        public decimal? CD_SLKH27 { get; set; }
        public decimal? CD_SLKH28 { get; set; }
        public decimal? CD_SLKH29 { get; set; }
        public decimal? CD_SLKH30 { get; set; }
        public decimal? CD_SLKH31 { get; set; }
        public decimal? CD_SLKH32 { get; set; }
        public decimal? CD_SLKH33 { get; set; }
        public decimal? CD_SLKH34 { get; set; }
        public decimal? CD_SLKH35 { get; set; }
        public decimal? CD_SLKH36 { get; set; }
        public decimal? CD_SLKH37 { get; set; }
        public decimal? CD_SLKH38 { get; set; }
        public decimal? CD_SLKH39 { get; set; }
        public decimal? CD_SLKH40 { get; set; }
        public decimal? CD_SLKH41 { get; set; }
        public decimal? CD_SLKH42 { get; set; }
        public decimal? CD_SLKH43 { get; set; }
        public decimal? CD_SLKH44 { get; set; }
        public decimal? CD_SLKH45 { get; set; }
        public decimal? CD_SLKH46 { get; set; }
        public decimal? CD_SLKH47 { get; set; }
        public decimal? CD_SLKH48 { get; set; }
        public decimal? CD_SLKH49 { get; set; }
        public decimal? CD_SLKH50 { get; set; }
        public decimal? CD_SLKH51 { get; set; }
        public decimal? CD_SLKH52 { get; set; }
        public decimal? CD_SLKH53 { get; set; }
        public decimal? CD_SLKH54 { get; set; }
        public decimal? CD_SLKH55 { get; set; }
        public decimal? CD_SLKH56 { get; set; }
        public decimal? CD_SLKH57 { get; set; }
        public decimal? CD_SLKH58 { get; set; }
        public decimal? CD_SLKH59 { get; set; }
        public decimal? CD_SLKH60 { get; set; }
        //SLHT
        public decimal? CD_SLHT01 { get; set; }
        public decimal? CD_SLHT02 { get; set; }
        public decimal? CD_SLHT03 { get; set; }
        public decimal? CD_SLHT04 { get; set; }
        public decimal? CD_SLHT05 { get; set; }
        public decimal? CD_SLHT06 { get; set; }
        public decimal? CD_SLHT07 { get; set; }
        public decimal? CD_SLHT08 { get; set; }
        public decimal? CD_SLHT09 { get; set; }
        public decimal? CD_SLHT10 { get; set; }
        public decimal? CD_SLHT11 { get; set; }
        public decimal? CD_SLHT12 { get; set; }
        public decimal? CD_SLHT13 { get; set; }
        public decimal? CD_SLHT14 { get; set; }
        public decimal? CD_SLHT15 { get; set; }
        public decimal? CD_SLHT16 { get; set; }
        public decimal? CD_SLHT17 { get; set; }
        public decimal? CD_SLHT18 { get; set; }
        public decimal? CD_SLHT19 { get; set; }
        public decimal? CD_SLHT20 { get; set; }
        public decimal? CD_SLHT21 { get; set; }
        public decimal? CD_SLHT22 { get; set; }
        public decimal? CD_SLHT23 { get; set; }
        public decimal? CD_SLHT24 { get; set; }
        public decimal? CD_SLHT25 { get; set; }
        public decimal? CD_SLHT26 { get; set; }
        public decimal? CD_SLHT27 { get; set; }
        public decimal? CD_SLHT28 { get; set; }
        public decimal? CD_SLHT29 { get; set; }
        public decimal? CD_SLHT30 { get; set; }
        public decimal? CD_SLHT31 { get; set; }
        public decimal? CD_SLHT32 { get; set; }
        public decimal? CD_SLHT33 { get; set; }
        public decimal? CD_SLHT34 { get; set; }
        public decimal? CD_SLHT35 { get; set; }
        public decimal? CD_SLHT36 { get; set; }
        public decimal? CD_SLHT37 { get; set; }
        public decimal? CD_SLHT38 { get; set; }
        public decimal? CD_SLHT39 { get; set; }
        public decimal? CD_SLHT40 { get; set; }
        public decimal? CD_SLHT41 { get; set; }
        public decimal? CD_SLHT42 { get; set; }
        public decimal? CD_SLHT43 { get; set; }
        public decimal? CD_SLHT44 { get; set; }
        public decimal? CD_SLHT45 { get; set; }
        public decimal? CD_SLHT46 { get; set; }
        public decimal? CD_SLHT47 { get; set; }
        public decimal? CD_SLHT48 { get; set; }
        public decimal? CD_SLHT49 { get; set; }
        public decimal? CD_SLHT50 { get; set; }
        public decimal? CD_SLHT51 { get; set; }
        public decimal? CD_SLHT52 { get; set; }
        public decimal? CD_SLHT53 { get; set; }
        public decimal? CD_SLHT54 { get; set; }
        public decimal? CD_SLHT55 { get; set; }
        public decimal? CD_SLHT56 { get; set; }
        public decimal? CD_SLHT57 { get; set; }
        public decimal? CD_SLHT58 { get; set; }
        public decimal? CD_SLHT59 { get; set; }
        public decimal? CD_SLHT60 { get; set; }
        //SLCL
        public decimal? CD_SLCL01 { get; set; }
        public decimal? CD_SLCL02 { get; set; }
        public decimal? CD_SLCL03 { get; set; }
        public decimal? CD_SLCL04 { get; set; }
        public decimal? CD_SLCL05 { get; set; }
        public decimal? CD_SLCL06 { get; set; }
        public decimal? CD_SLCL07 { get; set; }
        public decimal? CD_SLCL08 { get; set; }
        public decimal? CD_SLCL09 { get; set; }
        public decimal? CD_SLCL10 { get; set; }
        public decimal? CD_SLCL11 { get; set; }
        public decimal? CD_SLCL12 { get; set; }
        public decimal? CD_SLCL13 { get; set; }
        public decimal? CD_SLCL14 { get; set; }
        public decimal? CD_SLCL15 { get; set; }
        public decimal? CD_SLCL16 { get; set; }
        public decimal? CD_SLCL17 { get; set; }
        public decimal? CD_SLCL18 { get; set; }
        public decimal? CD_SLCL19 { get; set; }
        public decimal? CD_SLCL20 { get; set; }
        public decimal? CD_SLCL21 { get; set; }
        public decimal? CD_SLCL22 { get; set; }
        public decimal? CD_SLCL23 { get; set; }
        public decimal? CD_SLCL24 { get; set; }
        public decimal? CD_SLCL25 { get; set; }
        public decimal? CD_SLCL26 { get; set; }
        public decimal? CD_SLCL27 { get; set; }
        public decimal? CD_SLCL28 { get; set; }
        public decimal? CD_SLCL29 { get; set; }
        public decimal? CD_SLCL30 { get; set; }
        public decimal? CD_SLCL31 { get; set; }
        public decimal? CD_SLCL32 { get; set; }
        public decimal? CD_SLCL33 { get; set; }
        public decimal? CD_SLCL34 { get; set; }
        public decimal? CD_SLCL35 { get; set; }
        public decimal? CD_SLCL36 { get; set; }
        public decimal? CD_SLCL37 { get; set; }
        public decimal? CD_SLCL38 { get; set; }
        public decimal? CD_SLCL39 { get; set; }
        public decimal? CD_SLCL40 { get; set; }
        public decimal? CD_SLCL41 { get; set; }
        public decimal? CD_SLCL42 { get; set; }
        public decimal? CD_SLCL43 { get; set; }
        public decimal? CD_SLCL44 { get; set; }
        public decimal? CD_SLCL45 { get; set; }
        public decimal? CD_SLCL46 { get; set; }
        public decimal? CD_SLCL47 { get; set; }
        public decimal? CD_SLCL48 { get; set; }
        public decimal? CD_SLCL49 { get; set; }
        public decimal? CD_SLCL50 { get; set; }
        public decimal? CD_SLCL51 { get; set; }
        public decimal? CD_SLCL52 { get; set; }
        public decimal? CD_SLCL53 { get; set; }
        public decimal? CD_SLCL54 { get; set; }
        public decimal? CD_SLCL55 { get; set; }
        public decimal? CD_SLCL56 { get; set; }
        public decimal? CD_SLCL57 { get; set; }
        public decimal? CD_SLCL58 { get; set; }
        public decimal? CD_SLCL59 { get; set; }
        public decimal? CD_SLCL60 { get; set; }
        //NCTT
        public decimal? CD_NCTT01 { get; set; }
        public decimal? CD_NCTT02 { get; set; }
        public decimal? CD_NCTT03 { get; set; }
        public decimal? CD_NCTT04 { get; set; }
        public decimal? CD_NCTT05 { get; set; }
        public decimal? CD_NCTT06 { get; set; }
        public decimal? CD_NCTT07 { get; set; }
        public decimal? CD_NCTT08 { get; set; }
        public decimal? CD_NCTT09 { get; set; }
        public decimal? CD_NCTT10 { get; set; }
        public decimal? CD_NCTT11 { get; set; }
        public decimal? CD_NCTT12 { get; set; }
        public decimal? CD_NCTT13 { get; set; }
        public decimal? CD_NCTT14 { get; set; }
        public decimal? CD_NCTT15 { get; set; }
        public decimal? CD_NCTT16 { get; set; }
        public decimal? CD_NCTT17 { get; set; }
        public decimal? CD_NCTT18 { get; set; }
        public decimal? CD_NCTT19 { get; set; }
        public decimal? CD_NCTT20 { get; set; }
        public decimal? CD_NCTT21 { get; set; }
        public decimal? CD_NCTT22 { get; set; }
        public decimal? CD_NCTT23 { get; set; }
        public decimal? CD_NCTT24 { get; set; }
        public decimal? CD_NCTT25 { get; set; }
        public decimal? CD_NCTT26 { get; set; }
        public decimal? CD_NCTT27 { get; set; }
        public decimal? CD_NCTT28 { get; set; }
        public decimal? CD_NCTT29 { get; set; }
        public decimal? CD_NCTT30 { get; set; }
        public decimal? CD_NCTT31 { get; set; }
        public decimal? CD_NCTT32 { get; set; }
        public decimal? CD_NCTT33 { get; set; }
        public decimal? CD_NCTT34 { get; set; }
        public decimal? CD_NCTT35 { get; set; }
        public decimal? CD_NCTT36 { get; set; }
        public decimal? CD_NCTT37 { get; set; }
        public decimal? CD_NCTT38 { get; set; }
        public decimal? CD_NCTT39 { get; set; }
        public decimal? CD_NCTT40 { get; set; }
        public decimal? CD_NCTT41 { get; set; }
        public decimal? CD_NCTT42 { get; set; }
        public decimal? CD_NCTT43 { get; set; }
        public decimal? CD_NCTT44 { get; set; }
        public decimal? CD_NCTT45 { get; set; }
        public decimal? CD_NCTT46 { get; set; }
        public decimal? CD_NCTT47 { get; set; }
        public decimal? CD_NCTT48 { get; set; }
        public decimal? CD_NCTT49 { get; set; }
        public decimal? CD_NCTT50 { get; set; }
        public decimal? CD_NCTT51 { get; set; }
        public decimal? CD_NCTT52 { get; set; }
        public decimal? CD_NCTT53 { get; set; }
        public decimal? CD_NCTT54 { get; set; }
        public decimal? CD_NCTT55 { get; set; }
        public decimal? CD_NCTT56 { get; set; }
        public decimal? CD_NCTT57 { get; set; }
        public decimal? CD_NCTT58 { get; set; }
        public decimal? CD_NCTT59 { get; set; }
        public decimal? CD_NCTT60 { get; set; }
        #endregion
        public decimal? Total { get; set; }
        public Guid? DSXId { get; set; }
    }
}
