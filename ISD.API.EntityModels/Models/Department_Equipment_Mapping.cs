﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("Department_Equipment_Mapping", Schema = "tMasterData")]
    public partial class Department_Equipment_Mapping
    {
        [Key]
        public Guid MappingId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? EquipmentId { get; set; }
    }
}