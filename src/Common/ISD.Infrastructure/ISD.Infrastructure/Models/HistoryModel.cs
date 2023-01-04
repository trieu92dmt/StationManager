﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("HistoryModel", Schema = "utilities")]
    public partial class HistoryModel
    {
        [Key]
        public Guid HistoryModifyId { get; set; }
        public Guid? PageId { get; set; }
        public Guid? FieldId { get; set; }
        [StringLength(50)]
        public string FieldCode { get; set; }
        [StringLength(200)]
        public string FieldName { get; set; }
        public string OldData { get; set; }
        [StringLength(100)]
        public string LastModifiedUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedTime { get; set; }
    }
}