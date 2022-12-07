﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("DivisionModel", Schema = "DataCollection")]
    public partial class DivisionModel
    {
        [Key]
        public Guid DivisionId { get; set; }
        [StringLength(50)]
        public string DivisionCode { get; set; }
        [StringLength(50)]
        public string SaleOrgCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}