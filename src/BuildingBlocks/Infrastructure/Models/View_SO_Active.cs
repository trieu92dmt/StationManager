﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Keyless]
    public partial class View_SO_Active
    {
        [StringLength(50)]
        public string VBELN { get; set; }
        [StringLength(10)]
        public string WERKS { get; set; }
    }
}