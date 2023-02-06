﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("FileAttachmentModel", Schema = "Customer")]
    public partial class FileAttachmentModel
    {
        public FileAttachmentModel()
        {
            Comment_File_Mapping = new HashSet<Comment_File_Mapping>();
            Profile_File_Mapping = new HashSet<Profile_File_Mapping>();
            Task_File_Mapping = new HashSet<Task_File_Mapping>();
        }

        [Key]
        public Guid FileAttachmentId { get; set; }
        public Guid ObjectId { get; set; }
        [Required]
        [StringLength(50)]
        public string FileAttachmentCode { get; set; }
        [StringLength(255)]
        public string FileAttachmentName { get; set; }
        [StringLength(50)]
        public string FileExtention { get; set; }
        [StringLength(500)]
        public string FileUrl { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        [StringLength(50)]
        public string FileType { get; set; }
        [StringLength(100)]
        public string Description1 { get; set; }
        [StringLength(100)]
        public string Description2 { get; set; }
        [StringLength(100)]
        public string Description3 { get; set; }

        [InverseProperty("FileAttachment")]
        public virtual ICollection<Comment_File_Mapping> Comment_File_Mapping { get; set; }
        [InverseProperty("FileAttachment")]
        public virtual ICollection<Profile_File_Mapping> Profile_File_Mapping { get; set; }
        [InverseProperty("FileAttachment")]
        public virtual ICollection<Task_File_Mapping> Task_File_Mapping { get; set; }
    }
}