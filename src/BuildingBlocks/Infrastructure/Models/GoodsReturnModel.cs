﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("GoodsReturnModel", Schema = "DataCollection")]
    public partial class GoodsReturnModel
    {
        [Key]
        public Guid GoodsReturnId { get; set; }
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
        public string SOType { get; set; }
        [StringLength(50)]
        public string SalesOrg { get; set; }
        [StringLength(50)]
        public string DistributionChannel { get; set; }
        [StringLength(50)]
        public string Division { get; set; }
        [StringLength(50)]
        public string SalesOrder { get; set; }
        [StringLength(50)]
        public string ShipToParty { get; set; }
        [StringLength(50)]
        public string ShipToPartyName { get; set; }
        [StringLength(50)]
        public string MaterialCode { get; set; }
        public long? MaterialCodeInt { get; set; }
        [StringLength(50)]
        public string PurchaseOrderCode { get; set; }
        [StringLength(50)]
        public string SlocCode { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal TotalQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal DeliveredQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal OpenQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmQty { get; set; }
        [StringLength(50)]
        public string UOM { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? NetWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GrossWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DocumentDate { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityWithPackaging { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        public int? QuantityWeitght { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BagQuantity { get; set; }
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
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? BagQuantity2 { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SingleWeight2 { get; set; }
        [StringLength(50)]
        public string MaterialDocument { get; set; }
        [StringLength(50)]
        public string MaterialDocumentItem { get; set; }
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
        [Column(TypeName = "datetime")]
        public DateTime? RecordTime1 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RecordTime2 { get; set; }
        public bool? IsReverse { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("DetailODId")]
        [InverseProperty("GoodsReturnModel")]
        public virtual DetailOutboundDeliveryModel DetailOD { get; set; }
    }
}