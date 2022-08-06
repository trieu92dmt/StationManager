namespace WEIGHT.EntityModels.Responses
{
    public class ResourceMonitorResponse
    {
        public string WeighingStationCode { get; set; }
        public DateTime CheckingTime { get; set; }
        public double WeightNumberDisplayed { get; set; }
    }
}
