using System.ComponentModel.DataAnnotations;

namespace WEIGHT.EntityModels.Models
{
    public class WeightDetailModel
    {
        [Key]
        public Guid WeightDetailId { get; set; } = new Guid();
        public DateTime WeightTime { get; set; }
        [StringLength(50)]
        public string CustomerName { get; set; }
        [StringLength(50)]
        public string ProductName { get; set; }
        public decimal WeightPerWeighing { get; set; }
        [StringLength(50)]
        public string WeightStationCode { get; set; }

        [StringLength(64)]
        public string RawData { get; set; }
    }
}
