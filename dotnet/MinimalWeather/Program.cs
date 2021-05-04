var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddPolicy("weather", o => o.AllowAnyOrigin()));

var app = builder.Build();
app.UseCors();

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://atlas.microsoft.com/weather/");
var baseQuery = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}&unit=imperial";

Task<T> GetAsync<T>(string path, string query) => httpClient.GetFromJsonAsync<T>($"{path}?{baseQuery}{query}");

app.MapGet("/weather/{location}", [EnableCors("weather")] async (Coordinate location) =>
{
    var currentQuery = GetAsync<CurrentWeather>("currentConditions/json", $"&query={location}");
    var hourlyQuery = GetAsync<HourlyForecast>("forecast/hourly/json", $"&query={location}&duration=24");
    var dailyQuery = GetAsync<DailyForecast>("forecast/daily/json", $"&query={location}&duration=10");

    // Wait for the 3 parallel requests to complete and combine the responses.
    await Task.WhenAll(currentQuery, hourlyQuery, dailyQuery);

    return new
    {
        CurrentWeather = currentQuery.Result.Results[0],
        HourlyForecasts = hourlyQuery.Result.Forecasts,
        DailyForecasts = dailyQuery.Result.Forecasts,
    };
});

app.Run();