﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("CustomerGiftModel", Schema = "tMasterData")]
    public partial class CustomerGiftModel
    {
        public CustomerGiftModel()
        {
            CustomerGiftDetailModel = new HashSet<CustomerGiftDetailModel>();
        }

        [Key]
        public Guid GiftId { get; set; }
        [Required]
        [StringLength(50)]
        public string GiftCode { get; set; }
        [Required]
        [StringLength(200)]
        public string GiftName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EffectFromDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EffectToDate { get; set; }
        public string Description { get; set; }
        [StringLength(100)]
        public string ImageUrl { get; set; }
        [StringLength(4000)]
        public string Notes { get; set; }

        [InverseProperty("Gift")]
        public virtual ICollection<CustomerGiftDetailModel> CustomerGiftDetailModel { get; set; }
    }
}