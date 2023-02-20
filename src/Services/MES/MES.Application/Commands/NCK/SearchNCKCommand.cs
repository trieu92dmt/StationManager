using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NCK
{
    public class SearchNCKCommand
    {
        //Plant
        public string Plant { get; set; }
        //Receving Sloc
        public string SlocFrom { get; set; }
        public string SlocTo { get; set; }
        //Reservation
        public string ReservationFrom { get; set; }
        public string ReservationTo { get; set; }
        //Material Doc
        public string MaterialDocFrom { get; set; }
        public string MaterialDocTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Document Date
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        //CreateBy
        public Guid? CreateBy { get; set; }
    }
}
