﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("PromotionGiftAccessoryModel", Schema = "ghMasterData")]
    public partial class PromotionGiftAccessoryModel
    {
        [Key]
        public Guid GiftMaterialId { get; set; }
        public Guid? PromotionId { get; set; }
        [StringLength(50)]
        public string AccessoryCode { get; set; }
    }
}