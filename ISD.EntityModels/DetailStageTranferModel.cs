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
    
    public partial class DetailStageTranferModel
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> StageTranferId { get; set; }
        public string LotNumber { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<System.Guid> StockId { get; set; }
        public string ProductionStatus { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
    
        public virtual StageTransferModel StageTransferModel { get; set; }
        public virtual StockModel StockModel { get; set; }
    }
}
