﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("KanbanModel", Schema = "tMasterData")]
    public partial class KanbanModel
    {
        public KanbanModel()
        {
            KanbanDetailModel = new HashSet<KanbanDetailModel>();
        }

        [Key]
        public Guid KanbanId { get; set; }
        [StringLength(20)]
        public string KanbanCode { get; set; }
        [StringLength(200)]
        public string KanbanName { get; set; }
        public bool? Actived { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public int? OrderIndex { get; set; }

        [InverseProperty("Kanban")]
        public virtual ICollection<KanbanDetailModel> KanbanDetailModel { get; set; }
    }
}