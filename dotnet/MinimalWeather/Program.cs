using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalWeather;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddResponseCaching();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var baseQueryString = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}&unit=imperial";
using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
};

app.UseResponseCaching();

app.MapGet("/weather/{location}", async (Coordinate location) =>
{
    var currentWeather = await httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{baseQueryString}&query={location}");
    var hourlyWeather = await httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24");
    var dailyWeather = await httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{baseQueryString}&query={location}&duration=10");

    return new
    {
        CurrentWeather = currentWeather.Results.FirstOrDefault(),
        HourlyForecasts = hourlyWeather.Forecasts,
        DailyForecasts = dailyWeather.Forecasts
    };
});

app.MapGet("/weather/{location}/current", (Coordinate location) =>
     httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{baseQueryString}&query={location}"));

app.MapGet("/weather/{location}/forecast/hourly", (Coordinate location) =>
     httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24"));

app.MapGet("/weather/{location}/forecast/daily", (Coordinate location) =>
     httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{baseQueryString}&query={location}&duration=10"));

// These endpoints are temporary to give UI devs a chance to grab data we're not forwarding yet.
app.MapGet("/proxyweather/{location}/current", (Coordinate location) =>
     httpClient.GetStringAsync($"currentConditions/json?{baseQueryString}&query={location}"));

app.MapGet("/proxyweather/{location}/forecast/hourly", (Coordinate location) =>
     httpClient.GetStringAsync($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24"));

app.MapGet("/proxyweather/{location}/forecast/daily", (Coordinate location) =>
     httpClient.GetStringAsync($"forecast/daily/json?{baseQueryString}&query={location}&duration=10"));

app.Run();
