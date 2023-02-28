﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("ComponentImportModel", Schema = "DataCollection")]
    public partial class ComponentImportModel
    {
        [Key]
        public Guid ComponentImportId { get; set; }
        public Guid? WeightSessionId { get; set; }
        [StringLength(50)]
        public string WeightHeadCode { get; set; }
        [StringLength(50)]
        public string WeightVote { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(200)]
        public string PlantName { get; set; }
        [StringLength(50)]
        public string MaterialCode { get; set; }
        public long? MaterialCodeInt { get; set; }
        [StringLength(200)]
        public string MaterialName { get; set; }
        [StringLength(50)]
        public string SlocCode { get; set; }
        [StringLength(200)]
        public string SlocName { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmQty { get; set; }
        [StringLength(50)]
        public string UOM { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityWithPackaging { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        public int? QuantityWeight { get; set; }
        [StringLength(50)]
        public string VendorCode { get; set; }
        [StringLength(200)]
        public string VendorName { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Weight { get; set; }
        public int? BagQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SingleWeight { get; set; }
        [StringLength(50)]
        public string VehicleCode { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? InputWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OutputWeight { get; set; }
        [StringLength(50)]
        public string MaterialDocument { get; set; }
        [StringLength(50)]
        public string ReverseDocument { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        [StringLength(200)]
        public string Image { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public Guid? TruckInfoId { get; set; }
        [StringLength(50)]
        public string TruckNumber { get; set; }
        public bool? IsReverse { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("WeightSessionId")]
        [InverseProperty("ComponentImportModel")]
        public virtual WeighSessionModel WeightSession { get; set; }
    }
}