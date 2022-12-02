using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class View_BOM_Inventor_RipViewModel
    {
        public string PART_ID { get; set; }
        public string MATNR { get; set; }
        public string IDNRK_MES { get; set; }
        public decimal? P2 { get; set; }
        public string POT12 { get; set; }
        public string POT21 { get; set; }
        public decimal? MENGE { get; set; }
        public string MEINS { get; set; }
        public string VERSO { get; set; }
        public decimal? P1 { get; set; }
        public decimal? P3 { get; set; }
        
    }
}
