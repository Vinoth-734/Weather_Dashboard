using Microsoft.EntityFrameworkCore;

namespace WeatherDashboard.Models
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        public DbSet<CitySearch> CitySearches { get; set; } = null!;
    }
}
