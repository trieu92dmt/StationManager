﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("RoutingSapModel", Schema = "MES")]
    public partial class RoutingSapModel
    {
        [Key]
        public Guid RoutingSapId { get; set; }
        [StringLength(10)]
        public string MANDT { get; set; }
        [StringLength(100)]
        public string MATNR { get; set; }
        [StringLength(50)]
        public string WERKS { get; set; }
        [StringLength(10)]
        public string PLNTY { get; set; }
        [StringLength(100)]
        public string PLNNR { get; set; }
        [StringLength(100)]
        public string PLNAL { get; set; }
        [StringLength(100)]
        public string ZKRIZ { get; set; }
        [StringLength(100)]
        public string ZAEHL { get; set; }
        [StringLength(100)]
        public string DATUV { get; set; }
        [StringLength(10)]
        public string LOEKZ { get; set; }
        [StringLength(100)]
        public string SUMNR { get; set; }
        [StringLength(50)]
        public string VORNR { get; set; }
        [StringLength(50)]
        public string KTSCH { get; set; }
        [StringLength(10)]
        public string ARBPL { get; set; }
        [StringLength(500)]
        public string LTXA1 { get; set; }
        [StringLength(50)]
        public string MEINH { get; set; }
        [StringLength(50)]
        public string UMREN { get; set; }
        [StringLength(50)]
        public string UMREZ { get; set; }
        [StringLength(100)]
        public string BMSCH { get; set; }
        [StringLength(50)]
        public string LAR01 { get; set; }
        [StringLength(50)]
        public string VGE01 { get; set; }
        [StringLength(50)]
        public string VGW01 { get; set; }
        [StringLength(50)]
        public string LAR02 { get; set; }
        [StringLength(50)]
        public string VGE02 { get; set; }
        [StringLength(50)]
        public string VGW02 { get; set; }
        [StringLength(50)]
        public string LAR03 { get; set; }
        [StringLength(50)]
        public string VGE03 { get; set; }
        [StringLength(50)]
        public string VGW03 { get; set; }
        [StringLength(50)]
        public string LAR04 { get; set; }
        [StringLength(50)]
        public string VGE04 { get; set; }
        [StringLength(50)]
        public string VGW04 { get; set; }
        [StringLength(50)]
        public string LAR05 { get; set; }
        [StringLength(50)]
        public string VGE05 { get; set; }
        [StringLength(50)]
        public string VGW05 { get; set; }
        [StringLength(50)]
        public string LAR06 { get; set; }
        [StringLength(50)]
        public string VGE06 { get; set; }
        [StringLength(50)]
        public string VGW06 { get; set; }
        [StringLength(50)]
        public string ZERMA { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}