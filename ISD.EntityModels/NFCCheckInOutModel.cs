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
    
    public partial class NFCCheckInOutModel
    {
        public System.Guid CheckInId { get; set; }
        public System.DateTime CheckInDate { get; set; }
        public string SerialTag { get; set; }
        public string Description { get; set; }
        public Nullable<System.Guid> WorkingDepartment { get; set; }
        public Nullable<System.Guid> CheckInOutDepartment { get; set; }
    }
}
