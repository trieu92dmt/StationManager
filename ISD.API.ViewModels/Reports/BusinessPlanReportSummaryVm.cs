namespace ISD.API.ViewModels
{
    public class BusinessPlanReportSummaryVm
    {
        public string ProfileForeignCode { get; set; }
        public string ProfileName { get; set; }
        public int PlanYear { get; set; }
        public decimal ActualCommodityWeight { get; set; }
        public decimal ActualCommodityValue { get; set; }
        public decimal ActualValueAddedWeight { get; set; }
        public decimal ActualValueAddedValue { get; set; }
        public decimal PlanCommodityWeight { get; set; }
        public decimal PlanCommodityValue { get; set; }
        public decimal PlanValueAddedWeight { get; set; }
        public decimal PlanValueAddedValue { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }

    }
}
