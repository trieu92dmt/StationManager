using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.ViewModels
{
    public class SAPTonKhoReportVm
    {
        public string companycode { get; set; }
        public string planT_DESC { get; set; }
        public string plant { get; set; }
        public string storagE_LOCATION { get; set; }
        public string storagE_LOCATION_DESC { get; set; }
        public string batch { get; set; }
        public string speciaL_STOCK { get; set; }
        public string speciaL_STOCK_NUMBER { get; set; }
        public string material { get; set; }
        public string materiaL_DESC { get; set; }
        public string dvT_CHINH { get; set; }
        public decimal soluonG_DVT_CHINH { get; set; }
        public string dvT_KG2 { get; set; }
        public decimal soluonG_DVT_KG2 { get; set; }
        public string dvT_TT { get; set; }
        public string soluonG_DVT_TT { get; set; }
        public string grdate { get; set; }
        public string quicach { get; set; }
        public int tuoI_HANG { get; set; }
        public string DateKey { get; set; }
    }
}
