﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("WorkFlowModel", Schema = "Task")]
    public partial class WorkFlowModel
    {
        public WorkFlowModel()
        {
            StatusTransitionModel = new HashSet<StatusTransitionModel>();
            TaskStatusModel = new HashSet<TaskStatusModel>();
        }

        [Key]
        public Guid WorkFlowId { get; set; }
        [StringLength(50)]
        public string WorkFlowCode { get; set; }
        [Required]
        [StringLength(200)]
        public string WorkFlowName { get; set; }
        [StringLength(200)]
        public string ImageUrl { get; set; }
        public int? OrderIndex { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
        [StringLength(50)]
        public string WorkflowCategoryCode { get; set; }
        [StringLength(50)]
        public string CompanyCode { get; set; }
        public bool? IsDisabledSummary { get; set; }

        [InverseProperty("WorkFlow")]
        public virtual ICollection<StatusTransitionModel> StatusTransitionModel { get; set; }
        [InverseProperty("WorkFlow")]
        public virtual ICollection<TaskStatusModel> TaskStatusModel { get; set; }
    }
}