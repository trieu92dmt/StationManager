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
    
    public partial class TemplateAndGiftMemberAddressModel
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> TempalteAndGiftMemberId { get; set; }
        public string Address { get; set; }
        public Nullable<System.Guid> ProductId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
    
        public virtual AccountModel AccountModel { get; set; }
        public virtual AccountModel AccountModel1 { get; set; }
        public virtual ProductModel ProductModel { get; set; }
        public virtual TemplateAndGiftMemberModel TemplateAndGiftMemberModel { get; set; }
    }
}
