﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("DateClosedHistoryModel", Schema = "MES")]
    public partial class DateClosedHistoryModel
    {
        [Key]
        public Guid DateClosedId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateClosed { get; set; }
        public Guid? ModifiedUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedTime { get; set; }
    }
}