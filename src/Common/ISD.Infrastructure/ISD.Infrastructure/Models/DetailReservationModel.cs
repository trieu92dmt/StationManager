﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("DetailReservationModel", Schema = "DataCollection")]
    public partial class DetailReservationModel
    {
        public DetailReservationModel()
        {
            WarehouseTransferModel = new HashSet<WarehouseTransferModel>();
        }

        [Key]
        public Guid DetailReservationId { get; set; }
        public Guid? ReservationId { get; set; }
        [StringLength(50)]
        public string ReservationItem { get; set; }
        [StringLength(10)]
        public string ItemDeleted { get; set; }
        [StringLength(10)]
        public string MovementAllowed { get; set; }
        [StringLength(10)]
        public string MissingPart { get; set; }
        [StringLength(50)]
        public string Material { get; set; }
        [StringLength(50)]
        public string Batch { get; set; }
        [StringLength(50)]
        public string SpecialStock { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RequirementsDate { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? RequirementQty { get; set; }
        [StringLength(50)]
        public string BaseUnit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QtyIsFixed { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QtyWithdrawn { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QtyInUnitOfEntry { get; set; }
        [StringLength(50)]
        public string UnitOfEntry { get; set; }
        [StringLength(50)]
        public string PlannedOrder { get; set; }
        [StringLength(50)]
        public string PurchaseRequisition { get; set; }
        [StringLength(50)]
        public string ItemOfRequisition { get; set; }
        [StringLength(50)]
        public string Order { get; set; }
        [StringLength(50)]
        public string PeggedRequirement { get; set; }
        [StringLength(50)]
        public string SalesOrder { get; set; }
        [StringLength(50)]
        public string SalesOrderItem { get; set; }
        [StringLength(50)]
        public string MovementType { get; set; }
        [StringLength(50)]
        public string PurchasingDoc { get; set; }
        [StringLength(50)]
        public string Item { get; set; }
        [StringLength(10)]
        public string MaterialOrigin { get; set; }
        [StringLength(50)]
        public string MaterialGr { get; set; }
        [StringLength(50)]
        public string ReceivingBatch { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("ReservationId")]
        [InverseProperty("DetailReservationModel")]
        public virtual ReservationModel Reservation { get; set; }
        [InverseProperty("DetailReservation")]
        public virtual ICollection<WarehouseTransferModel> WarehouseTransferModel { get; set; }
    }
}