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
    
    public partial class ProductAttributeModel
    {
        public System.Guid ProductId { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string Color { get; set; }
        public string Thickness { get; set; }
        public string Allocation { get; set; }
        public string Grade { get; set; }
        public string Surface { get; set; }
        public string NumberOfSurface { get; set; }
        public Nullable<decimal> GrossWeight { get; set; }
        public Nullable<decimal> NetWeight { get; set; }
        public string WeightUnit { get; set; }
    }
}
