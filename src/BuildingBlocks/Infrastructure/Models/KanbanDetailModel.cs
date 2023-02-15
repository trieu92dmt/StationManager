﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("KanbanDetailModel", Schema = "tMasterData")]
    public partial class KanbanDetailModel
    {
        public KanbanDetailModel()
        {
            Kanban_TaskStatus_Mapping = new HashSet<Kanban_TaskStatus_Mapping>();
        }

        [Key]
        public Guid KanbanDetailId { get; set; }
        public Guid? KanbanId { get; set; }
        [StringLength(200)]
        public string ColumnName { get; set; }
        public int? OrderIndex { get; set; }
        [StringLength(200)]
        public string Note { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }

        [ForeignKey("KanbanId")]
        [InverseProperty("KanbanDetailModel")]
        public virtual KanbanModel Kanban { get; set; }
        [InverseProperty("KanbanDetail")]
        public virtual ICollection<Kanban_TaskStatus_Mapping> Kanban_TaskStatus_Mapping { get; set; }
    }
}