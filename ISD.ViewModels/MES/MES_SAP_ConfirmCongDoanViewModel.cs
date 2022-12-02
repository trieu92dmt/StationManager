using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class MES_SAP_ConfirmCongDoanViewModel
    {
        public Guid? LINE_ID { get; set; }
        public string WERKS { get; set; }
        public string AUFNR_DT { get; set; }
        public string KDAUF { get; set; }
        public string KDPOS { get; set; }
        public string STAGE_MES { get; set; }
        public string AUFNR { get; set; }
        public string MATNR { get; set; }
        public string MAKTX { get; set; }
        public int? LFIMG_LSX { get; set; }
        public string MEINS { get; set; }
        public decimal? LFIMG_DC { get; set; }
        public string PRODUCT_ATT { get; set; }
        public string KTEXT { get; set; }
        public string STOCK_CODE { get; set; }
        public decimal? LFIMG { get; set; }
        public Guid? CUST_REF { get; set; }
        public string STOCK_TYPE { get; set; }
        public int? DATE_KEY { get; set; }
        public DateTime? FROM_TIME { get; set; }
        public DateTime? TO_TIME { get; set; }
        public int? PHASE { get; set; }
        public string MVT_TYPE { get; set; }
        public Guid? DEPT_ID { get; set; }
        public string DEPT_CODE { get; set; }
        public string DEPT_NAME { get; set; }
        public bool? IS_DELETED { get; set; }
        public DateTime? ERDAT { get; set; }
        public string ERNAM { get; set; }
        public bool? IS_WORKC_COMP { get; set; }
        public string CF_WORKC { get; set; }
        public int? CF_WORKC_DATE { get; set; }
        public string CF_ERNAM { get; set; }
    }
}
