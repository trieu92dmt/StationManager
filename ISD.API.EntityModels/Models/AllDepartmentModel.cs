﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("AllDepartmentModel", Schema = "tMasterData")]
    public partial class AllDepartmentModel
    {
        [Key]
        public Guid AllDepartmentId { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(50)]
        public string DepartmentCode { get; set; }
        [StringLength(500)]
        public string DepartmentName { get; set; }
        public int? OrderIndex { get; set; }
        public bool? Actived { get; set; }
    }
}