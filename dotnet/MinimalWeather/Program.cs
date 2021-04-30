using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalWeather;

var buildier = WebApplication.CreateBuilder(args);
buildier.Services.AddCors(options => {
    options.AddDefaultPolicy(corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin();
    });
});
var app = buildier.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var baseQueryString = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}&unit=imperial";
using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
};

app.UseCors();

app.MapGet("/weather/{location}", (Func<Coordinate, Task<CombinedWeather>>)(async (Coordinate location) =>
{
    var currentQuery = httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{baseQueryString}&query={location}");
    var hourlyQuery = httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24");
    var dailyQuery = httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{baseQueryString}&query={location}&duration=10");

    return new CombinedWeather
    {
        CurrentWeather = (await currentQuery).Results.FirstOrDefault(),
        HourlyForecasts = (await hourlyQuery).Forecasts,
        DailyForecasts = (await dailyQuery).Forecasts
    };
}));


app.MapGet("/weather/{location}/current", (Func<Coordinate, Task<CurrentWeather>>)((Coordinate location) =>
     httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{baseQueryString}&query={location}")));

app.MapGet("/weather/{location}/forecast/hourly", (Func<Coordinate, Task<HourlyForecast>>)((Coordinate location) =>
     httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24")));

app.MapGet("/weather/{location}/forecast/daily", (Func<Coordinate, Task<DailyForecast>>)((Coordinate location) =>
     httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{baseQueryString}&query={location}&duration=10")));

// These endpoints are temporary to give UI devs a chance to grab data we're not forwarding yet.
app.MapGet("/proxyweather/{location}/current", (Func<Coordinate, Task<string>>)((Coordinate location) =>
     httpClient.GetStringAsync($"currentConditions/json?{baseQueryString}&query={location}")));

app.MapGet("/proxyweather/{location}/forecast/hourly", (Func<Coordinate, Task<string>>)((Coordinate location) =>
     httpClient.GetStringAsync($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24")));

app.MapGet("/proxyweather/{location}/forecast/daily", (Func<Coordinate, Task<string>>)((Coordinate location) =>
     httpClient.GetStringAsync($"forecast/daily/json?{baseQueryString}&query={location}&duration=10")));

app.Run();
