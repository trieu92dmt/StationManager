﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("JobQueue", Schema = "TTF_MES")]
    public partial class JobQueue
    {
        [Key]
        public int Id { get; set; }
        public long JobId { get; set; }
        [Key]
        [StringLength(50)]
        public string Queue { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FetchedAt { get; set; }
    }
}