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
    
    public partial class EquimenRecordModel
    {
        public System.Guid EquipmentRecordId { get; set; }
        public Nullable<System.Guid> OutputRecordId { get; set; }
        public string EquipmentCode { get; set; }
    
        public virtual OutputRecordModel OutputRecordModel { get; set; }
    }
}
