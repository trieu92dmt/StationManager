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
        //Vendor
        public string Vendor { get; set; }
        //Vendor Name
        public string VendorName { get; set; }
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
        //Order Quantity
        public decimal? OrderQuantity { get; set; }
        //Order Unit
        public string OrderUnit { get; set; }
        //Requirement Quantity
        public decimal? RequirementQuantity { get; set; }
        //Requirement Unit
        public string RequirementUnit { get; set; }
    }
    public class SearchXNVLGCResponse
    {
    }
}
