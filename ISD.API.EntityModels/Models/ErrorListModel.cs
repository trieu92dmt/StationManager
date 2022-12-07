﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("ErrorListModel", Schema = "tMasterData")]
    public partial class ErrorListModel
    {
        public ErrorListModel()
        {
            DetailQCModel = new HashSet<DetailQCModel>();
        }

        [Key]
        public Guid ErrorListId { get; set; }
        [StringLength(50)]
        public string ErrorListCode { get; set; }
        [StringLength(500)]
        public string ErrorListName { get; set; }
        [StringLength(50)]
        public string StepCode { get; set; }
        public int? TargetCode { get; set; }
        public int? OrderIndex { get; set; }
        public bool? Actived { get; set; }

        [InverseProperty("Error")]
        public virtual ICollection<DetailQCModel> DetailQCModel { get; set; }
    }
}