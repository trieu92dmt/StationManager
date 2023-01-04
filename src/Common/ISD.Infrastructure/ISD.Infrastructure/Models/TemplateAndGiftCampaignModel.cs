﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("TemplateAndGiftCampaignModel", Schema = "Marketing")]
    public partial class TemplateAndGiftCampaignModel
    {
        [Key]
        public Guid Id { get; set; }
        public int TemplateAndGiftCampaignCode { get; set; }
        [StringLength(50)]
        public string TemplateAndGiftCampaignName { get; set; }
        public Guid? TemplateAndGiftTargetGroupId { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }

        [ForeignKey("CreateBy")]
        [InverseProperty("TemplateAndGiftCampaignModelCreateByNavigation")]
        public virtual AccountModel CreateByNavigation { get; set; }
        [ForeignKey("LastEditBy")]
        [InverseProperty("TemplateAndGiftCampaignModelLastEditByNavigation")]
        public virtual AccountModel LastEditByNavigation { get; set; }
        [ForeignKey("TemplateAndGiftTargetGroupId")]
        [InverseProperty("TemplateAndGiftCampaignModel")]
        public virtual TemplateAndGiftTargetGroupModel TemplateAndGiftTargetGroup { get; set; }
    }
}