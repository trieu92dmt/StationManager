﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("DetailWorkOrderModel", Schema = "DataCollection")]
    public partial class DetailWorkOrderModel
    {
        public DetailWorkOrderModel()
        {
            IssueForProductionModel = new HashSet<IssueForProductionModel>();
            ScrapFromProductionModel = new HashSet<ScrapFromProductionModel>();
        }

        [Key]
        public Guid DetailWorkOrderId { get; set; }
        public Guid? WorkOrderId { get; set; }
        [StringLength(50)]
        public string WorkOrderItem { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        public long? ProductCodeInt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RequirementDate { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal RequirementQuantiy { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal QuantityWithdrawn { get; set; }
        [StringLength(50)]
        public string BaseUnit1 { get; set; }
        [StringLength(50)]
        public string BaseUnit2 { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [StringLength(50)]
        public string Activity { get; set; }
        [StringLength(50)]
        public string Reservation { get; set; }
        [StringLength(50)]
        public string ReservationItem { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OpenQuantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Shortage { get; set; }
        [StringLength(50)]
        public string StorageLocation { get; set; }
        [StringLength(50)]
        public string IconMessType { get; set; }
        [StringLength(50)]
        public string SystemStatus { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ConfirmedQty { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityFixed1 { get; set; }
        [StringLength(50)]
        public string PurchasingDoc { get; set; }
        [StringLength(50)]
        public string PurchasingDocItem { get; set; }
        [StringLength(50)]
        public string Supplier { get; set; }
        [StringLength(50)]
        public string MovementType { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityFixed2 { get; set; }
        [StringLength(50)]
        public string FinalIssue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("WorkOrderId")]
        [InverseProperty("DetailWorkOrderModel")]
        public virtual WorkOrderModel WorkOrder { get; set; }
        [InverseProperty("DetailWorkOrder")]
        public virtual ICollection<IssueForProductionModel> IssueForProductionModel { get; set; }
        [InverseProperty("DetailWorkOrder")]
        public virtual ICollection<ScrapFromProductionModel> ScrapFromProductionModel { get; set; }
    }
}