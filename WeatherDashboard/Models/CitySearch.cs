using System;
using System.ComponentModel.DataAnnotations;

namespace WeatherDashboard.Models
{
    public class CitySearch
    {
        public int Id { get; set; }

        [Required]
        public string CityName { get; set; } = null!;

        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
    }
}
