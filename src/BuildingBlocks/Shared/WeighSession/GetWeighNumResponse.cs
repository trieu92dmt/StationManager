namespace Shared.WeighSession
{
    public class GetWeighNumResponse
    {

        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Số lần cân
        public int? WeightQuantity { get; set; }
        //Thời gian bắt đâu
        public DateTime? StartTime { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Is success
        public bool isSuccess { get; set; }
    }
}
