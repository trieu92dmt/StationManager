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
    
    public partial class RatingModel
    {
        public System.Guid RatingId { get; set; }
        public string RatingTypeCode { get; set; }
        public Nullable<System.Guid> ReferenceId { get; set; }
        public string Ratings { get; set; }
        public string Reviews { get; set; }
        public Nullable<System.Guid> ProfileId { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
    }
}
