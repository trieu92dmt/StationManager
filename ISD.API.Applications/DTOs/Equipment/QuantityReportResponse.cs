using ISD.API.Applications.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Equipment
{
    public class QuantityReportResponse
    {
        public int STT { get; set; }
        //Tổ sản xuất
        public string DepartmentName { get; set; }
        //MSNV/Tên
        public string Employee { get; set; }
        //SL chi tiết KH
        public decimal? QuantityCustomer { get; set; }
        //SL chi tiết TT
        public decimal? QuantityActual { get; set; }
        //%HT
        public decimal? Complete { get; set; }
        //Số phút KH
        public decimal? MinuteCustomer { get; set; }
        //Số phút TT
        public decimal? MinuteActual { get; set; }
        //%Năng suất
        public decimal? Productivity { get; set; }
    }

    public class QuantityReportListResponse
    {
        public List<QuantityReportResponse> QuantityReports { get; set; } = new List<QuantityReportResponse>();
        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
