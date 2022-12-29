﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("SaleOrderItem100Model", Schema = "MES")]
    public partial class SaleOrderItem100Model
    {
        [Key]
        public Guid SO100ItemId { get; set; }
        [StringLength(50)]
        public string VBELN { get; set; }
        [StringLength(50)]
        public string POSNR { get; set; }
        [StringLength(100)]
        public string MATNR { get; set; }
        [StringLength(100)]
        public string ARKTX { get; set; }
        [StringLength(10)]
        public string BEDAE { get; set; }
        [StringLength(10)]
        public string WERKS { get; set; }
        [StringLength(10)]
        public string UEBTO { get; set; }
        [StringLength(10)]
        public string UNTTO { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? KWMENG { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? LSMENG { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? KBMENG { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? KLMENG { get; set; }
        public int? ETENR { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EDATU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? WMENG { get; set; }
        [StringLength(10)]
        public string VRKME { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? LMENG { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DLVQTY_BU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ORDQTY_BU { get; set; }
        [StringLength(10)]
        public string SOBKZ { get; set; }
        [StringLength(100)]
        public string PS_PSP_PNR { get; set; }
        [StringLength(100)]
        public string PS_PSP_PNR_OUTPUT { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? UMVKZ { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? UMVKN { get; set; }
        [StringLength(10)]
        public string MEINS { get; set; }
        [StringLength(10)]
        public string ABGRU { get; set; }
        [StringLength(10)]
        public string GBSTA { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ERDAT { get; set; }
        [StringLength(50)]
        public string ERNAM { get; set; }
        public TimeSpan? ERZET { get; set; }
        [StringLength(50)]
        public string ZZTERM { get; set; }
        [StringLength(500)]
        public string ZZTERM_DES { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? isDeleted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedTime { get; set; }
        [StringLength(50)]
        public string POSNR_MES { get; set; }
        [StringLength(50)]
        public string UPMAT { get; set; }
        [StringLength(500)]
        public string ZPYCSXDT { get; set; }
        [StringLength(10)]
        public string ZFLAG { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ZFDAT { get; set; }
        public TimeSpan? ZFTM { get; set; }
        [StringLength(500)]
        public string ZZSKU { get; set; }
        [StringLength(50)]
        public string ZPTG { get; set; }
    }
}