﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("Profile_File_Mapping", Schema = "Customer")]
    public partial class Profile_File_Mapping
    {
        [Key]
        public Guid ProfileId { get; set; }
        [Key]
        public Guid FileAttachmentId { get; set; }
        [StringLength(10)]
        public string Note { get; set; }

        [ForeignKey("FileAttachmentId")]
        [InverseProperty("Profile_File_Mapping")]
        public virtual FileAttachmentModel FileAttachment { get; set; }
        [ForeignKey("ProfileId")]
        [InverseProperty("Profile_File_Mapping")]
        public virtual ProfileModel Profile { get; set; }
    }
}