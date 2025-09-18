using System.Threading.Tasks;
using WeatherDashboard.ViewModels;

namespace WeatherDashboard.Services
{
    public interface IWeatherService
    {
        Task<WeatherResultViewModel?> GetWeatherAsync(string city);
    }
}
