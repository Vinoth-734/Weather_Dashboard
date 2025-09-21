using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WeatherDashboard.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace WeatherDashboard.Services
{
    public class OpenWeatherMapService : IWeatherService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public OpenWeatherMapService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["OpenWeatherMap:ApiKey"] ?? throw new ArgumentNullException("OpenWeatherMap:ApiKey missing");
        }

        public async Task<WeatherResultViewModel?> GetWeatherAsync(string city)
        {
            // Current weather
            var currentUrl = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";
            var current = await _http.GetFromJsonAsync<OpenWeatherCurrent?>(currentUrl);
            if (current == null || current.Cod != 200) return null;

            // 5 day / 3 hour forecast for temperature trends
            var forecastUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";
            var forecast = await _http.GetFromJsonAsync<OpenWeatherForecast?>(forecastUrl);

            var result = new WeatherResultViewModel
            {
                CityName = current.Name,
                TemperatureC = current.Main.Temp,
                Humidity = current.Main.Humidity,
                Condition = current.Weather?.FirstOrDefault()?.Main ?? current.Weather?.FirstOrDefault()?.Description ?? "N/A",
                IconCode = current.Weather?.FirstOrDefault()?.Icon ?? "01d",
                ForecastPoints = new List<WeatherPoint>()
            };

            if (forecast?.List != null)
            {
                // take up to next 10 forecast points to display compact trend
                foreach (var item in forecast.List.Take(10))
                {
                    result.ForecastPoints.Add(new WeatherPoint
                    {
                        DateTime = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime,
                        Temp = item.Main.Temp
                    });
                }
            }

            return result;
        }

        // Internal DTOs to deserialize only needed fields:
        private class OpenWeatherCurrent
        {
            public int Cod { get; set; }
            public string Name { get; set; } = "";
            public MainInfo Main { get; set; } = new();
            public WeatherInfo[]? Weather { get; set; }
        }
        private class OpenWeatherForecast
        {
            public ForecastItem[]? List { get; set; }
        }
        private class ForecastItem
        {
            public long Dt { get; set; }
            public MainInfo Main { get; set; } = new();
        }
        private class MainInfo { public double Temp { get; set; } public int Humidity { get; set; } }
        private class WeatherInfo
        {
            public string Main { get; set; } = "";
            public string Description { get; set; } = "";
            public string Icon { get; set; } = "";  // <--- NEW
        }
    }
}
