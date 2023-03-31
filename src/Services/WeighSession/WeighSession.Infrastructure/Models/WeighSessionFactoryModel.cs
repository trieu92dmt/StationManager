﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WeighSession.Infrastructure.Models
{
    public partial class WeighSessionFactoryModel
    {
        public WeighSessionFactoryModel()
        {
            WeighSessionDetailFactoryModel = new HashSet<WeighSessionDetailFactoryModel>();
        }

        [Key]
        [StringLength(50)]
        public string WeighSessionCode { get; set; }
        [StringLength(50)]
        public string ScaleCode { get; set; }
        [StringLength(50)]
        public string DateKey { get; set; }
        public int? OrderIndex { get; set; }
        public int? TotalNumberOfWeigh { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TotalWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public int? SessionCheck { get; set; }

        [InverseProperty("WeighSessionCodeNavigation")]
        public virtual ICollection<WeighSessionDetailFactoryModel> WeighSessionDetailFactoryModel { get; set; }
    }
}