﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("NotificationAccountMappingModel", Schema = "ghMasterData")]
    public partial class NotificationAccountMappingModel
    {
        [Key]
        public Guid NotificationId { get; set; }
        [Key]
        public Guid AccountId { get; set; }
        public bool? IsRead { get; set; }

        [ForeignKey("AccountId")]
        [InverseProperty("NotificationAccountMappingModel")]
        public virtual AccountModel Account { get; set; }
        [ForeignKey("NotificationId")]
        [InverseProperty("NotificationAccountMappingModel")]
        public virtual NotificationModel Notification { get; set; }
    }
}