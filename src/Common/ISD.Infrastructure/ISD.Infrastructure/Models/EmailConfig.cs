﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("EmailConfig", Schema = "tMasterData")]
    public partial class EmailConfig
    {
        [Key]
        public Guid EmailConfigId { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(50)]
        public string SmtpServer { get; set; }
        [StringLength(10)]
        public string SmtpPort { get; set; }
        public bool? EnableSsl { get; set; }
        [StringLength(200)]
        public string SmtpMailFrom { get; set; }
        [StringLength(100)]
        public string SmtpUser { get; set; }
        [StringLength(100)]
        public string SmtpPassword { get; set; }
        [StringLength(100)]
        public string ToEmail { get; set; }
        [StringLength(100)]
        public string CCMail { get; set; }
        [StringLength(100)]
        public string BCCMail { get; set; }
        [StringLength(200)]
        public string EmailTitle { get; set; }
        public string EmailContent { get; set; }
    }
}