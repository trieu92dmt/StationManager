﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("Department_Routing_Mapping", Schema = "tMasterData")]
    public partial class Department_Routing_Mapping
    {
        [Key]
        public Guid DepartmentId { get; set; }
        [Key]
        public Guid StepId { get; set; }
        public string Note { get; set; }

        [ForeignKey("DepartmentId")]
        [InverseProperty("Department_Routing_Mapping")]
        public virtual DepartmentModel Department { get; set; }
        [ForeignKey("StepId")]
        [InverseProperty("Department_Routing_Mapping")]
        public virtual RoutingModel Step { get; set; }
    }
}