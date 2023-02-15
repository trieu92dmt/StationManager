﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("TestTargetModel", Schema = "tMasterData")]
    public partial class TestTargetModel
    {
        public TestTargetModel()
        {
            DetailQCModel = new HashSet<DetailQCModel>();
        }

        [Key]
        public Guid TestTargetId { get; set; }
        public int TargetCode { get; set; }
        [StringLength(50)]
        public string TargetName { get; set; }
        [StringLength(50)]
        public string StepCode { get; set; }
        public bool? IsQualityIndiCation { get; set; }
        public Guid? StepId { get; set; }
        [StringLength(50)]
        public string Tolerance { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("StepId")]
        [InverseProperty("TestTargetModel")]
        public virtual RoutingModel Step { get; set; }
        [InverseProperty("TestTarget")]
        public virtual ICollection<DetailQCModel> DetailQCModel { get; set; }
    }
}