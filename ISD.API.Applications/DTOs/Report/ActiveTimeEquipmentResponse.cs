using ISD.API.Constant.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Report
{
    public class ActiveTimeEquipmentResponse
    {
        public int STT { get; set; }
        //Ngày
        public string FromTime { get; set; }
        //Máy/chuyền
        public string Equipment { get; set; }
        //Tổng thời gian trong ngày
        public decimal TotalTime { get; set; }
        //Canh máy
        public EquipmentStatusReport CMStatus { get; set; }
        //Hoạt động
        public EquipmentStatusReport HĐStatus { get; set; }
        //Vệ sinh
        public EquipmentStatusReport VSStatus { get; set; }
        //Ngưng máy
        public EquipmentStatusReport NMStatus { get; set; }
    }

    public class EquipmentStatusReport
    {
        public decimal ActiveTime { get; set; }
        public decimal Rate { get; set; }
    }
}
