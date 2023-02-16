﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("OrderTypeModel", Schema = "DataCollection")]
    public partial class OrderTypeModel
    {
        [Key]
        public Guid OrderTypeId { get; set; }
        [StringLength(50)]
        public string OrderTypeCode { get; set; }
        [StringLength(50)]
        public string OrderTypeName { get; set; }
        [StringLength(30)]
        public string Category { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(50)]
        public string ShortText { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}