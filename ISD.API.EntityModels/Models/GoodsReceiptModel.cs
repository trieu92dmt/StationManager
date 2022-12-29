﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("GoodsReceiptModel", Schema = "DataCollection")]
    public partial class GoodsReceiptModel
    {
        [Key]
        public Guid GoodsReceiptId { get; set; }
        public Guid? PurchaseOrderDetailId { get; set; }
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
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityWeitght { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TotalQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DeliveredQuantity { get; set; }
        public int? TruckQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? InputWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OutputWeight { get; set; }
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
        [Column(TypeName = "datetime")]
        public DateTime? DocumentDate { get; set; }
        public int? DateKey { get; set; }
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

        [ForeignKey("PurchaseOrderDetailId")]
        [InverseProperty("GoodsReceiptModel")]
        public virtual PurchaseOrderDetailModel PurchaseOrderDetail { get; set; }
    }
}