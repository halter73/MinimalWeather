using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApiWeather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseQuery;

        public WeatherController(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _httpClient = clientFactory.CreateClient("weather");
            _baseQuery = $"api-version=1.0&subscription-key={config["SubscriptionKey"]}&unit=imperial";
        }

        [EnableCors("weather")]
        [HttpGet("{latitude},{longitude}")]
        public async Task<CombinedWeather> Get(double latitude, double longitude)
        {
            var currentQuery = _httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{_baseQuery}&query={latitude},{longitude}");
            var hourlyQuery = _httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{_baseQuery}&query={latitude},{longitude}&duration=24");
            var dailyQuery = _httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{_baseQuery}&query={latitude},{longitude}&duration=10");

            // Wait for the 3 parallel requests to complete and combine the responses.
            await Task.WhenAll(currentQuery, hourlyQuery, dailyQuery);

            return new()
            {
                CurrentWeather = currentQuery.Result.Results[0],
                HourlyForecasts = hourlyQuery.Result.Forecasts,
                DailyForecasts = dailyQuery.Result.Forecasts,
            };
        }
    }
}
