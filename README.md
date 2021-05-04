# Weather Samples

## Minimal ASP.NET Core

### [Program.cs](https://github.com/halter73/MinimalWeather/blob/main/dotnet/MinimalWeather/Program.cs)

```csharp
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
```

## Express

### [app.ts](https://github.com/halter73/MinimalWeather/blob/main/node/ExpressWeather/app.ts)

```typescript
import * as cors from 'cors';
import * as express from 'express';
import got from 'got';

const baseUrl = 'https://atlas.microsoft.com/weather/';
const baseQuery = `api-version=1.0&subscription-key=${process.env['SubscriptionKey']}&unit=imperial`;

const app = express();

app.get('/weather/:lat,:lon', cors(), async (req, res, next) => {
    try {
        const lat = parseFloat(req.params.lat);
        const lon = parseFloat(req.params.lon);

        const currentQuery = got(`${baseUrl}currentConditions/json?${baseQuery}&query=${lat},${lon}`);
        const hourlyQuery = got(`${baseUrl}forecast/hourly/json?${baseQuery}&query=${lat},${lon}&duration=24`);
        const dailyQuery = got(`${baseUrl}forecast/daily/json?${baseQuery}&query=${lat},${lon}&duration=10`);

        // Wait for the 3 parallel requests to complete and combine the responses.
        const [currentResponse, hourlyResponse, dailyResponse] = await Promise.all([currentQuery, hourlyQuery, dailyQuery]);

        await res.json({
            currentWeather: JSON.parse(currentResponse.body).results[0],
            hourlyForecasts: JSON.parse(hourlyResponse.body).forecasts,
            dailyForecasts: JSON.parse(dailyResponse.body).forecasts,
        });
    } catch (err) {
        // Express doesn't handle async errors natively yet.
        next(err);
    }
});

const port = process.env.PORT || 3000;

app.listen(port, function () {
    console.log(`Listening on port ${port}`);
});
```

## ASP.NET Core Web API

### [Program.cs](https://github.com/halter73/MinimalWeather/blob/main/dotnet/WebApiWeather/Program.cs)

```csharp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApiWeather
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
```

### [Startup.cs](https://github.com/halter73/MinimalWeather/blob/main/dotnet/WebApiWeather/Startup.cs)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace WebApiWeather
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHttpClient("weather", options =>
            {
                options.BaseAddress = new Uri("https://atlas.microsoft.com/weather/");
            });

            services.AddCors(options =>
            {
                options.AddPolicy("weather", policyBuilder => policyBuilder.AllowAnyOrigin());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiWeather", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiWeather v1"));
            }

            app.UseRouting();

            app.UseCors();
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

### [WeatherController.cs](https://github.com/halter73/MinimalWeather/blob/main/dotnet/WebApiWeather/Controllers/WeatherController.cs)

```csharp
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApiWeather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseQuery;

        public WeatherController(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _httpClient = clientFactory.CreateClient("weather");
            _baseQuery = $"api-version=1.0&subscription-key={config["SubscriptionKey"]}&unit=imperial";
        }

        [EnableCors("weather")]
        [HttpGet("{latitude},{longitude}")]
        public async Task<CombinedWeather> Get(double latitude, double longitude)
        {
            var currentQuery = GetAsync<CurrentWeather>("currentConditions/json", $"&query={latitude},{longitude}");
            var hourlyQuery = GetAsync<HourlyForecast>("forecast/hourly/json", $"&query={latitude},{longitude}&duration=24");
            var dailyQuery = GetAsync<DailyForecast>("forecast/daily/json", $"&query={latitude},{longitude}&duration=10");

            // Wait for the 3 parallel requests to complete and combine the responses.
            await Task.WhenAll(currentQuery, hourlyQuery, dailyQuery);

            return new()
            {
                CurrentWeather = currentQuery.Result.Results[0],
                HourlyForecasts = hourlyQuery.Result.Forecasts,
                DailyForecasts = dailyQuery.Result.Forecasts,
            };
        }

        private Task<T> GetAsync<T>(string path, string query) => _httpClient.GetFromJsonAsync<T>($"{path}?{_baseQuery}{query}");
    }
}
```
