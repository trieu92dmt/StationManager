﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("ProfilePhoneModel", Schema = "Customer")]
    public partial class ProfilePhoneModel
    {
        [Key]
        public Guid PhoneId { get; set; }
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        public Guid? ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        [InverseProperty("ProfilePhoneModel")]
        public virtual ProfileModel Profile { get; set; }
    }
}