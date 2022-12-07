﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("AutoConditionModel", Schema = "Task")]
    public partial class AutoConditionModel
    {
        [Key]
        public Guid AutoConditionId { get; set; }
        public Guid? StatusTransitionId { get; set; }
        [StringLength(50)]
        public string AdditionalSQLText { get; set; }
        [StringLength(50)]
        public string ConditionType { get; set; }
        [StringLength(500)]
        public string Field { get; set; }
        [StringLength(50)]
        public string ComparisonType { get; set; }
        [StringLength(10)]
        public string ValueType { get; set; }
        [StringLength(500)]
        public string Value { get; set; }
        [StringLength(500)]
        public string SQLText { get; set; }
        public int? OrderIndex { get; set; }
    }
}