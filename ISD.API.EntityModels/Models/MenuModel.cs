﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("MenuModel", Schema = "pms")]
    public partial class MenuModel
    {
        public MenuModel()
        {
            MobileScreenModel = new HashSet<MobileScreenModel>();
            PageModel = new HashSet<PageModel>();
        }

        [Key]
        public Guid MenuId { get; set; }
        public Guid? ModuleId { get; set; }
        [StringLength(100)]
        public string MenuName { get; set; }
        public int? OrderIndex { get; set; }
        [StringLength(100)]
        public string Icon { get; set; }

        [ForeignKey("ModuleId")]
        [InverseProperty("MenuModel")]
        public virtual ModuleModel Module { get; set; }
        [InverseProperty("Menu")]
        public virtual ICollection<MobileScreenModel> MobileScreenModel { get; set; }
        [InverseProperty("Menu")]
        public virtual ICollection<PageModel> PageModel { get; set; }
    }
}