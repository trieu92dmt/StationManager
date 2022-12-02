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
    
    public partial class SalesEmployeeModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesEmployeeModel()
        {
            this.PersonInChargeModel = new HashSet<PersonInChargeModel>();
            this.DeliveryModel = new HashSet<DeliveryModel>();
            this.StockReceivingMasterModel = new HashSet<StockReceivingMasterModel>();
            this.TransferModel = new HashSet<TransferModel>();
        }
    
        public string SalesEmployeeCode { get; set; }
        public Nullable<System.Guid> CompanyId { get; set; }
        public Nullable<System.Guid> StoreId { get; set; }
        public Nullable<System.Guid> DepartmentId { get; set; }
        public string SalesEmployeeName { get; set; }
        public string AbbreviatedName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SerialTag { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
        public Nullable<bool> Actived { get; set; }
        public string Position { get; set; }
        public string Note { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonInChargeModel> PersonInChargeModel { get; set; }
        public virtual DepartmentModel DepartmentModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryModel> DeliveryModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockReceivingMasterModel> StockReceivingMasterModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferModel> TransferModel { get; set; }
    }
}
