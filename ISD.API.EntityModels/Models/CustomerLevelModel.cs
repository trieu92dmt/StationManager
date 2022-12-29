﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("CustomerLevelModel", Schema = "tMasterData")]
    public partial class CustomerLevelModel
    {
        [Key]
        public Guid CustomerLevelId { get; set; }
        [Required]
        [StringLength(50)]
        public string CustomerLevelCode { get; set; }
        [Required]
        [StringLength(100)]
        public string CustomerLevelName { get; set; }
        public int? OrderIndex { get; set; }
        public bool Actived { get; set; }
    }
}