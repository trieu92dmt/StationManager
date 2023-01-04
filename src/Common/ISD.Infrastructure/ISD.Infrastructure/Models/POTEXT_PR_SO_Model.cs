﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("POTEXT_PR_SO_Model", Schema = "MES")]
    public partial class POTEXT_PR_SO_Model
    {
        [Key]
        public Guid POTEXTId { get; set; }
        [StringLength(50)]
        public string BSART { get; set; }
        [StringLength(50)]
        public string EBELN { get; set; }
        [StringLength(10)]
        public string EBELP { get; set; }
        [StringLength(50)]
        public string ZMES_ZPOREF { get; set; }
        [StringLength(50)]
        public string BANFN { get; set; }
        [StringLength(10)]
        public string BNFPO { get; set; }
        [StringLength(50)]
        public string ZMES_VBELN { get; set; }
        [StringLength(10)]
        public string LOEKZ { get; set; }
        [StringLength(50)]
        public string MATNR { get; set; }
        [StringLength(10)]
        public string WERKS { get; set; }
        [StringLength(10)]
        public string BUKRS { get; set; }
        [StringLength(50)]
        public string TXZ01 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MENGE { get; set; }
        [StringLength(10)]
        public string MEINS { get; set; }
        [StringLength(10)]
        public string BPRME { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? UMREZ { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? UMREN { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BEDAT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AEDAT { get; set; }
        [StringLength(10)]
        public string ZCHECK { get; set; }
        [StringLength(50)]
        public string ERNAM { get; set; }
        [StringLength(500)]
        public string ZMES_LSXLON { get; set; }
        [StringLength(500)]
        public string ZMES_IDLSX { get; set; }
        [StringLength(500)]
        public string ZMES_POCN { get; set; }
        public bool? SOInvalid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}