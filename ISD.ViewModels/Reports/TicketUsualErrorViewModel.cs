using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class TicketUsualErrorViewModel
    {
        [Display(Name = "Phân cấp SP")]
        public string ProductLevelName { get; set; }

        [Display(Name = "Nhóm vật tư")]
        public string ProductCategoryName { get; set; }

        [Display(Name = "Mã màu")]
        public string ProductColorCode { get; set; }

        [Display(Name = "Các lỗi BH thường gặp")]
        public string UsualErrorName { get; set; }

        [Display(Name = "Số lượng")]
        public int? CountOfTaskProduct { get; set; }
    }
}
