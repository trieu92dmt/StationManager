﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Keyless]
    public partial class View_Stock_TransferTo_Receive
    {
        public Guid? StockId { get; set; }
        public Guid? ProductId { get; set; }
        [Column(TypeName = "decimal(38, 2)")]
        public decimal? ReceiveQty { get; set; }
        public int DeliveryQty { get; set; }
    }
}