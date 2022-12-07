﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("QualityControl_Error_Mapping", Schema = "MES")]
    public partial class QualityControl_Error_Mapping
    {
        [Key]
        public Guid QuanlityControl_Error_Id { get; set; }
        public Guid? QualityControlId { get; set; }
        [StringLength(50)]
        public string CatalogCode { get; set; }
        [StringLength(100)]
        public string Notes { get; set; }
        [StringLength(100)]
        public string LevelError { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? QuantityError { get; set; }
    }
}