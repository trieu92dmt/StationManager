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
    
    public partial class Stock_Store_Mapping
    {
        public System.Guid StockId { get; set; }
        public System.Guid StoreId { get; set; }
        public Nullable<bool> isMain { get; set; }
    
        public virtual StoreModel StoreModel { get; set; }
        public virtual StockModel StockModel { get; set; }
    }
}
