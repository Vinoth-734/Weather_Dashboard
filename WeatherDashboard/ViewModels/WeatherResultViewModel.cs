using System;
using System.Collections.Generic;

namespace WeatherDashboard.ViewModels
{
    public class WeatherResultViewModel
    {
        public string CityName { get; set; } = string.Empty;
        public double TemperatureC { get; set; }
        public int Humidity { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string IconCode { get; set; } = string.Empty;
        public List<WeatherPoint> ForecastPoints { get; set; } = new();
    }

    public class WeatherPoint
    {
        public DateTime DateTime { get; set; }
        public double Temp { get; set; }
    }
}
