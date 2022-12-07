﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("CommandQCModel", Schema = "MESP2")]
    public partial class CommandQCModel
    {
        public CommandQCModel()
        {
            DetailQCModel = new HashSet<DetailQCModel>();
        }

        [Key]
        public Guid CommandQCId { get; set; }
        public int CommandQCCode { get; set; }
        public Guid? ReferenceDocumentId { get; set; }
        [StringLength(50)]
        public string WorkOrderCode { get; set; }
        public Guid? ProductId { get; set; }
        [StringLength(50)]
        public string QCType { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantitySample { get; set; }
        [StringLength(2000)]
        public string ImagePath { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Quantity { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityOk { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? QuantityErr { get; set; }
        public Guid? StepId { get; set; }
        [StringLength(50)]
        public string ResultQC { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        public Guid? StaffQC { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Active { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? QCTime { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("CommandQCModel")]
        public virtual ProductModel1 Product { get; set; }
        [ForeignKey("StepId")]
        [InverseProperty("CommandQCModel")]
        public virtual RoutingModel Step { get; set; }
        [InverseProperty("CommandQC")]
        public virtual ICollection<DetailQCModel> DetailQCModel { get; set; }
    }
}