using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.EntityModels
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
        public double WeightPerWeighing { get; set; }
        [StringLength(50)]
        public string WeightStationCode { get; set; }
    }
}
