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
    
    public partial class BC19Model
    {
        public System.Guid BC19Id { get; set; }
        public string Plant { get; set; }
        public Nullable<System.DateTime> EndDateDSX { get; set; }
        public string DSX { get; set; }
        public string LSXSAP { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> PlanQuantity { get; set; }
        public Nullable<decimal> CompletedQuantity { get; set; }
        public string CDL { get; set; }
        public Nullable<int> CDLIndex { get; set; }
        public string CDLCode { get; set; }
        public string CDN { get; set; }
        public Nullable<int> CDNIndex { get; set; }
        public string CDNCode { get; set; }
        public Nullable<decimal> SLCTKH { get; set; }
        public Nullable<decimal> SLCTTT { get; set; }
        public Nullable<System.DateTime> FromTime { get; set; }
        public Nullable<int> TransferWaitTime { get; set; }
        public string Warning { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
    }
}
