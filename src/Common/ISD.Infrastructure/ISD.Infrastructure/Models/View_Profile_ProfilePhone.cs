﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Keyless]
    public partial class View_Profile_ProfilePhone
    {
        public Guid? ProfileId { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
    }
}