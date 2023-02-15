﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("ProductGroupModel", Schema = "DataCollection")]
    public partial class ProductGroupModel
    {
        [Key]
        public Guid ProductGroupId { get; set; }
        [StringLength(50)]
        public string ProductGroupCode { get; set; }
        [StringLength(50)]
        public string ProductGroupName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}