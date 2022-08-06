using System.ComponentModel.DataAnnotations;

namespace WEIGHT.EntityModels.ViewModels
{
    public class SaveWeightDetailViewModel
    {
        [Required(ErrorMessage = "Thời gian cân không được để trống")]
        public DateTime WeightTime { get; set; }

        [Required(ErrorMessage = "Trọng lượng cân không được để trống")]
        public decimal WeightPerWeighing { get; set; }

        [Required(ErrorMessage = "Mã đầu cân không được để trống")]
        public string WeightStationCode { get; set; }
    }
}
