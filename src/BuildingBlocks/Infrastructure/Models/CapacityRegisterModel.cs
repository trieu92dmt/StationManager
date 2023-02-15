﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("CapacityRegisterModel", Schema = "tMasterData")]
    public partial class CapacityRegisterModel
    {
        [Key]
        public Guid CapacityRegisterId { get; set; }
        [StringLength(50)]
        public string WorkShopCode { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Capacity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
    }
}