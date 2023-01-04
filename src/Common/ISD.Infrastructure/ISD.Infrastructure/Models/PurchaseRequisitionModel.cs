﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("PurchaseRequisitionModel", Schema = "MES")]
    public partial class PurchaseRequisitionModel
    {
        [Key]
        public Guid PurchaseRequisitionId { get; set; }
        [StringLength(50)]
        public string BANFN { get; set; }
        [StringLength(10)]
        public string BNFPO { get; set; }
        [StringLength(10)]
        public string BSART { get; set; }
        [StringLength(10)]
        public string LOEKZ { get; set; }
        [StringLength(10)]
        public string FRGKZ { get; set; }
        [StringLength(10)]
        public string EKGRP { get; set; }
        [StringLength(100)]
        public string TXZ01 { get; set; }
        [StringLength(100)]
        public string MATNR { get; set; }
        [StringLength(10)]
        public string WERKS { get; set; }
        [StringLength(10)]
        public string LGORT { get; set; }
        [StringLength(50)]
        public string BEDNR { get; set; }
        [StringLength(10)]
        public string MATKL { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MENGE { get; set; }
        [StringLength(10)]
        public string MEINS { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LFDAT { get; set; }
        [StringLength(10)]
        public string PSTYP { get; set; }
        [StringLength(10)]
        public string KNTTP { get; set; }
        [StringLength(10)]
        public string EKORG { get; set; }
        [StringLength(50)]
        public string VBELN { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GR_QTY { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BSMNG_SUM_PO { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? isDeleted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedTime { get; set; }
        [StringLength(50)]
        public string MType { get; set; }
        [StringLength(50)]
        public string AFNAM { get; set; }
    }
}