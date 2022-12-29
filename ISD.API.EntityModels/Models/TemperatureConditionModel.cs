﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("TemperatureConditionModel", Schema = "ghMasterData")]
    public partial class TemperatureConditionModel
    {
        public TemperatureConditionModel()
        {
            MaterialModel = new HashSet<MaterialModel>();
        }

        [Key]
        [StringLength(50)]
        public string TemperatureConditionCode { get; set; }
        [StringLength(400)]
        public string TemperatureConditionName { get; set; }
        public bool? Actived { get; set; }

        [InverseProperty("TemperatureConditionCodeNavigation")]
        public virtual ICollection<MaterialModel> MaterialModel { get; set; }
    }
}