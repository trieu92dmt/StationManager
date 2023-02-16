﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("RoutingModel", Schema = "MES")]
    public partial class RoutingModel
    {
        public RoutingModel()
        {
            CommandQCModel = new HashSet<CommandQCModel>();
            Department_Routing_Mapping = new HashSet<Department_Routing_Mapping>();
            TestTargetModel = new HashSet<TestTargetModel>();
        }

        [Key]
        public Guid StepId { get; set; }
        [StringLength(50)]
        public string StepCode { get; set; }
        [StringLength(100)]
        public string StepName { get; set; }
        [StringLength(100)]
        public string StepNameUnsigned { get; set; }
        public int? OrderIndex { get; set; }
        public bool? Actived { get; set; }
        public Guid? WorkShopId { get; set; }
        public int? NumberOfMachines { get; set; }
        [StringLength(50)]
        public string WorkCenter { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }

        [InverseProperty("Step")]
        public virtual ICollection<CommandQCModel> CommandQCModel { get; set; }
        [InverseProperty("Step")]
        public virtual ICollection<Department_Routing_Mapping> Department_Routing_Mapping { get; set; }
        [InverseProperty("Step")]
        public virtual ICollection<TestTargetModel> TestTargetModel { get; set; }
    }
}