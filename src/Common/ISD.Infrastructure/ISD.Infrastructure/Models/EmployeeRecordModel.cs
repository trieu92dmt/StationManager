﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("EmployeeRecordModel", Schema = "MESP2")]
    public partial class EmployeeRecordModel
    {
        [Key]
        public Guid EmployeeRecordId { get; set; }
        public Guid? OutputRecordId { get; set; }
        [StringLength(50)]
        public string EmployeeCode { get; set; }

        [ForeignKey("OutputRecordId")]
        [InverseProperty("EmployeeRecordModel")]
        public virtual OutputRecordModel OutputRecord { get; set; }
    }
}