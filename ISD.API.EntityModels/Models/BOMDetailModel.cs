﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("BOMDetailModel", Schema = "MES")]
    public partial class BOMDetailModel
    {
        [Key]
        public Guid BomDetailId { get; set; }
        [StringLength(10)]
        public string STLTY { get; set; }
        [StringLength(100)]
        public string STLNR { get; set; }
        [StringLength(100)]
        public string STLKN { get; set; }
        [StringLength(100)]
        public string STPOZ { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DATUV { get; set; }
        [StringLength(10)]
        public string LKENZ { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ANDAT { get; set; }
        [StringLength(100)]
        public string ANNAM { get; set; }
        [StringLength(500)]
        public string IDNRK { get; set; }
        [StringLength(500)]
        public string IDNRK_MES { get; set; }
        [StringLength(500)]
        public string ComponentText { get; set; }
        [StringLength(500)]
        public string MAKTX { get; set; }
        [StringLength(100)]
        public string MTART { get; set; }
        [StringLength(10)]
        public string POSTP { get; set; }
        [StringLength(50)]
        public string POSNR { get; set; }
        [StringLength(50)]
        public string MEINS { get; set; }
        [Column(TypeName = "decimal(13, 3)")]
        public decimal? MENGE { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AUSCH { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AVOAU { get; set; }
        [StringLength(500)]
        public string POTX1 { get; set; }
        [StringLength(500)]
        public string P1 { get; set; }
        [StringLength(500)]
        public string P2 { get; set; }
        [StringLength(500)]
        public string P3 { get; set; }
        [StringLength(500)]
        public string P4 { get; set; }
        [StringLength(500)]
        public string P5 { get; set; }
        [StringLength(500)]
        public string POTX2 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        [StringLength(10)]
        public string WERKS { get; set; }
        [StringLength(50)]
        public string MATNR { get; set; }
        [StringLength(50)]
        public string AENNR { get; set; }
        [StringLength(50)]
        public string POSNR_MES { get; set; }
        public Guid? CreateBy { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }
    }
}