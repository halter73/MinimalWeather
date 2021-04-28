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

using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
};
var baseQueryString = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}";

app.MapGet("/weather/current/{location}", (Coordinate location) =>
     httpClient.GetFromJsonAsync<WeatherResults>($"currentConditions/json?{baseQueryString}&unit=imperial&query={location}"));

app.MapGet("/proxyweather/current/{location}", (Coordinate location) =>
     httpClient.GetStringAsync($"currentConditions/json?{baseQueryString}&unit=imperial&query={location}"));

app.Run();
