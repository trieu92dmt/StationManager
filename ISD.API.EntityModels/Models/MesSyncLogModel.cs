﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("MesSyncLogModel", Schema = "utilities")]
    public partial class MesSyncLogModel
    {
        [Key]
        public int LogId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LogTime { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        public string Description { get; set; }
    }
}