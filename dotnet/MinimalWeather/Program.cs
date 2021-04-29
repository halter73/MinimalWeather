using System;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using MinimalWeather;

var app = WebApplication.Create(args);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var baseQueryString = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}&unit=imperial";
using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
};

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
