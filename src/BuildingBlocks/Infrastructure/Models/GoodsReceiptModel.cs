﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("GoodsReceiptModel", Schema = "DataCollection")]
    public partial class GoodsReceiptModel
    {
        [Key]
        public Guid GoodsReceiptId { get; set; }
        public Guid? PurchaseOrderDetailId { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(50)]
        public string MaterialCode { get; set; }
        public long? MaterialCodeInt { get; set; }
        [StringLength(50)]
        public string WeightId { get; set; }
        [StringLength(50)]
        public string WeitghtVote { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BagQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SingleWeight { get; set; }
        [StringLength(50)]
        public string WeightHeadCode { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmQty { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityWithPackaging { get; set; }
        [StringLength(50)]
        public string VehicleCode { get; set; }
        public int? QuantityWeitght { get; set; }
        public Guid? TruckInfoId { get; set; }
        [StringLength(50)]
        public string TruckQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? InputWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OutputWeight { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? WeighDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DocumentDate { get; set; }
        public int? DateKey { get; set; }
        [StringLength(50)]
        public string SlocCode { get; set; }
        [StringLength(50)]
        public string SlocName { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [StringLength(50)]
        public string MaterialDocument { get; set; }
        [StringLength(50)]
        public string ReverseDocument { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }
        [StringLength(2000)]
        public string Img { get; set; }

        [ForeignKey("PurchaseOrderDetailId")]
        [InverseProperty("GoodsReceiptModel")]
        public virtual PurchaseOrderDetailModel PurchaseOrderDetail { get; set; }
    }
}