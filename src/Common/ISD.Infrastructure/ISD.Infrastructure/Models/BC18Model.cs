﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("BC18Model", Schema = "Report")]
    public partial class BC18Model
    {
        [Key]
        public Guid BC18Id { get; set; }
        [StringLength(50)]
        public string SaleOrg { get; set; }
        public Guid? WorkShopId { get; set; }
        [StringLength(500)]
        public string WorkShop { get; set; }
        [StringLength(50)]
        public string WorkCenterCode { get; set; }
        [StringLength(500)]
        public string WorkCenter { get; set; }
        [StringLength(50)]
        public string LSXSAP { get; set; }
        [StringLength(50)]
        public string DSX { get; set; }
        [StringLength(50)]
        public string LSXDT { get; set; }
        public Guid? ProductId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CompletedDateTime { get; set; }
        public int? SLKH { get; set; }
        [StringLength(50)]
        public string ERPProductCode { get; set; }
        [StringLength(500)]
        public string ProductName { get; set; }
        [StringLength(50)]
        public string ProductAttributes { get; set; }
        [StringLength(500)]
        public string KTEXT { get; set; }
        [StringLength(50)]
        public string IDNRK_MES { get; set; }
        [StringLength(500)]
        public string MAKTX { get; set; }
        [StringLength(50)]
        public string BMEIN { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BMSCH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? P1 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? P2 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? P3 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLCTKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MENGE { get; set; }
        [StringLength(50)]
        public string StockRecevingType { get; set; }
        public bool? IsWorkCenterCompleted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? WorkCenterConfirmTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}