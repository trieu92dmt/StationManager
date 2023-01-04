﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("SO100TextModel", Schema = "MES")]
    public partial class SO100TextModel
    {
        [Key]
        public Guid SO100TextId { get; set; }
        [StringLength(50)]
        public string SONumber { get; set; }
        [StringLength(50)]
        public string OBJECT { get; set; }
        [StringLength(10)]
        public string TEXT_ID { get; set; }
        [StringLength(50)]
        public string ID_NAME { get; set; }
        public string LONGTEXT { get; set; }
        [StringLength(10)]
        public string TDID { get; set; }
        public string LINES { get; set; }
        [StringLength(1000)]
        public string H001 { get; set; }
        [StringLength(1000)]
        public string H003 { get; set; }
        [StringLength(1000)]
        public string H004 { get; set; }
        [StringLength(1000)]
        public string H005 { get; set; }
        [StringLength(1000)]
        public string H006 { get; set; }
        [StringLength(1000)]
        public string H007 { get; set; }
        [StringLength(1000)]
        public string H008 { get; set; }
        [StringLength(1000)]
        public string H010 { get; set; }
        [StringLength(1000)]
        public string H011 { get; set; }
        [StringLength(1000)]
        public string H012 { get; set; }
        [StringLength(1000)]
        public string H013 { get; set; }
        [StringLength(1000)]
        public string H015 { get; set; }
        [StringLength(1000)]
        public string H016 { get; set; }
        [StringLength(1000)]
        public string H017 { get; set; }
        [StringLength(1000)]
        public string H018 { get; set; }
        [StringLength(1000)]
        public string H019 { get; set; }
        [StringLength(1000)]
        public string H020 { get; set; }
        [StringLength(1000)]
        public string H021 { get; set; }
        [StringLength(1000)]
        public string H022 { get; set; }
        [StringLength(1000)]
        public string H023 { get; set; }
        [StringLength(1000)]
        public string H024 { get; set; }
        [StringLength(1000)]
        public string H025 { get; set; }
        [StringLength(1000)]
        public string H026 { get; set; }
        [StringLength(1000)]
        public string H056 { get; set; }
        [StringLength(1000)]
        public string H057 { get; set; }
        [StringLength(1000)]
        public string H060 { get; set; }
        [StringLength(1000)]
        public string H074 { get; set; }
        [StringLength(1000)]
        public string H075 { get; set; }
        [StringLength(1000)]
        public string H076 { get; set; }
        [StringLength(1000)]
        public string H077 { get; set; }
        [StringLength(1000)]
        public string H078 { get; set; }
        [StringLength(1000)]
        public string H079 { get; set; }
        [StringLength(1000)]
        public string H080 { get; set; }
        [StringLength(1000)]
        public string H081 { get; set; }
        [StringLength(1000)]
        public string H082 { get; set; }
        [StringLength(1000)]
        public string H083 { get; set; }
        [StringLength(1000)]
        public string H084 { get; set; }
        [StringLength(1000)]
        public string H085 { get; set; }
        [StringLength(1000)]
        public string H086 { get; set; }
        [StringLength(1000)]
        public string H087 { get; set; }
        [StringLength(1000)]
        public string H088 { get; set; }
        [StringLength(1000)]
        public string H091 { get; set; }
        [StringLength(1000)]
        public string H092 { get; set; }
        [StringLength(1000)]
        public string H093 { get; set; }
        [StringLength(1000)]
        public string I001 { get; set; }
        [StringLength(1000)]
        public string I002 { get; set; }
        [StringLength(1000)]
        public string I003 { get; set; }
        [StringLength(1000)]
        public string I004 { get; set; }
        [StringLength(1000)]
        public string I005 { get; set; }
        [StringLength(1000)]
        public string Q001 { get; set; }
        [StringLength(1000)]
        public string I023 { get; set; }
        [StringLength(1000)]
        public string I021 { get; set; }
        [StringLength(1000)]
        public string I022 { get; set; }
        [StringLength(1000)]
        public string I024 { get; set; }
        [StringLength(1000)]
        public string I025 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}