﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("PhysicsWorkShopModel", Schema = "tMasterData")]
    public partial class PhysicsWorkShopModel
    {
        [Key]
        public Guid PhysicsWorkShopId { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(50)]
        public string PhysicsWorkShopCode { get; set; }
        [StringLength(500)]
        public string PhysicsWorkShopName { get; set; }
        public int? OrderIndex { get; set; }
        public bool? Actived { get; set; }
    }
}