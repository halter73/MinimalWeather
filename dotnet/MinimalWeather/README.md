## Build instructions

To build the [main](https://github.com/halter73/MinimalWeather/tree/main/dotnet/MinimalWeather) branch relying on the natural type for lambdas, you can checkout the [features/compiler roslyn branch](https://github.com/dotnet/roslyn/tree/features/compiler) and set "Roslyn.VisualStudio.Setup" as your startup project to launch VS. Then you need to add something like `<CscToolPath>C:\dev\dotnet\roslyn\artifacts\bin\csc\Debug\net472</CscToolPath>` to the csproj.
 
If you just want to test in C# 9 for now, you can just checkout the [csharp-nine](https://github.com/halter73/MinimalWeather/tree/csharp-nine/dotnet/MinimalWeather) branch I just pushed that does a bunch of casting. Either way you'll need a preview4 sdk which you can get from dotnet/installer.

## Program.cs

The current majority of the program lives in Program.cs.

```C#
using System;
using System.Net.Http;
using System.Net.Http.Json;
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

var baseQueryString = $"api-version=1.0&subscription-key={app.Configuration["SubscriptionKey"]}&unit=imperial";
using var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://atlas.microsoft.com/weather/")
};

app.UseCors();

app.MapGet("/weather/{location}", [EnableCors("weather")] async (Coordinate location) =>
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
```

## Sample requests and response

### Current Weather

https://localhost:5001/weather/47.6062,-122.3321/

```json
{
  "currentWeather": {
    "dateTime": "2021-04-30T19:20:00+00:00",
    "phrase": "Cloudy",
    "temperature": {
      "value": 58,
      "unit": "F"
    }
  },
  "hourlyForecasts": [
    {
      "dateTime": "2021-04-30T20:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 58,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-30T21:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 60,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-30T22:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 61,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-30T23:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 63,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T00:00:00+00:00",
      "phrase": "Rain",
      "temperature": {
        "value": 63,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T01:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 62,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T02:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 61,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T03:00:00+00:00",
      "phrase": "Intermittent clouds",
      "temperature": {
        "value": 60,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T04:00:00+00:00",
      "phrase": "Intermittent clouds",
      "temperature": {
        "value": 59,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T05:00:00+00:00",
      "phrase": "Intermittent clouds",
      "temperature": {
        "value": 57,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T06:00:00+00:00",
      "phrase": "Intermittent clouds",
      "temperature": {
        "value": 55,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T07:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 54,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T08:00:00+00:00",
      "phrase": "Showers",
      "temperature": {
        "value": 53,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T09:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T10:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T11:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 50,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T12:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 50,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T13:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 49,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T14:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 48,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T15:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 49,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T16:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 50,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T17:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T18:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 52,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-05-01T19:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 53,
        "unit": "F"
      }
    }
  ],
  "dailyForecasts": [
    {
      "dateTime": "2021-04-30T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 48,
          "unit": "F"
        },
        "maximum": {
          "value": 63,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Showers"
      },
      "night": {
        "phrase": "Mostly cloudy"
      }
    },
    {
      "dateTime": "2021-05-01T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 45,
          "unit": "F"
        },
        "maximum": {
          "value": 57,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Mostly cloudy w/ showers"
      },
      "night": {
        "phrase": "Mostly cloudy"
      }
    },
    {
      "dateTime": "2021-05-02T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 44,
          "unit": "F"
        },
        "maximum": {
          "value": 61,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Intermittent clouds"
      },
      "night": {
        "phrase": "Intermittent clouds"
      }
    },
    {
      "dateTime": "2021-05-03T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 46,
          "unit": "F"
        },
        "maximum": {
          "value": 54,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Showers"
      },
      "night": {
        "phrase": "Showers"
      }
    },
    {
      "dateTime": "2021-05-04T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 41,
          "unit": "F"
        },
        "maximum": {
          "value": 61,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Mostly cloudy w/ showers"
      },
      "night": {
        "phrase": "Partly cloudy"
      }
    },
    {
      "dateTime": "2021-05-05T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 50,
          "unit": "F"
        },
        "maximum": {
          "value": 69,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Intermittent clouds"
      },
      "night": {
        "phrase": "Partly cloudy"
      }
    },
    {
      "dateTime": "2021-05-06T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 47,
          "unit": "F"
        },
        "maximum": {
          "value": 68,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Rain"
      },
      "night": {
        "phrase": "Intermittent clouds"
      }
    },
    {
      "dateTime": "2021-05-07T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 45,
          "unit": "F"
        },
        "maximum": {
          "value": 60,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Rain"
      },
      "night": {
        "phrase": "Showers"
      }
    },
    {
      "dateTime": "2021-05-08T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 46,
          "unit": "F"
        },
        "maximum": {
          "value": 61,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Mostly cloudy"
      },
      "night": {
        "phrase": "Dreary"
      }
    },
    {
      "dateTime": "2021-05-09T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 47,
          "unit": "F"
        },
        "maximum": {
          "value": 63,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Cloudy"
      },
      "night": {
        "phrase": "Intermittent clouds"
      }
    }
  ]
}
```
