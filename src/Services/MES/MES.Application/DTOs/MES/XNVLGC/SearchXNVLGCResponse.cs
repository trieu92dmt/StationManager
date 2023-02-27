using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XNVLGC
{
    public class GetInputDataResponse
    {
        //Index key
        public int IndexKey { get; set; }
        //Plant
        public string Plant { get; set; }
        public string PlantName { get; set; }
        //Ship to party name
        public string Vendor { get; set; }
        //Outbound delivery
        public string PurchaseOrder { get; set; }
        //Outbound delivery item
        public string PurchaseOrderItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialDesc { get; set; }
        //Component
        public string Component { get; set; }
        //Component desc
        public string ComponentDesc { get; set; }
        //Document Date
        public DateTime? DocumentDate { get; set; }
        //Batch
        public string Batch { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Storage location
        public string Sloc { get; set; }
        //Storage location desc
        public string SlocName { get; set; }
        public string SlocFmt => !string.IsNullOrEmpty(Sloc) && !string.IsNullOrEmpty(SlocName) ? $"{Sloc} | {SlocName}" : "";
        //Total quantity
        public decimal? TotalQty { get; set; }
        //Delivered quantity
        public decimal? DeliveredQty { get; set; }
        //Open quantity
        public decimal? OpenQty => TotalQty - DeliveredQty;
        //UOM
        public string Unit { get; set; }
        //ShipToParty
        public string ShipToParty { get; set; }
    }
    public class SearchXNVLGCResponse
    {
    }
}
