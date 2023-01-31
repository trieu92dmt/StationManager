using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.OutboundDelivery
{
    public class SearchOutboundDeliveryResponse
    {
        //Plant
        public string Plant { get; set; }
        //Ship-to party name
        public string ShipToPartyName { get; set; }
        //Outbound Delivery
        public string OutboundDelivery { get; set; }
        //Outbound Delivery Item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material Description
        public string MaterialDesc { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        //Storage Location Description
        public string SlocDesc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Total Quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveryQty { get; set; }
        //Units of Measure
        public string Unit { get; set; }
        //Document Date
        public DateTime DocumentDate { get; set; }
        //Ship to Party
        public string ShipToParty { get; set; }
    }
}
