﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("SpecificationsModel", Schema = "tSale")]
    public partial class SpecificationsModel
    {
        [Key]
        public Guid SpecificationsId { get; set; }
        [Required]
        [StringLength(50)]
        public string SpecificationsCode { get; set; }
        [Required]
        [StringLength(100)]
        public string SpecificationsName { get; set; }
        public int? OrderIndex { get; set; }
        public bool Actived { get; set; }
    }
}