﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    public partial class ApplicationLog
    {
        [Key]
        public Guid ApplicationLogId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PerformedAt { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public Guid? PerformedBy_AccountId { get; set; }

        [ForeignKey("PerformedBy_AccountId")]
        [InverseProperty("ApplicationLog")]
        public virtual AccountModel PerformedBy_Account { get; set; }
    }
}