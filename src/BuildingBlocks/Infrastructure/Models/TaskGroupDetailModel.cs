﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("TaskGroupDetailModel", Schema = "Task")]
    public partial class TaskGroupDetailModel
    {
        [Key]
        public Guid GroupId { get; set; }
        [Key]
        public Guid AccountId { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
    }
}