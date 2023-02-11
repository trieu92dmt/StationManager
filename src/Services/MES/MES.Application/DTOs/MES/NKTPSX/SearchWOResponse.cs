using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKTPSX
{
    public class SearchWOResponse
    {
        //Plant
        public string Plant { get; set; }
        //Production Order
        public string WorkOrder { get; set; }
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        public string SlocName { get; set; }
        //Batch
        public string Batch { get; set; }
        //Total Quantity
        public decimal? TotalQuantity { get; set; }
        //Delivery Quantity
        public decimal? DeliveryQuantity { get; set; }
        //Open Total Quantity
        public decimal? OpenQuantity { get; set; }
        //UoM
        public string Unit { get; set; }
        //Order Type
        public string OrderType { get; set; }
        //Sales Order
        public string SalesOrder { get; set; }
        //Sales order item
        public string SaleOrderItem { get; set; }

    }
}
