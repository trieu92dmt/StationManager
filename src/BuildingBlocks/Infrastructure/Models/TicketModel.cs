﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("TicketModel", Schema = "masterdata")]
    public partial class TicketModel
    {
        public TicketModel()
        {
            Ticket_Seat_Mapping = new HashSet<Ticket_Seat_Mapping>();
        }

        [Key]
        public Guid TicketId { get; set; }
        public int TicketCode { get; set; }
        public Guid? CarCompanyId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? TripId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Price { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public Guid? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("CarCompanyId")]
        [InverseProperty("TicketModel")]
        public virtual CarCompanyModel CarCompany { get; set; }
        [ForeignKey("TripId")]
        [InverseProperty("TicketModel")]
        public virtual TripModel Trip { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("TicketModel")]
        public virtual AccountModel User { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<Ticket_Seat_Mapping> Ticket_Seat_Mapping { get; set; }
    }
}