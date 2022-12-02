﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class RoutingInventorViewModel
    {
        public System.Guid RoutingInventorId { get; set; }
        public string MANDT { get; set; }
        public string VERSO { get; set; }
        public string ARBPL { get; set; }
        public string ARBPL_SUB { get; set; }
        public string WERKS { get; set; }
        public string MATNR { get; set; }
        public string ITMNO { get; set; }
        public string KTEXT { get; set; }
        public string IDNRK { get; set; }
        public string IDNRK_MES { get; set; }
        public string MAKTX { get; set; }
        public string BMEIN { get; set; }
        public Nullable<decimal> BMSCH { get; set; }
        public string PLNNR { get; set; }
        public string LTXA1 { get; set; }
        public string MEINS { get; set; }
        public Nullable<decimal> MENGE { get; set; }
        public Nullable<decimal> VGW01 { get; set; }
        public Nullable<decimal> VGW02 { get; set; }
        public Nullable<decimal> VGW03 { get; set; }
        public Nullable<decimal> VGW04 { get; set; }
        public Nullable<decimal> VGW05 { get; set; }
        public Nullable<decimal> VGW06 { get; set; }
        public string ACTON { get; set; }
        public Nullable<System.DateTime> ANDAT { get; set; }
        public string AENAM { get; set; }
        public Nullable<System.DateTime> AEDAT { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
    }
}
