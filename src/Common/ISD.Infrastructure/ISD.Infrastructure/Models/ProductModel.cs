﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("ProductModel", Schema = "DataCollection")]
    public partial class ProductModel
    {
        [Key]
        public Guid ProductId { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        public int? ProductCodeInt { get; set; }
        [StringLength(50)]
        public string ProductName { get; set; }
        [StringLength(50)]
        public string PlantCode { get; set; }
        [StringLength(50)]
        public string ProductGroupCode { get; set; }
        [StringLength(50)]
        public string ProductGroupDesc { get; set; }
        [StringLength(50)]
        public string PurchasingOrgCode { get; set; }
        [StringLength(50)]
        public string ProductTypeCode { get; set; }
        [StringLength(50)]
        public string ProductTypeName { get; set; }
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }
        [StringLength(50)]
        public string DivisionCode { get; set; }
        [StringLength(50)]
        public string SaleOrgCode { get; set; }
        [StringLength(50)]
        public string DistributionChannelCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Activce { get; set; }
    }
}