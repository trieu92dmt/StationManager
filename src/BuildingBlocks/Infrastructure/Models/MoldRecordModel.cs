﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("MoldRecordModel", Schema = "MESP2")]
    public partial class MoldRecordModel
    {
        [Key]
        public Guid MoldRecordId { get; set; }
        public Guid? OutputRecordId { get; set; }
        [StringLength(50)]
        public string MoldCode { get; set; }
        [StringLength(50)]
        public string Serial { get; set; }

        [ForeignKey("OutputRecordId")]
        [InverseProperty("MoldRecordModel")]
        public virtual OutputRecordModel OutputRecord { get; set; }
    }
}