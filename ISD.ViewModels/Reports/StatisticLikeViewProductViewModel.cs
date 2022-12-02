using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class StatisticLikeViewProductViewModel
    {
        [Display(Name = "Nhóm Sản phẩm")]
        public string NhomVT { get; set; }

        [Display(Name = "Mã SAP")]
        public string MaSAP { get; set; }

        [Display(Name = "Mã thương mại")]
        public string MaSP { get; set; }

        [Display(Name = "Tên sản phầm")]
        public string TenSP { get; set; }

        [Display(Name = "Số lượt Like")]
        public int? SoLuotLiked { get; set; }

        [Display(Name = "Số lượt View")]
        public int? SoLuotViewed { get; set; }
    }

    public class StatisticLikeViewSearchViewModel
    {
        [Display(Name = "Ngày tạo")]
        public string CommonDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? ToDate { get; set; }

        public List<Guid> StoreId { get; set; }
        public bool IsView { get; set; }
    }
}