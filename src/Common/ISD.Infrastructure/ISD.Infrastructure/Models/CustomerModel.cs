﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("CustomerModel", Schema = "DataCollection")]
    public partial class CustomerModel
    {
        [Key]
        public Guid CustomerId { get; set; }
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [StringLength(50)]
        public string CustomerName { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }
    }
}