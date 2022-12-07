﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("QualityControlInformationModel", Schema = "tMasterData")]
    public partial class QualityControlInformationModel
    {
        public QualityControlInformationModel()
        {
            WorkCenterCode = new HashSet<WorkCenterModel>();
        }

        [Key]
        public Guid Id { get; set; }
        public int Code { get; set; }
        [StringLength(50)]
        public string SaleOrgCode { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public int? OrderIndex { get; set; }
        public bool? Actived { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }

        [ForeignKey("QualityControlInformationId")]
        [InverseProperty("QualityControlInformation")]
        public virtual ICollection<WorkCenterModel> WorkCenterCode { get; set; }
    }
}