﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WeighSession.Infrastructure.Models
{
    public partial class ScaleModel
    {
        public string ScaleCode { get; set; }
        public string ScaleName { get; set; }
        public bool? ScaleType { get; set; }
        public bool? IsCantai { get; set; }
        public string Plant { get; set; }
        public DateTime? CreateTime { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? LastEditTime { get; set; }
        public Guid? LastEditBy { get; set; }
        public bool? Actived { get; set; }
        public string Note { get; set; }
    }
}