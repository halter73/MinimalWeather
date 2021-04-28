using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

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
    httpClient.GetStringAsync($"currentConditions/json?{baseQueryString}&unit=imperial&query={location}"));

app.Run();

public record Coordinate(double Latitude, double Longitude)
{
    public static bool TryParse(string input, out Coordinate coordinate)
    {
        coordinate = default;
        var splitArray = input.Split(',', 2);

        if (splitArray.Length != 2)
        {
            return false;
        }

        if (!double.TryParse(splitArray[0], out var lat))
        {
            return false;
        }

        if (!double.TryParse(splitArray[1], out var lon))
        {
            return false;
        }

        coordinate = new Coordinate(lat, lon);
        return true;
    }

    public override string ToString()
    {
        return $"{Latitude},{Longitude}";
    }
}
