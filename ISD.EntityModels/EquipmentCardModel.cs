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
    
    public partial class EquipmentCardModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EquipmentCardModel()
        {
            this.EquipmentCard_Equiment_Mapping = new HashSet<EquipmentCard_Equiment_Mapping>();
        }
    
        public System.Guid EquipmentCardId { get; set; }
        public string StepCode { get; set; }
        public string WorkOrderCode { get; set; }
        public string BarcodePath { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentCard_Equiment_Mapping> EquipmentCard_Equiment_Mapping { get; set; }
    }
}
