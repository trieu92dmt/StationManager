﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("PrognosisModel", Schema = "tMasterData")]
    public partial class PrognosisModel
    {
        [Key]
        public Guid PrognosisId { get; set; }
        [Required]
        [StringLength(100)]
        public string PrognosisDescription { get; set; }
    }
}