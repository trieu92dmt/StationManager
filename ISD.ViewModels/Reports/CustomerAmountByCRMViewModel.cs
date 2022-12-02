using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class CustomerAmountByCRMViewModel
    {
        [Display(Name = "Phân loại KH")]
        public string CustomerTypeName { get; set; }
        [Display(Name = "Nhóm khách hàng")]
        public string CustomerGroupName { get; set; }
        [Display(Name = "Khách ECC")]
        public int? QtyECC { get; set; }
        [Display(Name = "Khách CRM")]
        public int? QtyCRM { get; set; }
        [Display(Name = "Tổng")]
        public int? Total { get; set; }

    }

    public class CustomerAmountByCRMSearchViewModel
    {
        [Display(Name = "Phân loại KH")]
        public string CustomerTypeCode { get; set; }
        [Display(Name = "Nhóm khách hàng")]
        public string CustomerGroupCode { get; set; }
        //Ngày tạo
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CommonCreateDate")]
        public string CommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate", Description = "CreateTime")]
        public DateTime? FromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate", Description = "CreateTime")]
        public DateTime? ToDate { get; set; }
        public bool IsView { get; set; }
    }
}
