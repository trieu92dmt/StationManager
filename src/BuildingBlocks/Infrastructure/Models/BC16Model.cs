﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("BC16Model", Schema = "Report")]
    public partial class BC16Model
    {
        [Key]
        public Guid BC16Id { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        public Guid? LSXSAPId { get; set; }
        [StringLength(50)]
        public string LSXSAP { get; set; }
        [StringLength(1000)]
        public string LSX { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(500)]
        public string ProductName { get; set; }
        [StringLength(50)]
        public string ProductAttributes { get; set; }
        [StringLength(500)]
        public string ProductAttributesName { get; set; }
        [StringLength(50)]
        public string MaterialCode { get; set; }
        [StringLength(500)]
        public string MaterialName { get; set; }
        [StringLength(50)]
        public string DVT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLCTKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? P1 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? P2 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? P3 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TheTich { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLCTKHTheoSP { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NgayHTDC { get; set; }
        [StringLength(500)]
        public string LSXDT { get; set; }
        [StringLength(500)]
        public string DSX { get; set; }
        [StringLength(50)]
        public string StepCode { get; set; }
        public int? StepIndex { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Step_SLKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Step_SLHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Step_SLCL { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Step_NCTT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}