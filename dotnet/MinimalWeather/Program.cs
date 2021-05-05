using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalWeather;

var cacheTimeout = TimeSpan.FromMinutes(10);
var cache = new MemoryCache(new MemoryCacheOptions
{
    SizeLimit = 64,
});

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
    if (cache.TryGetValue<CombinedWeather>(location, out var weather))
    {
        return weather;
    }

    var timeZoneQuery = httpClient.GetFromJsonAsync<TimeZoneResponse>($"https://atlas.microsoft.com/timezone/byCoordinates/json?{baseQueryString}&query={location}");
    var currentQuery = httpClient.GetFromJsonAsync<CurrentWeather>($"currentConditions/json?{baseQueryString}&query={location}");
    var hourlyQuery = httpClient.GetFromJsonAsync<HourlyForecast>($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24");
    var dailyQuery = httpClient.GetFromJsonAsync<DailyForecast>($"forecast/daily/json?{baseQueryString}&query={location}&duration=10");

    await Task.WhenAll(timeZoneQuery, currentQuery, hourlyQuery, dailyQuery);

    var referenceTime = timeZoneQuery.Result.TimeZones[0].ReferenceTime;
    var offset = TimeSpan.Parse(referenceTime.StandardOffset) + TimeSpan.Parse(referenceTime.DaylightSavings);

    var currentWeather = currentQuery.Result.Results[0];
    currentWeather.DateTime = currentWeather.DateTime.ToOffset(offset);

    var hourlyForecasts = hourlyQuery.Result.Forecasts;
    foreach (var snapshot in hourlyForecasts)
    {
        snapshot.DateTime = snapshot.DateTime.ToOffset(offset);
    }

    var dailyForecasts = dailyQuery.Result.Forecasts;
    foreach (var forecast in dailyForecasts)
    {
        forecast.DateTime = forecast.DateTime.ToOffset(offset);
    }

    var combined = new CombinedWeather
    {
        CurrentWeather = currentWeather,
        HourlyForecasts = hourlyForecasts,
        DailyForecasts = dailyForecasts,
        DebugProperty = "Hello",
    };

    cache.Set(location, combined, new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = cacheTimeout,
        Size = 1,
    });

    return combined;
}));

app.Run();