using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XK
{
    public class GetDataByRsvAndRsvItemResponse
    {
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //Movement Type
        public string MovementType { get; set; }
        //Batch
        public string Batch { get; set; }
        //Total Qty
        public decimal? TotalQty { get; set; }
        //Dellivered Qty
        public decimal? DeliveryQty { get; set; }
        //Open Qty
        public decimal? OpenQty => TotalQty - DeliveryQty;
        //Unit
        public string Unit { get; set; }
    }
}
