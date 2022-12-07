namespace ISD.API.Applications.DTOs.CheckInOut
{
    public class CheckInOutResponse
    {
        public Guid Id { get; set; }
        public string EquipmentName { get; set; }
        public string StepName { get; set; }
        public string DepartmentNames { get; set; }
        public int? QuantityEmployeeCheckIn { get; set; }
        public List<EmployeeCheckInResponse> EmployeeCheckIns { get; set; } = new List<EmployeeCheckInResponse>();
    }

    public class EmployeeCheckInResponse
    {
        public int STT { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
    }
}
