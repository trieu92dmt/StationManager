﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    public partial class ApplicationConfig
    {
        [Key]
        [StringLength(100)]
        public string ConfigKey { get; set; }
        [StringLength(100)]
        public string ConfigValue { get; set; }
    }
}