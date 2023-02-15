﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("TransferDetailModel", Schema = "Warehouse")]
    public partial class TransferDetailModel
    {
        [Key]
        public Guid TransferDetailId { get; set; }
        public Guid? TransferId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? FromStockId { get; set; }
        public Guid? ToStockId { get; set; }
        public int? DateKey { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? UnitPrice { get; set; }
        [StringLength(500)]
        public string Note { get; set; }
        [StringLength(500)]
        public string ProductAttributes { get; set; }
        public Guid? FromCustomerReference { get; set; }
        public Guid? CustomerReference { get; set; }
        [StringLength(10)]
        public string StockRecevingType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FromTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ToTime { get; set; }
        public int? Phase { get; set; }
        [StringLength(50)]
        public string MovementType { get; set; }
        public Guid? DepartmentId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        public bool? isDeleted { get; set; }

        [ForeignKey("DateKey")]
        [InverseProperty("TransferDetailModel")]
        public virtual DimDateModel DateKeyNavigation { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("TransferDetailModel")]
        public virtual ProductModel1 Product { get; set; }
        [ForeignKey("ToStockId")]
        [InverseProperty("TransferDetailModel")]
        public virtual StockModel ToStock { get; set; }
    }
}