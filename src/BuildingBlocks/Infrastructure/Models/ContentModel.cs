﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("ContentModel", Schema = "Marketing")]
    public partial class ContentModel
    {
        [Key]
        public Guid Id { get; set; }
        public int ContentCode { get; set; }
        [Required]
        [StringLength(500)]
        public string ContentName { get; set; }
        public Guid FromEmailAccountId { get; set; }
        [Required]
        [StringLength(50)]
        public string SenderName { get; set; }
        [StringLength(50)]
        public string SaleOrg { get; set; }
        [Required]
        [StringLength(500)]
        public string Subject { get; set; }
        [Required]
        [Column(TypeName = "ntext")]
        public string Content { get; set; }
        public bool? Actived { get; set; }
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
        [StringLength(50)]
        public string CompanyCode { get; set; }

        [ForeignKey("CreateBy")]
        [InverseProperty("ContentModelCreateByNavigation")]
        public virtual AccountModel CreateByNavigation { get; set; }
        [ForeignKey("LastEditBy")]
        [InverseProperty("ContentModelLastEditByNavigation")]
        public virtual AccountModel LastEditByNavigation { get; set; }
    }
}