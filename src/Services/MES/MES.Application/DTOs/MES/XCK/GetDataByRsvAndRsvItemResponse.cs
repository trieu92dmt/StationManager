using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XCK
{
    public class GetDataByRsvAndRsvItemResponse
    {
        //Material
        public string Material { get; set; }
        //Material name
        public string MaterialDesc { get; set; }
        //Movement type
        public string MovementType { get; set; }
        //Rec Sloc
        public string ReceivingSloc { get; set; }
        public string ReceivingSlocName { get; set; }
        public string ReceivingSlocFmt => !string.IsNullOrEmpty(ReceivingSloc) && !string.IsNullOrEmpty(ReceivingSlocName) ? $"{ReceivingSloc} | {ReceivingSlocName}" : "";
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
