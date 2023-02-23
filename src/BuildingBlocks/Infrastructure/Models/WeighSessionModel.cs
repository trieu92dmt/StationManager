﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("WeighSessionModel", Schema = "DataCollection")]
    public partial class WeighSessionModel
    {
        public WeighSessionModel()
        {
            ExportByCommandModel = new HashSet<ExportByCommandModel>();
            GoodsReceiptTypeTModel = new HashSet<GoodsReceiptTypeTModel>();
            OtherExportModel = new HashSet<OtherExportModel>();
            OtherImportModel = new HashSet<OtherImportModel>();
            ScaleMonitorModel = new HashSet<ScaleMonitorModel>();
            WarehouseImportTransferModel = new HashSet<WarehouseImportTransferModel>();
            WeighSessionDetailModel = new HashSet<WeighSessionDetailModel>();
        }

        [Key]
        public Guid WeighSessionID { get; set; }
        public Guid ScaleId { get; set; }
        public int? TotalNumberOfWeigh { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TotalWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("ScaleId")]
        [InverseProperty("WeighSessionModel")]
        public virtual ScaleModel Scale { get; set; }
        [InverseProperty("WeightSession")]
        public virtual ICollection<ExportByCommandModel> ExportByCommandModel { get; set; }
        [InverseProperty("WeightSession")]
        public virtual ICollection<GoodsReceiptTypeTModel> GoodsReceiptTypeTModel { get; set; }
        [InverseProperty("WeightSession")]
        public virtual ICollection<OtherExportModel> OtherExportModel { get; set; }
        [InverseProperty("WeightSession")]
        public virtual ICollection<OtherImportModel> OtherImportModel { get; set; }
        [InverseProperty("WeightSession")]
        public virtual ICollection<ScaleMonitorModel> ScaleMonitorModel { get; set; }
        [InverseProperty("WeightNavigation")]
        public virtual ICollection<WarehouseImportTransferModel> WarehouseImportTransferModel { get; set; }
        [InverseProperty("WeighSession")]
        public virtual ICollection<WeighSessionDetailModel> WeighSessionDetailModel { get; set; }
    }
}