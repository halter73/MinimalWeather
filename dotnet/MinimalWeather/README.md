## Build instructions

To build the [main](https://github.com/halter73/MinimalWeather/tree/main/dotnet/MinimalWeather) branch relying on the natural type for lambdas, you can checkout the [features/compiler roslyn branch](https://github.com/dotnet/roslyn/tree/features/compiler) and set "Roslyn.VisualStudio.Setup" as your startup project to launch VS. Then you need to add something like `<CscToolPath>C:\dev\dotnet\roslyn\artifacts\bin\csc\Debug\net472</CscToolPath>` to the csproj.
 
If you just want to test in C# 9 for now, you can just checkout the [csharp-nine](https://github.com/halter73/MinimalWeather/tree/csharp-nine/dotnet/MinimalWeather) branch I just pushed that does a bunch of casting. Either way you'll need a preview4 sdk which you can get from dotnet/installer.

## Program.cs

The current majority of the program lives in Program.cs.

```C#
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

## Sample requests and response

### Current Weather

https://localhost:5001/weather/47.6062,-122.3321/

```json
{
  "currentWeather": {
    "dateTime": "2021-05-03T19:36:00+00:00",
    "phrase": "Cloudy",
    "temperature": {
      "value": 56,
      "unit": "F"
    },
    "relativeHumidity": 70,
    "precipitationProbability": null
  },
  "hourlyForecasts": [
    {
      "dateTime": "2021-05-03T20:00:00+00:00",
      "phrase": "Showers",
      "temperature": {
        "value": 56,
        "unit": "F"
      },
      "relativeHumidity": 63,
      "precipitationProbability": 40
    },
    {
      "dateTime": "2021-05-03T21:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 55,
        "unit": "F"
      },
      "relativeHumidity": 64,
      "precipitationProbability": 34
    },
    {
      "dateTime": "2021-05-03T22:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 56,
        "unit": "F"
      },
      "relativeHumidity": 63,
      "precipitationProbability": 28
    },
    {
      "dateTime": "2021-05-03T23:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 57,
        "unit": "F"
      },
      "relativeHumidity": 62,/current
      "precipitationProbability": 29
    },
    {
      "dateTime": "2021-05-04T00:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 57,
        "unit": "F"
      },
      "relativeHumidity": 59,
      "precipitationProbability": 32
    },
    {
      "dateTime": "2021-05-04T01:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 57,
        "unit": "F"
      },
      "relativeHumidity": 59,
      "precipitationProbability": 32
    },
    {
      "dateTime": "2021-05-04T02:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 56,
        "unit": "F"
      },
      "relativeHumidity": 60,
      "precipitationProbability": 25
    },
    {
      "dateTime": "2021-05-04T03:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 56,
        "unit": "F"
      },
      "relativeHumidity": 62,
      "precipitationProbability": 12
    },
    {
      "dateTime": "2021-05-04T04:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 56,
        "unit": "F"
      },
      "relativeHumidity": 63,
      "precipitationProbability": 12
    },
    {
      "dateTime": "2021-05-04T05:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 55,
        "unit": "F"
      },
      "relativeHumidity": 66,
      "precipitationProbability": 16
    },
    {
      "dateTime": "2021-05-04T06:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 55,
        "unit": "F"
      },
      "relativeHumidity": 68,
      "precipitationProbability": 47
    },
    {
      "dateTime": "2021-05-04T07:00:00+00:00",
      "phrase": "Showers",
      "temperature": {
        "value": 54,
        "unit": "F"
      },
      "relativeHumidity": 69,
      "precipitationProbability": 51
    },
    {
      "dateTime": "2021-05-04T08:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 54,
        "unit": "F"
      },
      "relativeHumidity": 70,
      "precipitationProbability": 47
    },
    {
      "dateTime": "2021-05-04T09:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 53,
        "unit": "F"
      },
      "relativeHumidity": 70,
      "precipitationProbability": 32
    },
    {
      "dateTime": "2021-05-04T10:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 53,
        "unit": "F"
      },
      "relativeHumidity": 70,
      "precipitationProbability": 32
    },
    {
      "dateTime": "2021-05-04T11:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 52,
        "unit": "F"
      },
      "relativeHumidity": 71,
      "precipitationProbability": 36
    },
    {
      "dateTime": "2021-05-04T12:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      },
      "relativeHumidity": 74,
      "precipitationProbability": 43
    },
    {
      "dateTime": "2021-05-04T13:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      },
      "relativeHumidity": 74,
      "precipitationProbability": 47
    },
    {
      "dateTime": "2021-05-04T14:00:00+00:00",
      "phrase": "Showers",
      "temperature": {
        "value": 49,
        "unit": "F"
      },
      "relativeHumidity": 78,
      "precipitationProbability": 51
    },
    {
      "dateTime": "2021-05-04T15:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      },
      "relativeHumidity": 77,
      "precipitationProbability": 5
    },
    {
      "dateTime": "2021-05-04T16:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      },
      "relativeHumidity": 73,
      "precipitationProbability": 2
    },
    {
      "dateTime": "2021-05-04T17:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 52,
        "unit": "F"
      },
      "relativeHumidity": 67,
      "precipitationProbability": 2
    },
    {
      "dateTime": "2021-05-04T18:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 54,
        "unit": "F"
      },
      "relativeHumidity": 63,
      "precipitationProbability": 2
    },
    {
      "dateTime": "2021-05-04T19:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 56,
        "unit": "F"
      },
      "relativeHumidity": 58,
      "precipitationProbability": 2
    }
  ],
  "dailyForecasts": [
    {
      "dateTime": "2021-05-03T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 49,
          "unit": "F"
        },
        "maximum": {
          "value": 57,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Cloudy",
        "precipitationProbability": 47
      },
      "night": {
        "phrase": "Showers",
        "precipitationProbability": 40
      }
    },
    {
      "dateTime": "2021-05-04T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 43,
          "unit": "F"
        },
        "maximum": {
          "value": 62,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Mostly cloudy",
        "precipitationProbability": 9
      },
      "night": {
        "phrase": "Partly cloudy",
        "precipitationProbability": 1
      }
    },
    {
      "dateTime": "2021-05-05T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 53,
          "unit": "F"
        },
        "maximum": {
          "value": 70,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Mostly cloudy",
        "precipitationProbability": 2
      },
      "night": {
        "phrase": "Partly cloudy",
        "precipitationProbability": 18
      }
    },
    {
      "dateTime": "2021-05-06T14:00:00+00:00",
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
        "phrase": "Rain",
        "precipitationProbability": 70
      },
      "night": {
        "phrase": "Mostly cloudy w/ showers",
        "precipitationProbability": 55
      }
    },
    {
      "dateTime": "2021-05-07T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 42,
          "unit": "F"
        },
        "maximum": {
          "value": 54,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Showers",
        "precipitationProbability": 80
      },
      "night": {
        "phrase": "Rain",
        "precipitationProbability": 62
      }
    },
    {
      "dateTime": "2021-05-08T14:00:00+00:00",
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
        "phrase": "Mostly cloudy",
        "precipitationProbability": 25
      },
      "night": {
        "phrase": "Intermittent clouds",
        "precipitationProbability": 25
      }
    },
    {
      "dateTime": "2021-05-09T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 45,
          "unit": "F"
        },
        "maximum": {
          "value": 63,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Showers",
        "precipitationProbability": 35
      },
      "night": {
        "phrase": "Intermittent clouds",
        "precipitationProbability": 20
      }
    },
    {
      "dateTime": "2021-05-10T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 46,
          "unit": "F"
        },
        "maximum": {
          "value": 65,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Mostly cloudy",
        "precipitationProbability": 30
      },
      "night": {
        "phrase": "Intermittent clouds",
        "precipitationProbability": 24
      }
    },
    {
      "dateTime": "2021-05-11T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 48,
          "unit": "F"
        },
        "maximum": {
          "value": 67,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Partly sunny",
        "precipitationProbability": 25
      },
      "night": {
        "phrase": "Mostly cloudy",
        "precipitationProbability": 21
      }
    },
    {
      "dateTime": "2021-05-12T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 49,
          "unit": "F"
        },
        "maximum": {
          "value": 68,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Cloudy",
        "precipitationProbability": 30
      },
      "night": {
        "phrase": "Mostly cloudy",
        "precipitationProbability": 16
      }
    }
  ]
}
```
