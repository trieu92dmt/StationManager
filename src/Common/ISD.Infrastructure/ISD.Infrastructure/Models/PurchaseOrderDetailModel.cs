﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("PurchaseOrderDetailModel", Schema = "DataCollection")]
    public partial class PurchaseOrderDetailModel
    {
        public PurchaseOrderDetailModel()
        {
            GoodsReceiptModel = new HashSet<GoodsReceiptModel>();
        }

        [Key]
        public Guid PurchaseOrderDetailId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        [StringLength(50)]
        public string POLine { get; set; }
        public int? PoLinetInt { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(50)]
        public string StorageLocation { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OrderQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityReceived { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OpenQuantity { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }
        [StringLength(50)]
        public string DeletionInd { get; set; }
        [StringLength(50)]
        public string Deliver { get; set; }
        [StringLength(50)]
        public string VehicleCode { get; set; }
        [StringLength(50)]
        public string VehicleOwner { get; set; }
        [StringLength(50)]
        public string TransportUnit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? GrossWeight { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? NetWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
        [StringLength(50)]
        public string DeliveryCompleted { get; set; }

        [ForeignKey("PurchaseOrderId")]
        [InverseProperty("PurchaseOrderDetailModel")]
        public virtual PurchaseOrderMasterModel PurchaseOrder { get; set; }
        [InverseProperty("PurchaseOrderDetail")]
        public virtual ICollection<GoodsReceiptModel> GoodsReceiptModel { get; set; }
    }
}