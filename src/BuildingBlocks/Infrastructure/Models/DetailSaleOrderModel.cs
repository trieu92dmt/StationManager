﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("DetailSaleOrderModel", Schema = "MESP2")]
    public partial class DetailSaleOrderModel
    {
        [Key]
        public Guid DetailSaleOrderId { get; set; }
        public Guid? SaleOrderHeaderId { get; set; }
        [StringLength(50)]
        public string SOItem { get; set; }
        public Guid? WorkOderId { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(200)]
        public string Desciption { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Quantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SampleQuantity { get; set; }
        [StringLength(50)]
        public string InforCard { get; set; }
        [StringLength(50)]
        public string ProductStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Trandate { get; set; }
        [StringLength(50)]
        public string OrderStatus { get; set; }
        [StringLength(2000)]
        public string SONote { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Shipdate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("SaleOrderHeaderId")]
        [InverseProperty("DetailSaleOrderModel")]
        public virtual HeaderSaleOrderModel SaleOrderHeader { get; set; }
    }
}