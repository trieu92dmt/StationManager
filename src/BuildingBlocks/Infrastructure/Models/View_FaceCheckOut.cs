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
    public partial class View_FaceCheckOut
    {
        [StringLength(150)]
        [Unicode(false)]
        public string PersonID { get; set; }
        [StringLength(250)]
        public string PersonName { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string Avatar { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateCheckIn { get; set; }
        public TimeSpan? LastTimeCheckIn { get; set; }
        [StringLength(250)]
        public string DeviceCheckOut { get; set; }
    }
}