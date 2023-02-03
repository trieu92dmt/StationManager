﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("TruckInfoModel", Schema = "DataCollection")]
    public partial class TruckInfoModel
    {
        [Key]
        public Guid TruckInfoId { get; set; }
        [StringLength(50)]
        public string TruckInfoCode { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(10)]
        public string TruckNumber { get; set; }
        [StringLength(50)]
        public string Driver { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? InputWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}