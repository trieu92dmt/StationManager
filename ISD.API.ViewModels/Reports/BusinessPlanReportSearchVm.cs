using System;

namespace ISD.API.ViewModels
{
    public class BusinessPlanReportSearchVm
    {
        public string CompanyCode { get; set; }
        public string EmployeeCode { get; set; }
        public int PlanYear { get; set; }
        public bool isSummary { get; set; }
        public string ProfileForeignCode { get; set; }
    }
}
