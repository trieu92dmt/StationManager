﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("EquipmentCardModel", Schema = "MESP2")]
    public partial class EquipmentCardModel
    {
        public EquipmentCardModel()
        {
            CheckInOutModel = new HashSet<CheckInOutModel>();
        }

        [Key]
        public Guid EquipmentCardId { get; set; }
        [StringLength(50)]
        public string StepCode { get; set; }
        [StringLength(500)]
        public string BarcodePath { get; set; }
        [StringLength(50)]
        public string StatusEquipment { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        public int? OrderIndex { get; set; }
        [StringLength(500)]
        public string BarcodeName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FromTime { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Quantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityError { get; set; }
        [StringLength(50)]
        public string EquipmentStopReason { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }

        [InverseProperty("EquipmentCard")]
        public virtual ICollection<CheckInOutModel> CheckInOutModel { get; set; }
    }
}