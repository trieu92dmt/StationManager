using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WEIGHT.EntityModels.Models
{
    public class ResourceMonitorModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string WeightDetailId { get; set; }
        [StringLength(50)]
        public string WeighingStationCode { get; set; }
        public DateTime CheckingTime { get; set; }
        public double WeightNumberDisplayed { get; set; }
    }
}
