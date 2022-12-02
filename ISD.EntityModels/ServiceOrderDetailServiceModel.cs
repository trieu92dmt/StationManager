//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISD.EntityModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class ServiceOrderDetailServiceModel
    {
        public System.Guid ServiceOrderDetailServiceId { get; set; }
        public System.Guid ServiceOrderId { get; set; }
        public Nullable<System.Guid> FixingTypeId { get; set; }
        public string MaterialNumber { get; set; }
        public string ShortText { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> HourPrice { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string ServiceTypeCode { get; set; }
        public string AccessoryCode { get; set; }
        public string AccessoryCodeReference { get; set; }
        public Nullable<System.Guid> AccessoryIdReference { get; set; }
        public string AccessoryName { get; set; }
        public Nullable<decimal> AccessoryPrice { get; set; }
        public string Note { get; set; }
        public string Plant { get; set; }
        public Nullable<int> Number { get; set; }
    
        public virtual ServiceTypeModel ServiceTypeModel { get; set; }
        public virtual ServiceOrderModel ServiceOrderModel { get; set; }
    }
}
