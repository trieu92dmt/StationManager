﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("DistributionChannelModel", Schema = "DataCollection")]
    public partial class DistributionChannelModel
    {
        [Key]
        public Guid DistributionChannelId { get; set; }
        [StringLength(50)]
        public string DistributionChannelCode { get; set; }
        [StringLength(50)]
        public string DistributionChannelName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}