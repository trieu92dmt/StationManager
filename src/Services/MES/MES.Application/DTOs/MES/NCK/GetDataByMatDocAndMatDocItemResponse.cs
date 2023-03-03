using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NCK
{
    public class GetDataByMatDocAndMatDocItemResponse
    {
        //Reservation
        public string Reservation { get; set; }
        //Material
        public string Material { get; set; }
        //Material name
        public string MaterialDesc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Total quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveredQty { get; set; }
        //Open quantity
        public decimal? OpenQty => TotalQty - DeliveredQty;
        //UOM
        public string Unit { get; set; }
    }
}
