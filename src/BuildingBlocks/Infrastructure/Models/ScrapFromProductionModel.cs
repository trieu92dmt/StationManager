﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("ScrapFromProductionModel", Schema = "DataCollection")]
    public partial class ScrapFromProductionModel
    {
        [Key]
        public Guid ScFromProductiontId { get; set; }
        public Guid? DetailWorkOrderId { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(50)]
        public string ComponentCode { get; set; }
        public long? ComponentCodeInt { get; set; }
        public Guid? WeightId { get; set; }
        [StringLength(50)]
        public string WeightVote { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BagQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SingleWeight { get; set; }
        [StringLength(50)]
        public string WeightHeadCode { get; set; }
        [StringLength(50)]
        public string DateKey { get; set; }
        public int? OrderIndex { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal TotalQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal RequirementQuantiy { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal QuantityWithdrawn { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmQty { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityWithPackaging { get; set; }
        public int? QuantityWeitght { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        [StringLength(2000)]
        public string Image { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [StringLength(50)]
        public string MaterialDocument { get; set; }
        [StringLength(50)]
        public string ReverseDocument { get; set; }
        [StringLength(50)]
        public string SlocCode { get; set; }
        [StringLength(50)]
        public string SlocName { get; set; }
        public bool? IsReverse { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("DetailWorkOrderId")]
        [InverseProperty("ScrapFromProductionModel")]
        public virtual DetailWorkOrderModel DetailWorkOrder { get; set; }
    }
}