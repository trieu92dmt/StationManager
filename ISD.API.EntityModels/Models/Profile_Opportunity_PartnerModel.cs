﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("Profile_Opportunity_PartnerModel", Schema = "Customer")]
    public partial class Profile_Opportunity_PartnerModel
    {
        [Key]
        public Guid OpportunityPartnerId { get; set; }
        public Guid? ProfileId { get; set; }
        public Guid? PartnerId { get; set; }
        /// <summary>
        /// 1: Chủ đầu tư, 2: Thiết kế, 3: Tổng thầu, 4: Căn mẫu, 5: Đại trà
        /// </summary>
        public int? PartnerType { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public bool? IsMain { get; set; }

        [ForeignKey("PartnerId")]
        [InverseProperty("Profile_Opportunity_PartnerModelPartner")]
        public virtual ProfileModel Partner { get; set; }
        [ForeignKey("ProfileId")]
        [InverseProperty("Profile_Opportunity_PartnerModelProfile")]
        public virtual ProfileModel Profile { get; set; }
    }
}