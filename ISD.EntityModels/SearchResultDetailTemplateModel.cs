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
    
    public partial class SearchResultDetailTemplateModel
    {
        public System.Guid SearchResultDetailTemplateId { get; set; }
        public Nullable<System.Guid> SearchResultTemplateId { get; set; }
        public Nullable<int> PivotArea { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string CellFormat_FormatType { get; set; }
        public string CellFormat_FormatString { get; set; }
        public Nullable<int> AreaIndex { get; set; }
        public Nullable<bool> Visible { get; set; }
        public Nullable<int> Width { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<bool> Resize { get; set; }
        public Nullable<bool> Tree { get; set; }
    
        public virtual SearchResultTemplateModel SearchResultTemplateModel { get; set; }
    }
}
