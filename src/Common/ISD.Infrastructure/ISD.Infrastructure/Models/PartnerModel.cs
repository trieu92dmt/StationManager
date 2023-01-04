﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("PartnerModel", Schema = "Customer")]
    public partial class PartnerModel
    {
        [Key]
        public Guid PartnerId { get; set; }
        public Guid? ProfileId { get; set; }
        [StringLength(50)]
        public string PartnerType { get; set; }
        public Guid? PartnerProfileId { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }

        [ForeignKey("PartnerProfileId")]
        [InverseProperty("PartnerModelPartnerProfile")]
        public virtual ProfileModel PartnerProfile { get; set; }
        [ForeignKey("ProfileId")]
        [InverseProperty("PartnerModelProfile")]
        public virtual ProfileModel Profile { get; set; }
    }
}