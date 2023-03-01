﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("WarehouseImportTransferModel", Schema = "DataCollection")]
    public partial class WarehouseImportTransferModel
    {
        [Key]
        public Guid WarehouseImportTransferId { get; set; }
        [StringLength(50)]
        public string WeightHeadCode { get; set; }
        public Guid? WeightId { get; set; }
        [StringLength(50)]
        public string WeightVote { get; set; }
        public Guid? MaterialDocId { get; set; }
        [StringLength(50)]
        public string Reservation { get; set; }
        [StringLength(50)]
        public string ReservationItem { get; set; }
        [StringLength(50)]
        public string Customer { get; set; }
        [StringLength(50)]
        public string MaterialCode { get; set; }
        public long? MaterialCodeInt { get; set; }
        [StringLength(200)]
        public string MaterialName { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(200)]
        public string PlantName { get; set; }
        [StringLength(50)]
        public string SlocCode { get; set; }
        [StringLength(200)]
        public string SlocName { get; set; }
        [StringLength(50)]
        public string ReceivingSlocCode { get; set; }
        [StringLength(200)]
        public string ReceivingSlocName { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [StringLength(10)]
        public string MovementType { get; set; }
        [StringLength(10)]
        public string StockType { get; set; }
        public Guid? TruckInfoId { get; set; }
        [StringLength(50)]
        public string TruckNumber { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TotalQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DeliveryQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OpenQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmQty { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? NetWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GrossWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DocumentDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BagQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SingleWeight { get; set; }
        public int? QuantityWeitght { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? InputWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OutputWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GoodsWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityWithPackaging { get; set; }
        [StringLength(50)]
        public string VehicleCode { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(200)]
        public string Image { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(50)]
        public string MaterialDocument { get; set; }
        [StringLength(50)]
        public string ReverseDocument { get; set; }
        public bool? IsReverse { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("MaterialDocId")]
        [InverseProperty("WarehouseImportTransferModel")]
        public virtual MaterialDocumentModel MaterialDoc { get; set; }
        [ForeignKey("TruckInfoId")]
        [InverseProperty("WarehouseImportTransferModel")]
        public virtual TruckInfoModel TruckInfo { get; set; }
    }
}