﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("GoodsReceiptTypeTModel", Schema = "DataCollection")]
    public partial class GoodsReceiptTypeTModel
    {
        [Key]
        public Guid GoodsReceiptTypeTId { get; set; }
        public Guid? WeightSessionId { get; set; }
        [StringLength(50)]
        public string WeightHeadCode { get; set; }
        [StringLength(50)]
        public string DateKey { get; set; }
        public int? OrderIndex { get; set; }
        [StringLength(50)]
        public string WeightVote { get; set; }
        public Guid? DetailODId { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(50)]
        public string MaterialCode { get; set; }
        public long? MaterialCodeInt { get; set; }
        [StringLength(50)]
        public string PurchaseOrderCode { get; set; }
        [StringLength(50)]
        public string SlocCode { get; set; }
        [StringLength(200)]
        public string SlocName { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmQty { get; set; }
        [StringLength(50)]
        public string UOM { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityWithPackaging { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [StringLength(50)]
        public string Customer { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        public int? QuantityWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TotalQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DeliveryQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OpenQuantity { get; set; }
        public int? BagQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SingleWeight { get; set; }
        [StringLength(50)]
        public string VehicleCode { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? InputWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OutputWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GoodsWeight { get; set; }
        [StringLength(50)]
        public string MaterialDocument { get; set; }
        [StringLength(50)]
        public string ReverseDocument { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        [StringLength(2000)]
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

        [ForeignKey("DetailODId")]
        [InverseProperty("GoodsReceiptTypeTModel")]
        public virtual DetailOutboundDeliveryModel DetailOD { get; set; }
        [ForeignKey("TruckInfoId")]
        [InverseProperty("GoodsReceiptTypeTModel")]
        public virtual TruckInfoModel TruckInfo { get; set; }
    }
}