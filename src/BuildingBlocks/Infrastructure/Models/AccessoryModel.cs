﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("AccessoryModel", Schema = "tSale")]
    public partial class AccessoryModel
    {
        public AccessoryModel()
        {
            AccessoryDetailModel = new HashSet<AccessoryDetailModel>();
        }

        [Key]
        public Guid AccessoryId { get; set; }
        public Guid AccessoryCategoryId { get; set; }
        [Required]
        [StringLength(50)]
        public string AccessoryCode { get; set; }
        [Required]
        [StringLength(100)]
        public string AccessoryName { get; set; }
        public bool? isHelmet { get; set; }
        public bool? isHelmetAdult { get; set; }
        [StringLength(50)]
        public string Size { get; set; }
        [StringLength(100)]
        public string ImageUrl { get; set; }
        public int? OrderIndex { get; set; }
        public bool Actived { get; set; }

        [InverseProperty("Accessory")]
        public virtual ICollection<AccessoryDetailModel> AccessoryDetailModel { get; set; }
    }
}