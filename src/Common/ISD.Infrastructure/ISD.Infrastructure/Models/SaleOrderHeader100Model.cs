﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("SaleOrderHeader100Model", Schema = "MES")]
    public partial class SaleOrderHeader100Model
    {
        [Key]
        public Guid SO100HeaderId { get; set; }
        [StringLength(50)]
        public string VBELN { get; set; }
        [StringLength(10)]
        public string BEDAE { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AUDAT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ERDAT { get; set; }
        public TimeSpan? ERZET { get; set; }
        [StringLength(50)]
        public string ERNAM { get; set; }
        [StringLength(10)]
        public string AUART { get; set; }
        [StringLength(10)]
        public string VKORG { get; set; }
        [StringLength(10)]
        public string VTWEG { get; set; }
        [StringLength(10)]
        public string SPART { get; set; }
        [StringLength(50)]
        public string BSTNK { get; set; }
        [StringLength(50)]
        public string KUNNR { get; set; }
        [StringLength(100)]
        public string PS_PSP_PNR { get; set; }
        [StringLength(100)]
        public string PS_PSP_PNR_OUTPUT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? VDATU { get; set; }
        [StringLength(50)]
        public string LFGSK { get; set; }
        [StringLength(50)]
        public string ZZTERM { get; set; }
        [StringLength(500)]
        public string ZZTERM_DES { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? isSyncWithSAP { get; set; }
        [StringLength(50)]
        public string SORTL { get; set; }
    }
}