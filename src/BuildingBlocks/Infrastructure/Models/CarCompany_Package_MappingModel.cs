﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("CarCompany_Package_MappingModel", Schema = "masterdata")]
    public partial class CarCompany_Package_MappingModel
    {
        [Key]
        public Guid CarCompany_Package_MappingId { get; set; }
        public Guid? CarCompanyId { get; set; }
        public Guid? PackageId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireTime { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("CarCompanyId")]
        [InverseProperty("CarCompany_Package_MappingModel")]
        public virtual CarCompanyModel CarCompany { get; set; }
        [ForeignKey("PackageId")]
        [InverseProperty("CarCompany_Package_MappingModel")]
        public virtual PackageModel Package { get; set; }
    }
}