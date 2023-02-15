﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("SaleOrderDetailModel", Schema = "ghSale")]
    public partial class SaleOrderDetailModel
    {
        [Key]
        public Guid SaleOrderDetailId { get; set; }
        public Guid? SaleOrderMasterId { get; set; }
        [StringLength(50)]
        public string AccessoryCode { get; set; }
        [StringLength(50)]
        public string AccessoryName { get; set; }
        [StringLength(50)]
        public string AccessoryTypeCode { get; set; }
        [StringLength(20)]
        public string Unit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? UnitPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalPrice { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(50)]
        public string WarehouseCode { get; set; }
        [StringLength(200)]
        public string WarehouseName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DiscountAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DiscountPercent { get; set; }
        public int? Number { get; set; }

        [ForeignKey("SaleOrderMasterId")]
        [InverseProperty("SaleOrderDetailModel")]
        public virtual SaleOrderMasterModel SaleOrderMaster { get; set; }
    }
}