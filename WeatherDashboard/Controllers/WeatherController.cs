using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WeatherDashboard.Models;
using WeatherDashboard.Services;
using WeatherDashboard.ViewModels;

namespace WeatherDashboard.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weather;
        private readonly WeatherDbContext _db;

        public WeatherController(IWeatherService weather, WeatherDbContext db)
        {
            _weather = weather;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var lastFive = await _db.CitySearches
                .OrderByDescending(c => c.SearchedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.LastFive = lastFive;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                ModelState.AddModelError("", "Please enter a city name.");
                return RedirectToAction(nameof(Index));
            }

            var weatherResult = await _weather.GetWeatherAsync(city);
            if (weatherResult == null)
            {
                TempData["Error"] = "City not found or weather API error.";
                return RedirectToAction(nameof(Index));
            }

            // save city search
            var cs = new CitySearch { CityName = weatherResult.CityName };
            _db.CitySearches.Add(cs);
            await _db.SaveChangesAsync();

            // fetch last 5 for sidebar
            var lastFive = await _db.CitySearches
                .OrderByDescending(c => c.SearchedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.LastFive = lastFive;
            return View("Index", weatherResult);
        }
    }
}
