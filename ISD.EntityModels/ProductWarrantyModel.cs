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
    
    public partial class ProductWarrantyModel
    {
        public System.Guid ProductWarrantyId { get; set; }
        public int ProductWarrantyCode { get; set; }
        public System.Guid ProfileId { get; set; }
        public System.Guid ProductId { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.Guid WarrantyId { get; set; }
        public string SerriNo { get; set; }
        public string ProductWarrantyNo { get; set; }
        public Nullable<decimal> ActivatedQuantity { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string SaleOrder { get; set; }
        public string OrderDelivery { get; set; }
        public string ERPProductCode { get; set; }
        public string ProfileName { get; set; }
        public string ProfileShortName { get; set; }
        public string Age { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<System.Guid> CompanyId { get; set; }
        public Nullable<System.Guid> ProvinceId { get; set; }
        public Nullable<System.Guid> DistrictId { get; set; }
        public Nullable<System.Guid> WardId { get; set; }
        public string Address { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
        public string Note { get; set; }
        public Nullable<bool> Actived { get; set; }
    
        public virtual ProfileModel ProfileModel { get; set; }
        public virtual ProductModel ProductModel { get; set; }
        public virtual WarrantyModel WarrantyModel { get; set; }
    }
}
