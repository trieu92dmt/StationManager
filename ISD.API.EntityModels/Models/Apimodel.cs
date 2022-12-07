﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("APIModel", Schema = "tMasterData")]
    public partial class APIModel
    {
        [Key]
        [StringLength(100)]
        public string Token { get; set; }
        [Required]
        [StringLength(100)]
        public string Key { get; set; }
        public bool? isAllowedToBooking { get; set; }
        public bool? isRequiredLogin { get; set; }
        public bool? isReceiveInCurrentDay { get; set; }
    }
}