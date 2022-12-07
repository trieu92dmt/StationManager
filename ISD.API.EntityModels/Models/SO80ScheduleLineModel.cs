﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("SO80ScheduleLineModel", Schema = "MES")]
    public partial class SO80ScheduleLineModel
    {
        [Key]
        public Guid SO80ScheduleLineId { get; set; }
        [StringLength(50)]
        public string VBELN { get; set; }
        [StringLength(50)]
        public string POSNR { get; set; }
        [StringLength(10)]
        public string ETENR { get; set; }
        [StringLength(10)]
        public string LFREL { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EDATU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? WMENG { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BMENG { get; set; }
        [StringLength(10)]
        public string VRKME { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? LMENG { get; set; }
        [StringLength(10)]
        public string MEINS { get; set; }
        [StringLength(10)]
        public string LIFSP { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DLVQTY_BU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DLVQTY_SU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OCDQTY_BU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OCDQTY_SU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ORDQTY_BU { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ORDQTY_SU { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CREA_DLVDATE { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? REQ_DLVDATE { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}