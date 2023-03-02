﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("MaterialDocumentModel", Schema = "DataCollection")]
    public partial class MaterialDocumentModel
    {
        public MaterialDocumentModel()
        {
            WarehouseImportTransferModel = new HashSet<WarehouseImportTransferModel>();
        }

        [Key]
        public Guid MaterialDocId { get; set; }
        [StringLength(50)]
        public string MaterialDocCode { get; set; }
        [StringLength(50)]
        public string MaterialDocItem { get; set; }
        [StringLength(50)]
        public string Identification { get; set; }
        [StringLength(50)]
        public string ParentLineID { get; set; }
        [StringLength(50)]
        public string HierarchyLevel { get; set; }
        [StringLength(50)]
        public string OriginalLineItem { get; set; }
        [StringLength(50)]
        public string MovementType { get; set; }
        [StringLength(10)]
        public string ItemAutoCreated { get; set; }
        [StringLength(50)]
        public string MaterialCode { get; set; }
        public long? MaterialCodeInt { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(50)]
        public string StorageLocation { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [StringLength(50)]
        public string StockType { get; set; }
        [StringLength(50)]
        public string BatchRestricted { get; set; }
        [StringLength(10)]
        public string SpecialStock { get; set; }
        [StringLength(50)]
        public string Supplier { get; set; }
        [StringLength(50)]
        public string Customer { get; set; }
        [StringLength(50)]
        public string SalesOrder { get; set; }
        [StringLength(50)]
        public string SalesOrderItem { get; set; }
        [StringLength(50)]
        public string SalesOrderSchedule { get; set; }
        [StringLength(20)]
        public string DebitCredit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Quantity { get; set; }
        [StringLength(20)]
        public string BaseUOM { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QtyUnitOfEntry { get; set; }
        [StringLength(20)]
        public string UnitOfEntry { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QtyOPU { get; set; }
        [StringLength(20)]
        public string OPUnit { get; set; }
        [StringLength(50)]
        public string PurchaseOrder { get; set; }
        [StringLength(50)]
        public string Item { get; set; }
        [StringLength(50)]
        public string ReferenceDocItem { get; set; }
        [StringLength(10)]
        public string MaterialDocYear { get; set; }
        [StringLength(50)]
        public string MaterialDoc2 { get; set; }
        [StringLength(50)]
        public string MaterialDocItem2 { get; set; }
        [StringLength(20)]
        public string DeliveryCompleted { get; set; }
        [StringLength(50)]
        public string Text { get; set; }
        [StringLength(50)]
        public string Reservation { get; set; }
        [StringLength(50)]
        public string ItemReservation { get; set; }
        [StringLength(50)]
        public string FinalIssue { get; set; }
        [StringLength(50)]
        public string ReceivingMaterial { get; set; }
        [StringLength(50)]
        public string ReceivingPlant { get; set; }
        [StringLength(50)]
        public string ReceivingSloc { get; set; }
        [StringLength(50)]
        public string ReceivingBatch { get; set; }
        [StringLength(10)]
        public string MovementIndicator { get; set; }
        [StringLength(10)]
        public string ReceiptIndicator { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QtyOrderUnit { get; set; }
        [StringLength(20)]
        public string OrderUnit { get; set; }
        [StringLength(50)]
        public string Supplier2 { get; set; }
        [StringLength(50)]
        public string SalesOrder2 { get; set; }
        [StringLength(50)]
        public string SalesOrderItem2 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PostingDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EntryDateTime { get; set; }
        [StringLength(50)]
        public string Reference { get; set; }
        [StringLength(50)]
        public string TransactionCode { get; set; }
        [StringLength(50)]
        public string Delivery { get; set; }
        [StringLength(50)]
        public string Item2 { get; set; }
        [StringLength(50)]
        public string CompletedIndicator { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [InverseProperty("MaterialDoc")]
        public virtual ICollection<WarehouseImportTransferModel> WarehouseImportTransferModel { get; set; }
    }
}