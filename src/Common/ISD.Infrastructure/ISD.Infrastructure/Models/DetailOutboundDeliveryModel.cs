﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("DetailOutboundDeliveryModel", Schema = "DataCollection")]
    public partial class DetailOutboundDeliveryModel
    {
        public DetailOutboundDeliveryModel()
        {
            GoodsReturnModel = new HashSet<GoodsReturnModel>();
        }

        [Key]
        public Guid DetailOutboundDeliveryId { get; set; }
        public Guid? OutboundDeliveryId { get; set; }
        [StringLength(50)]
        public string OutboundDeliveryItem { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(50)]
        public string StorageLocation { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DeliveryQuantity { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PickedQuantityPUoM { get; set; }
        [StringLength(50)]
        public string SalesUnit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? NetWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GrossWeight { get; set; }
        [StringLength(50)]
        public string WeightUnit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ActualDeliveryQty { get; set; }
        [StringLength(50)]
        public string ItemDescription { get; set; }
        [StringLength(50)]
        public string ReferenceDocument1 { get; set; }
        [StringLength(50)]
        public string ReferenceItem { get; set; }
        [StringLength(50)]
        public string SalesOffice { get; set; }
        [StringLength(50)]
        public string SalesGroup { get; set; }
        [StringLength(50)]
        public string DivisionCode { get; set; }
        [StringLength(50)]
        public string Order { get; set; }
        [StringLength(50)]
        public string OrderItem { get; set; }
        [StringLength(50)]
        public string SalesOrder { get; set; }
        [StringLength(50)]
        public string SalesOrderItem { get; set; }
        [StringLength(50)]
        public string ReferenceDocument2 { get; set; }
        [StringLength(50)]
        public string ReferenceDocItem { get; set; }
        [StringLength(50)]
        public string GoodsMvmntControl { get; set; }
        [StringLength(50)]
        public string DeliveryCompleted { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OriginalQuantity { get; set; }
        [StringLength(50)]
        public string ItemNumberDocument { get; set; }
        [StringLength(50)]
        public string OverallStatus { get; set; }
        [StringLength(50)]
        public string PickingStatus { get; set; }
        [StringLength(50)]
        public string Item { get; set; }
        [StringLength(50)]
        public string PickingPutawayItem { get; set; }
        [StringLength(50)]
        public string DeliveryItem { get; set; }
        [StringLength(50)]
        public string GoodsMvtItem { get; set; }
        [StringLength(50)]
        public string GoodsMovementSts { get; set; }
        [StringLength(50)]
        public string DistributionChannel { get; set; }

        [ForeignKey("OutboundDeliveryId")]
        [InverseProperty("DetailOutboundDeliveryModel")]
        public virtual OutboundDeliveryModel OutboundDelivery { get; set; }
        [InverseProperty("DetailOD")]
        public virtual ICollection<GoodsReturnModel> GoodsReturnModel { get; set; }
    }
}