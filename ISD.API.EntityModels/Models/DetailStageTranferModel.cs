﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("DetailStageTranferModel", Schema = "MESP2")]
    public partial class DetailStageTranferModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? StageTranferId { get; set; }
        [StringLength(50)]
        public string LotNumber { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Quantity { get; set; }
        public Guid? StockId { get; set; }
        [StringLength(50)]
        public string ProductionStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpirationDate { get; set; }

        [ForeignKey("StageTranferId")]
        [InverseProperty("DetailStageTranferModel")]
        public virtual StageTransferModel StageTranfer { get; set; }
        [ForeignKey("StockId")]
        [InverseProperty("DetailStageTranferModel")]
        public virtual StockModel Stock { get; set; }
    }
}