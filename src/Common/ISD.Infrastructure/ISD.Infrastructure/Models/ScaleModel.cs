﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("ScaleModel", Schema = "DataCollection")]
    public partial class ScaleModel
    {
        [Key]
        public Guid ScaleId { get; set; }
        [StringLength(50)]
        public string ScaleName { get; set; }
        [StringLength(10)]
        public string ScaleType { get; set; }
        public bool? isCantai { get; set; }
    }
}