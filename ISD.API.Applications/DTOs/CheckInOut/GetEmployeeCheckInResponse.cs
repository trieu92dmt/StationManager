namespace ISD.API.Applications.DTOs.CheckInOut
{
    public class GetEmployeeCheckInResponse
    {
        public string DepartmentName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public bool IsCheckIn { get; set; }
        public string StatusCheckIn => IsCheckIn == true ? "Đã check-in" : "Chưa check-in";
    }
}
