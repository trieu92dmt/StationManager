﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("WeighSessionDetailModel", Schema = "DataCollection")]
    public partial class WeighSessionDetailModel
    {
        [Key]
        public Guid WeighSessionDetailID { get; set; }
        public Guid WeighSessionID { get; set; }
        public int? NumberOfWeigh { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DetailWeight { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? WeighTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("WeighSessionID")]
        [InverseProperty("WeighSessionDetailModel")]
        public virtual WeighSessionModel WeighSession { get; set; }
    }
}