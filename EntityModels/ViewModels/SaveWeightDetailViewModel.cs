﻿using System.ComponentModel.DataAnnotations;

namespace EntityModels.ViewModels
{
    public class SaveWeightDetailViewModel
    {
        [Required(ErrorMessage = "Thời gian cân không được để trống")]
        public DateTime WeightTime { get; set; }

        [Required(ErrorMessage = "Trọng lượng cân không được để trống")]
        public double WeightPerWeighing { get; set; }

        [Required(ErrorMessage = "Mã đầu cân không được để trống")]
        public string WeightStationCode { get; set; }
    }
}
