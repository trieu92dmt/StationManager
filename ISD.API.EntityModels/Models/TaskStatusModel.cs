﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("TaskStatusModel", Schema = "Task")]
    public partial class TaskStatusModel
    {
        public TaskStatusModel()
        {
            Kanban_TaskStatus_Mapping = new HashSet<Kanban_TaskStatus_Mapping>();
            StatusTransitionModelFromStatus = new HashSet<StatusTransitionModel>();
            StatusTransitionModelToStatus = new HashSet<StatusTransitionModel>();
        }

        [Key]
        public Guid TaskStatusId { get; set; }
        [Required]
        [StringLength(200)]
        public string TaskStatusName { get; set; }
        public Guid? WorkFlowId { get; set; }
        public int? OrderIndex { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
        [StringLength(50)]
        public string TaskStatusCode { get; set; }
        [StringLength(50)]
        public string ProcessCode { get; set; }
        [StringLength(200)]
        public string Category { get; set; }
        public int? PositionLeft { get; set; }
        public int? PositionRight { get; set; }

        [ForeignKey("WorkFlowId")]
        [InverseProperty("TaskStatusModel")]
        public virtual WorkFlowModel WorkFlow { get; set; }
        [InverseProperty("TaskStatus")]
        public virtual ICollection<Kanban_TaskStatus_Mapping> Kanban_TaskStatus_Mapping { get; set; }
        [InverseProperty("FromStatus")]
        public virtual ICollection<StatusTransitionModel> StatusTransitionModelFromStatus { get; set; }
        [InverseProperty("ToStatus")]
        public virtual ICollection<StatusTransitionModel> StatusTransitionModelToStatus { get; set; }
    }
}