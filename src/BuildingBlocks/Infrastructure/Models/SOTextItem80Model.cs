﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("SOTextItem80Model", Schema = "MES")]
    public partial class SOTextItem80Model
    {
        [Key]
        public Guid SOTextItem80Id { get; set; }
        [StringLength(50)]
        public string SO { get; set; }
        [StringLength(10)]
        public string SO_LINE { get; set; }
        [StringLength(10)]
        public string OBJECT { get; set; }
        [StringLength(10)]
        public string TEXT_ID { get; set; }
        [StringLength(500)]
        public string ID_NAME { get; set; }
        [StringLength(4000)]
        public string LONGTEXT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}