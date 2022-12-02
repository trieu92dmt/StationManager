using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class TransferSearchViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Company")]
        public Guid? SearchCompanyId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public Guid? SearchStoreId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Transfer_SearchFromStock")]
        public Guid? SearchFromStockId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Transfer_SearchToStock")]
        public Guid? SearchToStockId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RequestPerson")]
        public string SearchSalesEmployeeCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate", Description = "DocumentDate")]
        public DateTime? SearchFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate", Description = "DocumentDate")]
        public DateTime? SearchToDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Promotion_ProductName")]
        public Guid? SearchProductId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TransferCode")]
        public string SearchTransferCode { get; set; }

        //
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Stock_ProductCode")]
        public string ProductCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ProductName")]
        public string ProductName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public bool? isDelete { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public Guid? SearchDepartmentId { get; set; }
        public bool IsView { get; set; }
    }
}
