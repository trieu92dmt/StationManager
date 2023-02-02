﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("DetailWorkOrderModel", Schema = "DataCollection")]
    public partial class DetailWorkOrderModel
    {
        [Key]
        public Guid DetailWorkOrderId { get; set; }
        public Guid? WorkOrderId { get; set; }
        [StringLength(50)]
        public string WorkOrderItem { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TotalQuantiy { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? RoutingScrapQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ScrapQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GRQuantity { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }
        [StringLength(50)]
        public string UnloadingPoint { get; set; }
        [StringLength(50)]
        public string SerialNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? MRPArea { get; set; }
        [StringLength(50)]
        public string ValuationType { get; set; }
        [StringLength(50)]
        public string ValuationCategory { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [StringLength(50)]
        public string InternalObject { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActualStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmedYieldQuantity { get; set; }
        [StringLength(50)]
        public string DeletionFlag { get; set; }
        [StringLength(50)]
        public string LongTextExists { get; set; }
        [StringLength(50)]
        public string ReferenceOrder { get; set; }
        [StringLength(50)]
        public string SystemStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }
    }
}