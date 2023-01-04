﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("WeighingSessionModel", Schema = "MES")]
    public partial class WeighingSessionModel
    {
        [Key]
        public Guid WeighingSessionId { get; set; }
        public Guid? WeighId { get; set; }
        public int? NumberOfWeighs { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(50)]
        public string ZSLBAO { get; set; }
        [StringLength(50)]
        public string ZDONTRONG { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [StringLength(10)]
        public string Active { get; set; }
    }
}