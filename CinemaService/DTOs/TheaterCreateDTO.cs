﻿using System.ComponentModel.DataAnnotations;

namespace P2.CinemaService.DTOs
{
    public class TheaterCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid CinemaId { get; set; }
        [Range(1, 100)]
        public int TotalSeats { get; set; }
    }
}
