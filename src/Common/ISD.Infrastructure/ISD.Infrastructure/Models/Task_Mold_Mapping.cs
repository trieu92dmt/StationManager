﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("Task_Mold_Mapping", Schema = "Task")]
    public partial class Task_Mold_Mapping
    {
        [Key]
        public Guid MappingId { get; set; }
        public Guid? TaskId { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(50)]
        public string SerialNumber { get; set; }
    }
}