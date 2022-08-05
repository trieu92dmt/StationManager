using System.ComponentModel.DataAnnotations;

namespace EntityModels.ViewModels
{
    public class SaveResourceMonitorModel
    {
        [Required(ErrorMessage = "Mã đầu cân không được để trống")]
        public string WeighingStationCode { get; set; }

        [Required(ErrorMessage = "Thời gian kiểm tra không được để trống")]
        public DateTime CheckingTime { get; set; }

        [Required(ErrorMessage = "Trọng lượng hiển thị ở đầu cân không được để trống")]
        public double WeightNumberDisplayed { get; set; }
    }
}
