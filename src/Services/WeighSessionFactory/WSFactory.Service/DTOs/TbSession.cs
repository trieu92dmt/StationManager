using System;

namespace WSFactory.Service.DTOs
{
    public class TbSession
    {
        public int Id { get; set; }
        public int LotNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    public class WeighSessionRefactoryResponse
    {
        public WeighSessionRefactoryResponse(int lotNumber, string scaleCode, decimal? totalWeight, DateTime? startTime, DateTime? endTime, int sessionCheck)
        {
            LotNumber = lotNumber;
            ScaleCode = scaleCode;
            TotalWeight = totalWeight;
            StartTime = startTime;
            EndTime = endTime;
            SessionCheck = sessionCheck;
        }
        public int LotNumber { get; set; }
        public string ScaleCode { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int SessionCheck { get; set; }
    }
    public class Line1And2Response : TbSession
    {
        public float V_Material { get; set; }  //Data 150Kg Cân nguyên liệu đầu vào để sản xuất số 1
        public float V_Product_1 { get; set; }   //Data 150Kg Cân thành phẩm đầu ra 1
    }

    public class Line3Response : TbSession
    {
        public float V_Product_1_1 { get; set; }  //Data 150Kg Cân tấm thành phẩm đầu ra 1 và 2
    }

    public class WeighSessionResponse
    {
        public string DateKey { get; set; }
        public int? OrderIndex { get; set; }
    }
}
