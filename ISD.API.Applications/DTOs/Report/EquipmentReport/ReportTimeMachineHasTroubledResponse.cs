namespace ISD.API.Applications.DTOs.Report.EquipmentReport
{
    public class ReportTimeMachineHasTroubledResponse
    {
        public int STT { get; set; }
        public Guid? EquipmentCardId { get; set; }
        public string WorkShopName { get; set; }                //Phân xưởng
        public string EquipmentGroup { get; set; }              //Nhóm máy
        public string EquipmentName { get; set; }               //Máy móc/Chuyền
        public decimal? TotalTimeStop { get; set; }             //Tổng thời gian ngưng máy    
        public List<MachineHasTroubledResponse> DetaileRportTimeMachines { get; set; } = new List<MachineHasTroubledResponse>(); //Chi tiết
    }

    public class MachineHasTroubledResponse
    {
        public string ReasonStopCode { get; set; }              //Mã lý do ngưng máy
        public string ReasonStopName { get; set; }              //Tên lý do ngưng máy
        public decimal? TimeStop { get; set; }                  //Thời gian ngưng máy   
    }
}
