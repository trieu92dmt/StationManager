using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKDCNB
{
    public class GetDataByOdAndOdItem
    {
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialName { get; set; }
        //Batch
        public string Batch { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Total Qty
        public decimal? TotalQty { get; set; }
        //Delivery Qty
        public decimal? DeliveryQty { get; set; }
        //Open Qty
        public decimal? OpenQty { get; set; }
    }
}
