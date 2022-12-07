using System;

namespace ISD.API.ViewModels
{
    public class BusinessPlanReportDetailVm
    {
        public string CompanyCode { get; set; }
        public string EmployeeCode { get; set; }
        public string ProfileForeignCode { get; set; }
        public string ProfileName { get; set; }
        public int PlanYear { get; set; }
        public int PlanMonth { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Value { get; set; }
        public decimal? ActualWeight { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? Commodity { get; set; }
        public decimal? ValueAdded { get; set; }
        public decimal? CommodityWeight { get; set; }
        public decimal? CommodityValue { get; set; }
        public decimal? ValueAddedWeight { get; set; }
        public decimal? ValueAddedValue { get; set; }
        public decimal? ActualCommodityWeight { get; set; }
        public decimal? ActualCommodityValue { get; set; }
        public decimal? ActualValueAddedWeight { get; set; }
        public decimal? ActualValueAddedValue { get; set; }
    }
}
