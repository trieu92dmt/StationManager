﻿namespace WEIGHT.EntityModels.Responses
{
    public class WeightDetailResponse
    {
        public DateTime WeightTime { get; set; }
        //public string CustomerName { get; set; }
        //public string ProductName { get; set; }
        public double WeightPerWeighing { get; set; }
        public string WeightStationCode { get; set; }
    }
}
