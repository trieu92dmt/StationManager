﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("Task_Component_Mapping", Schema = "Task")]
    public partial class Task_Component_Mapping
    {
        [Key]
        public Guid MappingId { get; set; }
        public Guid? TaskId { get; set; }
        [StringLength(50)]
        public string ProductId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Qty { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }
    }
}