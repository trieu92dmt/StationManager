using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKTPSX
{
    public class GetDataByWoResponse
    {
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialName { get; set; }
        //Batch
        public string Batch { get; set; }
        //Total Qty
        public decimal? TotalQty { get; set; }
        //Delivery Qty
        public decimal? DeliveryQty { get; set; }
        //Open Qty
        public decimal? OpenQty { get; set; }
    }
}
