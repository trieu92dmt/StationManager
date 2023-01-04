﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("QualityControlDetailModel", Schema = "MES")]
    public partial class QualityControlDetailModel
    {
        [Key]
        public Guid QualityControlDetailId { get; set; }
        public Guid? QualityControlId { get; set; }
        [StringLength(50)]
        public string TestMethod { get; set; }
        [StringLength(50)]
        public string SamplingLevel { get; set; }
        [StringLength(50)]
        public string AcceptableLevel { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? InspectionQuantity { get; set; }
        [StringLength(50)]
        public string Result { get; set; }
    }
}