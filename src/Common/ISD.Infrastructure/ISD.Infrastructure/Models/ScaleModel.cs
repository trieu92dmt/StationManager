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
        public ScaleModel()
        {
            WeighSessionModel = new HashSet<WeighSessionModel>();
        }

        [Key]
        public Guid ScaleId { get; set; }
        [StringLength(50)]
        public string ScaleName { get; set; }
        public bool? ScaleType { get; set; }
        public bool? isCantai { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }

        [InverseProperty("Scale")]
        public virtual ICollection<WeighSessionModel> WeighSessionModel { get; set; }
    }
}