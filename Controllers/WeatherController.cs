using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Domains;

namespace WeatherAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly HttpClient _httpClient;
        private readonly OpenWeatherSettings _openWeatherSettings;

        public WeatherController(IHttpClientFactory httpClientFactory, OpenWeatherSettings openWeatherSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _openWeatherSettings = openWeatherSettings;
        }

        [HttpGet(Name = "GetWeather")]
        public async Task<IActionResult> Get(double lat, double lon)
        {
            var url = $"{_openWeatherSettings.BaseUrl}?lat={lat}&lon={lon}&appid={_openWeatherSettings.ApiKey}&units=imperial";
            var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);

            if (response == null) return NotFound("Weather data not found.");

            var temperature = response.Main.Temp;
            var weatherCondition = response.Weather.FirstOrDefault()?.Main ?? "Unknown";

            string tempDescription = temperature switch
            {
                > 90 => "Hot",
                < 40 => "Cold",
                _ => "Moderate"
            };

            return Ok(new { 
                Weather = weatherCondition, TempDescription = tempDescription 
            });
        }
    }
}