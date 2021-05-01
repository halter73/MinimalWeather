using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WebApiWeather
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseQueryString;

        public WeatherService(IConfiguration config)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
            };

            _baseQueryString = $"api-version=1.0&subscription-key={config["SubscriptionKey"]}&unit=imperial";
        }

        public Task<T> GetFromJsonAsync<T>(string path, string extraQuery)
             => _httpClient.GetFromJsonAsync<T>($"{path}?{_baseQueryString}{extraQuery}");
    }
}
