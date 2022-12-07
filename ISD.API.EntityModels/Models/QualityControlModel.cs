﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("QualityControlModel", Schema = "MES")]
    public partial class QualityControlModel
    {
        public QualityControlModel()
        {
            QualityControl_FileAttachment_Mapping = new HashSet<QualityControl_FileAttachment_Mapping>();
        }

        [Key]
        public Guid QualityControlId { get; set; }
        public int QualityControlCode { get; set; }
        [StringLength(50)]
        public string SaleOrgCode { get; set; }
        [StringLength(50)]
        public string WorkShopCode { get; set; }
        [StringLength(50)]
        public string WorkCenterCode { get; set; }
        [StringLength(100)]
        public string VBELN { get; set; }
        [StringLength(200)]
        public string LSXDT { get; set; }
        [StringLength(50)]
        public string LSXSAP { get; set; }
        [StringLength(50)]
        public string ProductAttribute { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(500)]
        public string ProductName { get; set; }
        public Guid? CustomerReference { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ConfirmDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? QuantityConfirm { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? QualityDate { get; set; }
        public Guid? QualityChecker { get; set; }
        [StringLength(50)]
        public string QualityType { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? InspectionLotQuantity { get; set; }
        [StringLength(50)]
        public string PO { get; set; }
        [StringLength(100)]
        public string Environmental { get; set; }
        public string Descriptions { get; set; }
        public bool? Status { get; set; }
        [StringLength(50)]
        public string Result { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }

        [InverseProperty("QualityControl")]
        public virtual ICollection<QualityControl_FileAttachment_Mapping> QualityControl_FileAttachment_Mapping { get; set; }
    }
}