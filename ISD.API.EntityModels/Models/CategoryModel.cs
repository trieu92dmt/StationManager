﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("CategoryModel", Schema = "tSale")]
    public partial class CategoryModel
    {
        public CategoryModel()
        {
            InverseParentCategory = new HashSet<CategoryModel>();
            ProductModelCategory = new HashSet<ProductModel>();
            ProductModelCategoryDetail = new HashSet<ProductModel>();
            ProductModelParentCategory = new HashSet<ProductModel>();
        }

        [Key]
        public Guid CategoryId { get; set; }
        [Required]
        [StringLength(50)]
        public string CategoryCode { get; set; }
        [Required]
        [StringLength(500)]
        public string CategoryName { get; set; }
        public Guid? CategoryTypeId { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int? OrderIndex { get; set; }
        public bool Actived { get; set; }
        [StringLength(100)]
        public string ImageUrl { get; set; }
        public Guid? CompanyId { get; set; }
        public bool? IsTrackTrend { get; set; }
        public int? ProductTypeId { get; set; }
        [StringLength(500)]
        public string ADN { get; set; }

        [ForeignKey("ParentCategoryId")]
        [InverseProperty("InverseParentCategory")]
        public virtual CategoryModel ParentCategory { get; set; }
        [InverseProperty("ParentCategory")]
        public virtual ICollection<CategoryModel> InverseParentCategory { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<ProductModel> ProductModelCategory { get; set; }
        [InverseProperty("CategoryDetail")]
        public virtual ICollection<ProductModel> ProductModelCategoryDetail { get; set; }
        [InverseProperty("ParentCategory")]
        public virtual ICollection<ProductModel> ProductModelParentCategory { get; set; }
    }
}