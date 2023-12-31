﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("SeatModel", Schema = "masterdata")]
    public partial class SeatModel
    {
        public SeatModel()
        {
            Ticket_Seat_Mapping = new HashSet<Ticket_Seat_Mapping>();
        }

        [Key]
        public Guid SeatId { get; set; }
        public Guid? CarTypeId { get; set; }
        [StringLength(50)]
        public string SeatNumber { get; set; }
        public int? Floor { get; set; }
        public int? Col { get; set; }
        public int? Row { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("CarTypeId")]
        [InverseProperty("SeatModel")]
        public virtual CarTypeModel CarType { get; set; }
        [InverseProperty("Seat")]
        public virtual ICollection<Ticket_Seat_Mapping> Ticket_Seat_Mapping { get; set; }
    }
}