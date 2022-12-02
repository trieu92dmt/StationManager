using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
   public class FieldSettingGranttModel
    {
        public Nullable<int> PivotArea { get; set; }
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string CellFormat_FormatType { get; set; }
        public string CellFormat_FormatString { get; set; }
        public Nullable<int> AreaIndex { get; set; }
        public int? Width { set; get; }
        public int? Height { set; get; }
        public bool? Resize { set; get; }
        public bool? Tree { set; get; }
        public bool? Visible { get; set; }
    }
}
