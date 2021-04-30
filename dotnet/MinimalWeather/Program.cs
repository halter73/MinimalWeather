using System;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using MinimalWeather;

var app  = WebApplication.Create(args);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var baseQueryString = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}&unit=imperial";
using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
};

app.MapGet("/weather/{location}", async (Coordinate location) =>
{
    var currentQuery = httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{baseQueryString}&query={location}");
    var hourlyQuery = httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24");
    var dailyQuery = httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{baseQueryString}&query={location}&duration=10");

    // Wait for the 3 parallel requests to complete and combine the responses.
    return new
    {
        CurrentWeather = (await currentQuery).Results[0],
        HourlyForecasts = (await hourlyQuery).Forecasts,
        DailyForecasts = (await dailyQuery).Forecasts
    };
});

app.Run();
