﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("ProfileEmailDeletedModel", Schema = "Customer")]
    public partial class ProfileEmailDeletedModel
    {
        [Key]
        public Guid EmailId { get; set; }
        [StringLength(1000)]
        public string Email { get; set; }
        public Guid? ProfileId { get; set; }
    }
}