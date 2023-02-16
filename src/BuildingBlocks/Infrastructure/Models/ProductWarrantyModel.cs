﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("ProductWarrantyModel", Schema = "Maintenance")]
    public partial class ProductWarrantyModel
    {
        [Key]
        public Guid ProductWarrantyId { get; set; }
        public int ProductWarrantyCode { get; set; }
        public Guid ProfileId { get; set; }
        public Guid ProductId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime FromDate { get; set; }
        public Guid WarrantyId { get; set; }
        [StringLength(100)]
        public string SerriNo { get; set; }
        [StringLength(100)]
        public string ProductWarrantyNo { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ActivatedQuantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ToDate { get; set; }
        [StringLength(50)]
        public string SaleOrder { get; set; }
        [StringLength(50)]
        public string OrderDelivery { get; set; }
        [StringLength(50)]
        public string ERPProductCode { get; set; }
        [StringLength(1000)]
        public string ProfileName { get; set; }
        [StringLength(1000)]
        public string ProfileShortName { get; set; }
        [StringLength(50)]
        public string Age { get; set; }
        [StringLength(500)]
        public string Phone { get; set; }
        [StringLength(500)]
        public string Email { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        [StringLength(4000)]
        public string Address { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        [StringLength(4000)]
        public string Note { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductWarrantyModel")]
        public virtual ProductModel1 Product { get; set; }
        [ForeignKey("ProfileId")]
        [InverseProperty("ProductWarrantyModel")]
        public virtual ProfileModel Profile { get; set; }
        [ForeignKey("WarrantyId")]
        [InverseProperty("ProductWarrantyModel")]
        public virtual WarrantyModel Warranty { get; set; }
    }
}