﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("CampaignModel", Schema = "Marketing")]
    public partial class CampaignModel
    {
        [Key]
        public Guid Id { get; set; }
        public int CampaignCode { get; set; }
        [Required]
        [StringLength(255)]
        public string CampaignName { get; set; }
        public string Description { get; set; }
        public Guid ContentId { get; set; }
        public Guid TargetGroupId { get; set; }
        [StringLength(50)]
        public string SaleOrg { get; set; }
        public Guid? Status { get; set; }
        public bool? isImmediately { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ScheduledToStart { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        /// <summary>
        /// Marketing|Event
        /// </summary>
        [StringLength(50)]
        public string Type { get; set; }

        [ForeignKey("Status")]
        [InverseProperty("CampaignModel")]
        public virtual CatalogModel StatusNavigation { get; set; }
    }
}