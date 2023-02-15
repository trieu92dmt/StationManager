﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("SendMailCalendarModel", Schema = "Marketing")]
    public partial class SendMailCalendarModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? ToProfileId { get; set; }
        [StringLength(200)]
        public string ToEmail { get; set; }
        public bool? IsSend { get; set; }
        public string Param { get; set; }
        public bool? IsBounce { get; set; }
        public bool? IsOpened { get; set; }
        [StringLength(500)]
        public string FullName { get; set; }
        /// <summary>
        /// Đã xác nhận
        /// </summary>
        public bool? isConfirm { get; set; }
        /// <summary>
        /// Thời gian xác nhận
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? ConfirmTime { get; set; }
        /// <summary>
        /// Đã check in
        /// </summary>
        public bool? isCheckin { get; set; }
        /// <summary>
        /// Thời gian checkin
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? CheckinTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SendTime { get; set; }
        public bool? isError { get; set; }
        public string ErrorMessage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastOpenedTime { get; set; }
    }
}