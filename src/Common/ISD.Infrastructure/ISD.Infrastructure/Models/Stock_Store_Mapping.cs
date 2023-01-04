﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("Stock_Store_Mapping", Schema = "Warehouse")]
    public partial class Stock_Store_Mapping
    {
        [Key]
        public Guid StockId { get; set; }
        [Key]
        public Guid StoreId { get; set; }
        public bool? isMain { get; set; }

        [ForeignKey("StockId")]
        [InverseProperty("Stock_Store_Mapping")]
        public virtual StockModel Stock { get; set; }
        [ForeignKey("StoreId")]
        [InverseProperty("Stock_Store_Mapping")]
        public virtual StoreModel Store { get; set; }
    }
}