﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    public partial class provinces
    {
        public provinces()
        {
            districts = new HashSet<districts>();
        }

        [Key]
        [StringLength(20)]
        public string code { get; set; }
        [Required]
        [StringLength(255)]
        public string name { get; set; }
        [StringLength(255)]
        public string name_en { get; set; }
        [Required]
        [StringLength(255)]
        public string full_name { get; set; }
        [StringLength(255)]
        public string full_name_en { get; set; }
        [StringLength(255)]
        public string code_name { get; set; }
        public int? administrative_unit_id { get; set; }
        public int? administrative_region_id { get; set; }

        [ForeignKey("administrative_region_id")]
        [InverseProperty("provinces")]
        public virtual administrative_regions administrative_region { get; set; }
        [ForeignKey("administrative_unit_id")]
        [InverseProperty("provinces")]
        public virtual administrative_units administrative_unit { get; set; }
        [InverseProperty("province_codeNavigation")]
        public virtual ICollection<districts> districts { get; set; }
    }
}