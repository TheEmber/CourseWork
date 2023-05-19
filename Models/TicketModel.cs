using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models;
    public class Ticket
    {
        [Key]
        public int Seat {get; set;}
        [ForeignKey("Flight")]
        public Guid FlightID { get; set; }
        public Flight Flight { get; set; }
        public Guid? BookedBy { get; set; }
        public User? User { get; set; }
    }