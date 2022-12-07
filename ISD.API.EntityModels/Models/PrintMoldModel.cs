﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("PrintMoldModel", Schema = "ghMasterData")]
    public partial class PrintMoldModel
    {
        [Key]
        public Guid PrintMoldId { get; set; }
        public int PrintMoldIntId { get; set; }
        [StringLength(50)]
        public string PrintMoldCode { get; set; }
        [StringLength(100)]
        public string PrintMoldName { get; set; }
        [StringLength(50)]
        public string PrintMoldType { get; set; }
        public Guid ProfileId { get; set; }
        public Guid ProductId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Specifications_Length { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Specifications_Width { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Specifications_Height { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Specifications_Overalls { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Specifications_Side { get; set; }
        public int? ProductPerMold { get; set; }
        [StringLength(50)]
        public string LocationNote { get; set; }
        [StringLength(100)]
        public string PrintMoldFilm { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PrintMoldDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastMaintenanceDate { get; set; }
        public int? MaintenanceAlert { get; set; }
        public int? StampQuantity { get; set; }
        public int? CurrentStampeQuantity { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Serial { get; set; }
        [StringLength(50)]
        public string Bin { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public int? StampQuantityAlert { get; set; }
        [StringLength(50)]
        public string ProfileName { get; set; }
    }
}