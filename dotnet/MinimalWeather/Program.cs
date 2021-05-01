using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalWeather;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("weather", policyBuilder => policyBuilder.AllowAnyOrigin());
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var baseQuery = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}&unit=imperial";
using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
};

app.UseCors();
app.UseHttpsRedirection();

app.MapGet("/weather/{location}", [EnableCors("weather")] async (Coordinate location) =>
{
    var currentQuery = httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{baseQuery}&query={location}");
    var hourlyQuery = httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{baseQuery}&query={location}&duration=24");
    var dailyQuery = httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{baseQuery}&query={location}&duration=10");

    // Wait for the 3 parallel requests to complete and combine the responses.
    await Task.WhenAll(currentQuery, hourlyQuery, dailyQuery);

    return new
    {
        CurrentWeather = currentQuery.Result.Results[0],
        HourlyForecasts = hourlyQuery.Result.Forecasts,
        DailyForecasts = dailyQuery.Result.Forecasts
    };
});

app.Run();
