using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC18_2ReportViewModel
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
        //Mã chi tiết
        public string ITMNO { get; set; }
        //Tên chi tiết
        public string KTEXT { get; set; }   
        //Công đoạn lớn
        public string ARBPL { get; set; }

        //IDNRK : Mã nguyên liệu
        public string IDNRK_MES { get; set; }
        //Tên nguyên liệu
        public string MAKTX { get; set; }
        //Đơn vị tính
        public string BMEIN { get; set; }

        //Số lượng chi tiêt
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? BMSCH { get; set; }

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

        //SL CT KH
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SLCTKH { get; set; }

        //Khối lượng tinh KH
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? MENGE { get; set; }

      

        //Số lượng hoàn thành
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? CompletedQuantity { get; set; }
        //Ngày hoàn thành
        public DateTime? CompletedDateTime { get; set; }

        //Đồng bộ
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Sync
        {
            get
            {
                decimal? res = 0;
                if (BMSCH != null && BMSCH != 0 )
                {
                    res = CompletedQuantity / BMSCH;
                }
                return res;
            }
        }
        

        //Số lượng còn thiếu
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? QuantityMissing {
            get
            {
                decimal? res = 0;
                if (SLCTKH != 0 && SLCTKH != null)
                {
                    res = SLCTKH - CompletedQuantity;
                }

                return res;
            }
        }

        //% hoàn thành
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? PercentCompleted
        {
            get
            {
                decimal? res = 0;
                //if (MENGE != 0 && MENGE != null)
                //{
                //    if (SLCTKH != null && SLCTKH != 0)
                //    {
                //        res = CompletedQuantity * 100 / SLCTKH;
                //    }
                //}
                if (SLCTKH != null && SLCTKH != 0)
                {
                    res = CompletedQuantity * 100 / SLCTKH;
                }

                return res;
            }
        }

        //Khối lượng hoàn thành
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? CompletedVolume {
            get
            {
                decimal? res = PercentCompleted *(MENGE/100);

                return res;
            }
        }




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
        public string WorkShop { get; set; }
        public string WorkCenter { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductAttributes { get; set; }
        public int? SLKH { get; set; }
        public string StockRecevingType { get; set; }
        public bool? IsWorkCenterCompleted { get; set; }
        public DateTime? WorkCenterConfirmTime { get; set; }
    }
}
