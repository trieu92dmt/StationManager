﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("AccessorySellTypeModel", Schema = "ghSale")]
    public partial class AccessorySellTypeModel
    {
        [Key]
        public Guid AccessorySellTypeId { get; set; }
        [StringLength(50)]
        public string AccessorySellTypeCode { get; set; }
        [StringLength(200)]
        public string AccessorySellTypeName { get; set; }
        public Guid? ServiceFlagId { get; set; }
        public bool? Actived { get; set; }
        public bool? IsTinhTien { get; set; }
    }
}