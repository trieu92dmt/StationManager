﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("QualityControl_QCInformation_File_Mapping", Schema = "MES")]
    public partial class QualityControl_QCInformation_File_Mapping
    {
        [Key]
        public Guid FileAttachmentId { get; set; }
        [Key]
        public Guid QualityControl_QCInformation_Id { get; set; }
        [StringLength(200)]
        public string Note { get; set; }
    }
}