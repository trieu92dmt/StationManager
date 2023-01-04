﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("PersonInChargeModel", Schema = "Customer")]
    public partial class PersonInChargeModel
    {
        [Key]
        public Guid PersonInChargeId { get; set; }
        public Guid? ProfileId { get; set; }
        [StringLength(50)]
        public string SalesEmployeeCode { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [StringLength(50)]
        public string RoleCode { get; set; }
        [StringLength(50)]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 1: NV kinh doanh, 2: NV sales admin
        /// </summary>
        public int? SalesEmployeeType { get; set; }

        [ForeignKey("ProfileId")]
        [InverseProperty("PersonInChargeModel")]
        public virtual ProfileModel Profile { get; set; }
        [ForeignKey("SalesEmployeeCode")]
        [InverseProperty("PersonInChargeModel")]
        public virtual SalesEmployeeModel SalesEmployeeCodeNavigation { get; set; }
    }
}