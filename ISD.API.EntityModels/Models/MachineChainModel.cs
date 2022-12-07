﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("MachineChainModel", Schema = "tMasterData")]
    public partial class MachineChainModel
    {
        [Key]
        public Guid MachineChainId { get; set; }
        public int MachineChainIntId { get; set; }
        [StringLength(20)]
        public string MachineChainCode { get; set; }
        [StringLength(500)]
        public string MachineChainName { get; set; }
        [StringLength(20)]
        public string MachineChainTypeCode { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        public Guid? WorkShopId { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public int? MachineChainProduction { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? IsDeleted { get; set; }
    }
}