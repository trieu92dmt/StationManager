﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("CatalogTypeModel", Schema = "masterdata")]
    public partial class CatalogTypeModel
    {
        [Key]
        public Guid CatalogTypeId { get; set; }
        [StringLength(50)]
        public string CatalogTypeCode { get; set; }
        [StringLength(200)]
        public string CatalogTypeName { get; set; }
        [StringLength(200)]
        public string CatalogTypeName_en { get; set; }
        public bool? Actived { get; set; }
    }
}