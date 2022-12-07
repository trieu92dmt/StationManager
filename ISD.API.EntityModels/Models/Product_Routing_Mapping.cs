﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("Product_Routing_Mapping", Schema = "MES")]
    public partial class Product_Routing_Mapping
    {
        public Product_Routing_Mapping()
        {
            Product_Routing_Mold_Mapping = new HashSet<Product_Routing_Mold_Mapping>();
        }

        [Key]
        public Guid Product_Routing_MappingId { get; set; }
        [StringLength(50)]
        public string RoutingVersion { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        public int? OrderIndex { get; set; }
        [StringLength(50)]
        public string StepCode { get; set; }
        [StringLength(500)]
        public string ProductionGuide { get; set; }
        [StringLength(500)]
        public string ComponentUsed { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? EstimateComplete { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? SetupTime { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? RatedTime { get; set; }
        public int? ProductPerPage { get; set; }

        [InverseProperty("Product_Routing_Mapping")]
        public virtual ICollection<Product_Routing_Mold_Mapping> Product_Routing_Mold_Mapping { get; set; }
    }
}