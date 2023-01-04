﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("ContRegisterSO60Model", Schema = "tMasterData")]
    public partial class ContRegisterSO60Model
    {
        [Key]
        public Guid ContRegister60Id { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        [StringLength(500)]
        public string Customer { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Cont { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
    }
}