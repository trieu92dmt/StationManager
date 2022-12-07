using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Equipment
{
    public class StatusEquipmentResponse
    {
        //Phân xưởng
        public string Workshop { get; set; }
        //Mã máy
        public string MachineCode { get; set; }
        //Têm máy
        public string MachineName { get; set; }
        //Phân loại
        public string EquipmentTypeCode { get; set; }
        //SL Công nhân đang đứng máy
        public int? NumberEmployee { get; set; }
        //Lệnh sản xuất đang thực hiện
        public string CurrentLSX { get; set; }
        //TP/BTP đang chạy
        public string CurrentBTP { get; set; }
        //Số lượng
        public int? Quantity { get; set; }
        //Thời Gian bắt đầu
        public DateTime? StartTime { get; set; }
        //TG kết thúc dự kiến
        public DateTime? FinishTime { get; set; }
        //LSX Đang chờ
        public string WaitingLSX { get; set; }
        //Trạng thái
        public string Status { get; set; }
    }
}
