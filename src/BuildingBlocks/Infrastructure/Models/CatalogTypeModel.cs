﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("CatalogTypeModel", Schema = "tMasterData")]
    public partial class CatalogTypeModel
    {
        [Key]
        [StringLength(100)]
        public string CatalogTypeCode { get; set; }
        [StringLength(100)]
        public string CatalogTypeName { get; set; }
        public bool? Actived { get; set; }
        [StringLength(20)]
        public string CategoryType { get; set; }
        [StringLength(200)]
        public string Note { get; set; }
    }
}