namespace ISD.API.Applications.DTOs.Equipment
{
    public class BarcodeEquipmentResponse
    {
        public Guid? EquipmentCardId { get; set; }
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
        public string StepName { get; set; }
        public string StepCode { get; set; }
        public string WorkShopName { get; set; }
        public string Plant { get; set; }
        public string QRCode { get; set; }
        public string Departments { get; set; }
        public List<SaleEmployeeCheckInResponse> Employees { get; set; } = new List<SaleEmployeeCheckInResponse>();  
        public List<EquipmentUseResponse> Equipments { get; set; } = new List<EquipmentUseResponse>();
    }

    public class SaleEmployeeCheckInResponse
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeNameDisplay => $"{EmployeeCode} | {EmployeeName}";
    }

    public class EquipmentUseResponse
    {
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
    }
}
