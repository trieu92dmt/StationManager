﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("BC19Model", Schema = "Report")]
    public partial class BC19Model
    {
        [Key]
        public Guid BC19Id { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDateDSX { get; set; }
        [StringLength(500)]
        public string DSX { get; set; }
        [StringLength(50)]
        public string LSXSAP { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(50)]
        public string ProductName { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PlanQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedQuantity { get; set; }
        [StringLength(50)]
        public string CDL { get; set; }
        public int? CDLIndex { get; set; }
        [StringLength(50)]
        public string CDLCode { get; set; }
        [StringLength(500)]
        public string CDN { get; set; }
        public int? CDNIndex { get; set; }
        [StringLength(50)]
        public string CDNCode { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLCTKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLCTTT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FromTime { get; set; }
        public int? TransferWaitTime { get; set; }
        [StringLength(50)]
        public string Warning { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}