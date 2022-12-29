﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("VendorModel", Schema = "DataCollection")]
    public partial class VendorModel
    {
        [Key]
        public Guid VendorId { get; set; }
        [StringLength(50)]
        public string VendorCode { get; set; }
        public int? VendorCodeInt { get; set; }
        [StringLength(50)]
        public string VendorName { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}