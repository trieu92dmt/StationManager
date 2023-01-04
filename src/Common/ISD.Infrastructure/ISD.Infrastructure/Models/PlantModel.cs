﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("PlantModel", Schema = "DataCollection")]
    public partial class PlantModel
    {
        [Key]
        public Guid PlantId { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(50)]
        public string PlantName { get; set; }
        [StringLength(50)]
        public string CustomerNoPlant { get; set; }
        [StringLength(50)]
        public string SupplierNoPlant { get; set; }
        [StringLength(50)]
        public string ShippingPoin { get; set; }
        [StringLength(50)]
        public string PurchasingOrgCode { get; set; }
        [StringLength(50)]
        public string SaleOrgCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}