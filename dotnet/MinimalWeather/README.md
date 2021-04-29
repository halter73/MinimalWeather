## Program.cs

The current majority of the program lives in Program.cs.

TODO: Caching specific locations.
TODO: Maybe do something more interesting with the results.

```C#
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

app.Run();
```

Program.cs also *temporarily* includes these passthrough endpoints that copies the full response from the Azure Maps API. If there's anything needed from any of the `/proxyweather` endpoints please tell @halter73 before the demo.
```C#
// These endpoints are temporary to give UI devs a chance to grab data we're not forwarding yet.
app.MapGet("/proxyweather/{location}/current", (Func<Coordinate, Task<string>>)((Coordinate location) =>
     httpClient.GetStringAsync($"currentConditions/json?{baseQueryString}&query={location}")));

app.MapGet("/proxyweather/{location}/forecast/hourly", (Func<Coordinate, Task<string>>)((Coordinate location) =>
     httpClient.GetStringAsync($"forecast/hourly/json?{baseQueryString}&query={location}&duration=24")));

app.MapGet("/proxyweather/{location}/forecast/daily", (Func<Coordinate, Task<string>>)((Coordinate location) =>
     httpClient.GetStringAsync($"forecast/daily/json?{baseQueryString}&query={location}&duration=10")));

```

## Sample requests and responses

### Current Weather

https://localhost:5001/weather/47.6062,-122.3321/current

```json
{
  "results": [
    {
      "dateTime": "2021-04-29T00:59:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 60,
        "unit": "F"
      }
    }
  ]
}
```

https://localhost:5001/proxyweather/47.6062,-122.3321/current
<details>
  <summary>Click to expand.</summary>
  
```json
{
  "results": [
    {
      "dateTime": "2021-04-29T00:59:00+00:00",
      "phrase": "Cloudy",
      "iconCode": 7,
      "hasPrecipitation": false,
      "isDayTime": true,
      "temperature": {
        "value": 60,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 63,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperatureShade": {
        "value": 63,
        "unit": "F",
        "unitType": 18
      },
      "relativeHumidity": 60,
      "dewPoint": {
        "value": 47,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 315,
          "localizedDescription": "NW"
        },
        "speed": {
          "value": 1.7,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 3,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "uvIndex": 1,
      "uvIndexPhrase": "Low",
      "visibility": {
        "value": 7,
        "unit": "mi",
        "unitType": 2
      },
      "obstructionsToVisibility": "",
      "cloudCover": 99,
      "ceiling": {
        "value": 14700,
        "unit": "ft",
        "unitType": 0
      },
      "pressure": {
        "value": 30.28,
        "unit": "inHg",
        "unitType": 12
      },
      "pressureTendency": {
        "localizedDescription": "Steady",
        "code": "S"
      },
      "past24HourTemperatureDeparture": {
        "value": 3,
        "unit": "F",
        "unitType": 18
      },
      "apparentTemperature": {
        "value": 65,
        "unit": "F",
        "unitType": 18
      },
      "windChillTemperature": {
        "value": 60,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 53,
        "unit": "F",
        "unitType": 18
      },
      "precipitationSummary": {
        "pastHour": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "past3Hours": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "past6Hours": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "past9Hours": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "past12Hours": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "past18Hours": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "past24Hours": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        }
      },
      "temperatureSummary": {
        "past6Hours": {
          "minimum": {
            "value": 57,
            "unit": "F",
            "unitType": 18
          },
          "maximum": {
            "value": 64,
            "unit": "F",
            "unitType": 18
          }
        },
        "past12Hours": {
          "minimum": {
            "value": 47,
            "unit": "F",
            "unitType": 18
          },
          "maximum": {
            "value": 64,
            "unit": "F",
            "unitType": 18
          }
        },
        "past24Hours": {
          "minimum": {
            "value": 47,
            "unit": "F",
            "unitType": 18
          },
          "maximum": {
            "value": 64,
            "unit": "F",
            "unitType": 18
          }
        }
      }
    }
  ]
}
```
</details>

### Hourly Forecast

https://localhost:5001/weather/47.6062,-122.3321/forecast/hourly
<details>
  <summary>Click to expand.</summary>
  
```json
{
  "forecasts": [
    {
      "dateTime": "2021-04-29T02:00:00+00:00",
      "phrase": "Partly sunny",
      "temperature": {
        "value": 61,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T03:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 61,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T04:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 59,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T05:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 57,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T06:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 55,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T07:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 54,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T08:00:00+00:00",
      "phrase": "Cloudy",
      "temperature": {
        "value": 53,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T09:00:00+00:00",
      "phrase": "Mostly clear",
      "temperature": {
        "value": 52,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T10:00:00+00:00",
      "phrase": "Mostly clear",
      "temperature": {
        "value": 51,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T11:00:00+00:00",
      "phrase": "Mostly clear",
      "temperature": {
        "value": 50,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T12:00:00+00:00",
      "phrase": "Mostly clear",
      "temperature": {
        "value": 49,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T13:00:00+00:00",
      "phrase": "Partly sunny",
      "temperature": {
        "value": 47,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T14:00:00+00:00",
      "phrase": "Partly sunny",
      "temperature": {
        "value": 49,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T15:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 51,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T16:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 55,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T17:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 58,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T18:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 61,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T19:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 64,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T20:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 67,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T21:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 69,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T22:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 71,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-29T23:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 73,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-30T00:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 72,
        "unit": "F"
      }
    },
    {
      "dateTime": "2021-04-30T01:00:00+00:00",
      "phrase": "Mostly cloudy",
      "temperature": {
        "value": 70,
        "unit": "F"
      }
    }
  ]
}
```
</details>
https://localhost:5001/proxyweather/47.6062,-122.3321/forecast/hourly
<details>
  <summary>Click to expand.</summary>
  
```json
{
  "forecasts": [
    {
      "date": "2021-04-29T02:00:00+00:00",
      "iconCode": 3,
      "iconPhrase": "Partly sunny",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 61,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 60,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 52,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 282,
          "localizedDescription": "WNW"
        },
        "speed": {
          "value": 6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "direction": {
          "degrees": 282,
          "localizedDescription": "WNW"
        },
        "speed": {
          "value": 7,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 54,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 45,
      "ceiling": {
        "value": 17500,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T03:00:00+00:00",
      "iconCode": 7,
      "iconPhrase": "Cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 61,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 59,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 51,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 42,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 307,
          "localizedDescription": "NW"
        },
        "speed": {
          "value": 6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "direction": {
          "degrees": 307,
          "localizedDescription": "NW"
        },
        "speed": {
          "value": 8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 50,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 95,
      "ceiling": {
        "value": 18900,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T04:00:00+00:00",
      "iconCode": 7,
      "iconPhrase": "Cloudy",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 59,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 57,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 50,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 42,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 339,
          "localizedDescription": "NNW"
        },
        "speed": {
          "value": 6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "direction": {
          "degrees": 339,
          "localizedDescription": "NNW"
        },
        "speed": {
          "value": 8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 54,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 95,
      "ceiling": {
        "value": 20800,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T05:00:00+00:00",
      "iconCode": 7,
      "iconPhrase": "Cloudy",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 57,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 55,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 50,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 44,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 358,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "direction": {
          "degrees": 358,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 7,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 63,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 95,
      "ceiling": {
        "value": 22600,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T06:00:00+00:00",
      "iconCode": 7,
      "iconPhrase": "Cloudy",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 55,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 54,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 50,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 10,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 5,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "direction": {
          "degrees": 10,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 7,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 67,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 95,
      "ceiling": {
        "value": 24500,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T07:00:00+00:00",
      "iconCode": 7,
      "iconPhrase": "Cloudy",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 54,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 53,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 44,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 10,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 5,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "direction": {
          "degrees": 10,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 7,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 69,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 95,
      "ceiling": {
        "value": 24500,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T08:00:00+00:00",
      "iconCode": 7,
      "iconPhrase": "Cloudy",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 53,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 52,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 8,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 6.9,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 74,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 95,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T09:00:00+00:00",
      "iconCode": 34,
      "iconPhrase": "Mostly clear",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 52,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 52,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 8,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 5.8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 76,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 26,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T10:00:00+00:00",
      "iconCode": 34,
      "iconPhrase": "Mostly clear",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 51,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 50,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 48,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 12,
          "localizedDescription": "NNE"
        },
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 5.8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 79,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 26,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T11:00:00+00:00",
      "iconCode": 34,
      "iconPhrase": "Mostly clear",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 50,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 48,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 18,
          "localizedDescription": "NNE"
        },
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 5.8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 84,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 26,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T12:00:00+00:00",
      "iconCode": 34,
      "iconPhrase": "Mostly clear",
      "hasPrecipitation": false,
      "isDaylight": false,
      "temperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 48,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 46,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 21,
          "localizedDescription": "NNE"
        },
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 5.8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 90,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 26,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T13:00:00+00:00",
      "iconCode": 3,
      "iconPhrase": "Partly sunny",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 47,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 47,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 47,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 46,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 19,
          "localizedDescription": "NNE"
        },
        "speed": {
          "value": 3.5,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 5.8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 96,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 42,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 0,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T14:00:00+00:00",
      "iconCode": 3,
      "iconPhrase": "Partly sunny",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 52,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 47,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 46,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 11,
          "localizedDescription": "N"
        },
        "speed": {
          "value": 3.5,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 90,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 45,
      "ceiling": {
        "value": 24500,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 1,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T15:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 51,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 55,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 49,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 47,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 260,
          "localizedDescription": "W"
        },
        "speed": {
          "value": 2.3,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 3.5,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 85,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 24500,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 1,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T16:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 55,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 60,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 51,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 46,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 208,
          "localizedDescription": "SSW"
        },
        "speed": {
          "value": 3.5,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 74,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 26300,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 2,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T17:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 58,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 64,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 51,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 201,
          "localizedDescription": "SSW"
        },
        "speed": {
          "value": 4.6,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 5.8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 61,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 3,
      "uvIndexPhrase": "Moderate",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T18:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 61,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 67,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 52,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 44,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 200,
          "localizedDescription": "SSW"
        },
        "speed": {
          "value": 5.8,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 8.1,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 54,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 4,
      "uvIndexPhrase": "Moderate",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T19:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 64,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 71,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 54,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 45,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 204,
          "localizedDescription": "SSW"
        },
        "speed": {
          "value": 6.9,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 8.1,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 49,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 5,
      "uvIndexPhrase": "Moderate",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T20:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 67,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 72,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 54,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 43,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 208,
          "localizedDescription": "SSW"
        },
        "speed": {
          "value": 6.9,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 9.2,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 42,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 5,
      "uvIndexPhrase": "Moderate",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T21:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 69,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 73,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 54,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 42,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 211,
          "localizedDescription": "SSW"
        },
        "speed": {
          "value": 8.1,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 10.4,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 37,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 5,
      "uvIndexPhrase": "Moderate",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T22:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 71,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 73,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 54,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 40,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 216,
          "localizedDescription": "SW"
        },
        "speed": {
          "value": 8.1,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 10.4,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 33,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 4,
      "uvIndexPhrase": "Moderate",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-29T23:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 73,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 74,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 55,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 40,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 221,
          "localizedDescription": "SW"
        },
        "speed": {
          "value": 9.2,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 11.5,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 30,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 3,
      "uvIndexPhrase": "Moderate",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-30T00:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 72,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 72,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 54,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 38,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 224,
          "localizedDescription": "SW"
        },
        "speed": {
          "value": 9.2,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 12.7,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 29,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 2,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    },
    {
      "date": "2021-04-30T01:00:00+00:00",
      "iconCode": 6,
      "iconPhrase": "Mostly cloudy",
      "hasPrecipitation": false,
      "isDaylight": true,
      "temperature": {
        "value": 70,
        "unit": "F",
        "unitType": 18
      },
      "realFeelTemperature": {
        "value": 69,
        "unit": "F",
        "unitType": 18
      },
      "wetBulbTemperature": {
        "value": 53,
        "unit": "F",
        "unitType": 18
      },
      "dewPoint": {
        "value": 38,
        "unit": "F",
        "unitType": 18
      },
      "wind": {
        "direction": {
          "degrees": 227,
          "localizedDescription": "SW"
        },
        "speed": {
          "value": 9.2,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "windGust": {
        "speed": {
          "value": 12.7,
          "unit": "mi/h",
          "unitType": 9
        }
      },
      "relativeHumidity": 31,
      "visibility": {
        "value": 10,
        "unit": "mi",
        "unitType": 2
      },
      "cloudCover": 76,
      "ceiling": {
        "value": 30000,
        "unit": "ft",
        "unitType": 0
      },
      "uvIndex": 1,
      "uvIndexPhrase": "Low",
      "precipitationProbability": 0,
      "rainProbability": 0,
      "snowProbability": 0,
      "iceProbability": 0,
      "totalLiquid": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "rain": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "snow": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      },
      "ice": {
        "value": 0,
        "unit": "in",
        "unitType": 1
      }
    }
  ]
}
```
</details>

### Daily Forecast

https://localhost:5001/weather/47.6062,-122.3321/forecast/daily
<details>
  <summary>Click to expand.</summary>
  
```json
{
  "forecasts": [
    {
      "dateTime": "2021-04-28T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 47,
          "unit": "F"
        },
        "maximum": {
          "value": 64,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Intermittent clouds"
      },
      "night": {
        "phrase": "Mostly cloudy"
      }
    },
    {
      "dateTime": "2021-04-29T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 52,
          "unit": "F"
        },
        "maximum": {
          "value": 73,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Mostly cloudy"
      },
      "night": {
        "phrase": "Partly cloudy w/ showers"
      }
    },
    {
      "dateTime": "2021-04-30T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 48,
          "unit": "F"
        },
        "maximum": {
          "value": 62,
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
      "dateTime": "2021-05-01T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 45,
          "unit": "F"
        },
        "maximum": {
          "value": 59,
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
      "dateTime": "2021-05-02T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 44,
          "unit": "F"
        },
        "maximum": {
          "value": 60,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Intermittent clouds"
      },
      "night": {
        "phrase": "Mostly cloudy"
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
          "value": 57,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Cloudy"
      },
      "night": {
        "phrase": "Intermittent clouds"
      }
    },
    {
      "dateTime": "2021-05-04T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 47,
          "unit": "F"
        },
        "maximum": {
          "value": 64,
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
      "dateTime": "2021-05-05T14:00:00+00:00",
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
        "phrase": "Cloudy"
      },
      "night": {
        "phrase": "Rain"
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
          "value": 65,
          "unit": "F"
        }
      },
      "day": {
        "phrase": "Cloudy"
      },
      "night": {
        "phrase": "Rain"
      }
    },
    {
      "dateTime": "2021-05-07T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 46,
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
        "phrase": "Mostly cloudy"
      }
    }
  ]
}
```
</details>
https://localhost:5001/proxyweather/47.6062,-122.3321/forecast/daily
<details>
  <summary>Click to expand.</summary>
  
```json
{
  "summary": {
    "startDate": "2021-04-30T09:00:00+00:00",
    "endDate": "2021-05-01T21:00:00+00:00",
    "severity": 5,
    "phrase": "Expect showery weather late tomorrow night through Saturday morning",
    "category": "rain"
  },
  "forecasts": [
    {
      "date": "2021-04-28T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 47,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 64,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 47,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 71,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 47,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 65,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 2.4,
      "degreeDaySummary": {
        "heating": {
          "value": 9,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 37,
          "category": "Good",
          "categoryValue": 1,
          "type": "Particle Pollution"
        },
        {
          "name": "Grass",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 750,
          "category": "High",
          "categoryValue": 3
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "UVIndex",
          "value": 6,
          "category": "High",
          "categoryValue": 3
        }
      ],
      "day": {
        "iconCode": 4,
        "iconPhrase": "Intermittent clouds",
        "hasPrecipitation": false,
        "shortPhrase": "Clouds and sun",
        "longPhrase": "Intervals of clouds and sunshine",
        "precipitationProbability": 1,
        "thunderstormProbability": 0,
        "rainProbability": 0,
        "snowProbability": 1,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 241,
            "localizedDescription": "WSW"
          },
          "speed": {
            "value": 4,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 241,
            "localizedDescription": "WSW"
          },
          "speed": {
            "value": 7,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 60
      },
      "night": {
        "iconCode": 38,
        "iconPhrase": "Mostly cloudy",
        "hasPrecipitation": false,
        "shortPhrase": "Clouds breaking",
        "longPhrase": "Clouds breaking",
        "precipitationProbability": 3,
        "thunderstormProbability": 0,
        "rainProbability": 3,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 12,
            "localizedDescription": "NNE"
          },
          "speed": {
            "value": 5,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 12,
            "localizedDescription": "NNE"
          },
          "speed": {
            "value": 8,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 63
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-04-29T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 52,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 73,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 47,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 74,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 47,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 70,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 1.9,
      "degreeDaySummary": {
        "heating": {
          "value": 3,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 41,
          "category": "Good",
          "categoryValue": 1,
          "type": "Particle Pollution"
        },
        {
          "name": "Grass",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 300,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 8,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "UVIndex",
          "value": 5,
          "category": "Moderate",
          "categoryValue": 2
        }
      ],
      "day": {
        "iconCode": 6,
        "iconPhrase": "Mostly cloudy",
        "hasPrecipitation": false,
        "shortPhrase": "Mostly cloudy and warm",
        "longPhrase": "Warm with clouds and intervals of sunshine",
        "precipitationProbability": 7,
        "thunderstormProbability": 0,
        "rainProbability": 7,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 213,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 7,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 213,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 12,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 76
      },
      "night": {
        "iconCode": 39,
        "iconPhrase": "Partly cloudy w/ showers",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Partly cloudy, a shower late",
        "longPhrase": "Partly cloudy with a shower in places late",
        "precipitationProbability": 40,
        "thunderstormProbability": 8,
        "rainProbability": 40,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 198,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 8,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 198,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 11,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.01,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.01,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0.5,
        "hoursOfRain": 0.5,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 47
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-04-30T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 48,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 62,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 44,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 60,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 44,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 60,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 0.1,
      "degreeDaySummary": {
        "heating": {
          "value": 10,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 17,
          "category": "Good",
          "categoryValue": 1,
          "type": "Particle Pollution"
        },
        {
          "name": "Grass",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 450,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 15,
          "category": "Moderate",
          "categoryValue": 2
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "UVIndex",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        }
      ],
      "day": {
        "iconCode": 12,
        "iconPhrase": "Showers",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Clouds, a little rain; cooler",
        "longPhrase": "Considerable clouds with occasional rain; cooler",
        "precipitationProbability": 60,
        "thunderstormProbability": 2,
        "rainProbability": 60,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 197,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 9,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 197,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 12,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.12,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.12,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 2,
        "hoursOfRain": 2,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 96
      },
      "night": {
        "iconCode": 12,
        "iconPhrase": "Showers",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Mostly cloudy, a shower or two",
        "longPhrase": "Mainly cloudy with a brief shower or two",
        "precipitationProbability": 56,
        "thunderstormProbability": 11,
        "rainProbability": 56,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 191,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 6,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 191,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 10,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.06,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.06,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 2,
        "hoursOfRain": 2,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 76
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-05-01T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 45,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 59,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 41,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 61,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 41,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 57,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 1.3,
      "degreeDaySummary": {
        "heating": {
          "value": 13,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 17,
          "category": "Good",
          "categoryValue": 1,
          "type": "Particle Pollution"
        },
        {
          "name": "Grass",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 608,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 480,
          "category": "High",
          "categoryValue": 3
        },
        {
          "name": "UVIndex",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        }
      ],
      "day": {
        "iconCode": 12,
        "iconPhrase": "Showers",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "A shower in the a.m.; cloudy",
        "longPhrase": "A passing shower in the morning; otherwise, cloudy",
        "precipitationProbability": 55,
        "thunderstormProbability": 11,
        "rainProbability": 55,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 191,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 5.8,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 207,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 9.2,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.02,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.02,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0.5,
        "hoursOfRain": 0.5,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 95
      },
      "night": {
        "iconCode": 38,
        "iconPhrase": "Mostly cloudy",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Mostly cloudy, a shower late",
        "longPhrase": "Mostly cloudy with a shower in spots late",
        "precipitationProbability": 44,
        "thunderstormProbability": 9,
        "rainProbability": 44,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 96,
            "localizedDescription": "E"
          },
          "speed": {
            "value": 4.6,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 13,
            "localizedDescription": "NNE"
          },
          "speed": {
            "value": 6.9,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.02,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.02,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0.5,
        "hoursOfRain": 0.5,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 81
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-05-02T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 44,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 60,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 41,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 68,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 41,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 59,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 3.7,
      "degreeDaySummary": {
        "heating": {
          "value": 13,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 41,
          "category": "Good",
          "categoryValue": 1,
          "type": "Nitrogen Dioxide"
        },
        {
          "name": "Grass",
          "value": 1,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 669,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 915,
          "category": "High",
          "categoryValue": 3
        },
        {
          "name": "UVIndex",
          "value": 5,
          "category": "Moderate",
          "categoryValue": 2
        }
      ],
      "day": {
        "iconCode": 4,
        "iconPhrase": "Intermittent clouds",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Clouds and sun with a shower",
        "longPhrase": "Clouds and sun with a passing shower",
        "precipitationProbability": 55,
        "thunderstormProbability": 11,
        "rainProbability": 55,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 271,
            "localizedDescription": "W"
          },
          "speed": {
            "value": 4.6,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 14,
            "localizedDescription": "NNE"
          },
          "speed": {
            "value": 6.9,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.04,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.04,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 1,
        "hoursOfRain": 1,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 73
      },
      "night": {
        "iconCode": 38,
        "iconPhrase": "Mostly cloudy",
        "hasPrecipitation": false,
        "shortPhrase": "Mostly cloudy",
        "longPhrase": "Mostly cloudy",
        "precipitationProbability": 25,
        "thunderstormProbability": 0,
        "rainProbability": 25,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 103,
            "localizedDescription": "ESE"
          },
          "speed": {
            "value": 5.8,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 185,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 10.4,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 79
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-05-03T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 46,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 57,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 38,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 56,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 38,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 54,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 2.1,
      "degreeDaySummary": {
        "heating": {
          "value": 13,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 0,
          "category": "Good",
          "categoryValue": 1,
          "type": "Ozone"
        },
        {
          "name": "Grass",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 736,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 1377,
          "category": "High",
          "categoryValue": 3
        },
        {
          "name": "UVIndex",
          "value": 3,
          "category": "Moderate",
          "categoryValue": 2
        }
      ],
      "day": {
        "iconCode": 7,
        "iconPhrase": "Cloudy",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Cloudy with a shower",
        "longPhrase": "Cloudy with a shower",
        "precipitationProbability": 57,
        "thunderstormProbability": 11,
        "rainProbability": 57,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 193,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 8.1,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 194,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 12.7,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.09,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.09,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 1.5,
        "hoursOfRain": 1.5,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 92
      },
      "night": {
        "iconCode": 36,
        "iconPhrase": "Intermittent clouds",
        "hasPrecipitation": false,
        "shortPhrase": "Partly cloudy",
        "longPhrase": "Partly cloudy",
        "precipitationProbability": 23,
        "thunderstormProbability": 1,
        "rainProbability": 23,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 191,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 10.4,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 189,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 17.3,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 70
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-05-04T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 47,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 64,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 48,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 65,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 48,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 62,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 4.2,
      "degreeDaySummary": {
        "heating": {
          "value": 9,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 0,
          "category": "Good",
          "categoryValue": 1,
          "type": "Ozone"
        },
        {
          "name": "Grass",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 920,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 1904,
          "category": "Very High",
          "categoryValue": 4
        },
        {
          "name": "UVIndex",
          "value": 3,
          "category": "Moderate",
          "categoryValue": 2
        }
      ],
      "day": {
        "iconCode": 4,
        "iconPhrase": "Intermittent clouds",
        "hasPrecipitation": false,
        "shortPhrase": "A shower possible",
        "longPhrase": "A blend of sun and clouds with a shower possible",
        "precipitationProbability": 30,
        "thunderstormProbability": 5,
        "rainProbability": 30,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 215,
            "localizedDescription": "SW"
          },
          "speed": {
            "value": 5.8,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 219,
            "localizedDescription": "SW"
          },
          "speed": {
            "value": 13.8,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 73
      },
      "night": {
        "iconCode": 36,
        "iconPhrase": "Intermittent clouds",
        "hasPrecipitation": false,
        "shortPhrase": "Partly cloudy",
        "longPhrase": "Partly cloudy",
        "precipitationProbability": 16,
        "thunderstormProbability": 0,
        "rainProbability": 16,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 224,
            "localizedDescription": "SW"
          },
          "speed": {
            "value": 2.3,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 224,
            "localizedDescription": "SW"
          },
          "speed": {
            "value": 8.1,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 57
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-05-05T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 48,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 67,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 38,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 65,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 38,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 65,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 1.2,
      "degreeDaySummary": {
        "heating": {
          "value": 7,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 0,
          "category": "Good",
          "categoryValue": 1,
          "type": "Ozone"
        },
        {
          "name": "Grass",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 1150,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 1973,
          "category": "Very High",
          "categoryValue": 4
        },
        {
          "name": "UVIndex",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        }
      ],
      "day": {
        "iconCode": 7,
        "iconPhrase": "Cloudy",
        "hasPrecipitation": false,
        "shortPhrase": "A couple of showers possible",
        "longPhrase": "Cloudy with a couple of showers possible",
        "precipitationProbability": 30,
        "thunderstormProbability": 5,
        "rainProbability": 30,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 153,
            "localizedDescription": "SSE"
          },
          "speed": {
            "value": 5.8,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 154,
            "localizedDescription": "SSE"
          },
          "speed": {
            "value": 23,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 96
      },
      "night": {
        "iconCode": 18,
        "iconPhrase": "Rain",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Rain",
        "longPhrase": "Rain",
        "precipitationProbability": 80,
        "thunderstormProbability": 0,
        "rainProbability": 80,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 179,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 9.2,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 191,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 26.5,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.59,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.59,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 9,
        "hoursOfRain": 9,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 97
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-05-06T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 47,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 65,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 39,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 62,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 39,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 61,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 1.8,
      "degreeDaySummary": {
        "heating": {
          "value": 9,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 0,
          "category": "Good",
          "categoryValue": 1,
          "type": "Ozone"
        },
        {
          "name": "Grass",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 1553,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 2006,
          "category": "Very High",
          "categoryValue": 4
        },
        {
          "name": "UVIndex",
          "value": 3,
          "category": "Moderate",
          "categoryValue": 2
        }
      ],
      "day": {
        "iconCode": 7,
        "iconPhrase": "Cloudy",
        "hasPrecipitation": false,
        "shortPhrase": "Overcast with rain possible",
        "longPhrase": "Considerable cloudiness with rain possible",
        "precipitationProbability": 35,
        "thunderstormProbability": 0,
        "rainProbability": 35,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 192,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 12.7,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 193,
            "localizedDescription": "SSW"
          },
          "speed": {
            "value": 32.2,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 92
      },
      "night": {
        "iconCode": 18,
        "iconPhrase": "Rain",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "Rain",
        "longPhrase": "Rain",
        "precipitationProbability": 74,
        "thunderstormProbability": 4,
        "rainProbability": 74,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 178,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 10.4,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 170,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 26.5,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.31,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.31,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 5.5,
        "hoursOfRain": 5.5,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 95
      },
      "sources": [
        "AccuWeather"
      ]
    },
    {
      "date": "2021-05-07T14:00:00+00:00",
      "temperature": {
        "minimum": {
          "value": 46,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 63,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperature": {
        "minimum": {
          "value": 43,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 66,
          "unit": "F",
          "unitType": 18
        }
      },
      "realFeelTemperatureShade": {
        "minimum": {
          "value": 43,
          "unit": "F",
          "unitType": 18
        },
        "maximum": {
          "value": 64,
          "unit": "F",
          "unitType": 18
        }
      },
      "hoursOfSun": 2.5,
      "degreeDaySummary": {
        "heating": {
          "value": 11,
          "unit": "F",
          "unitType": 18
        },
        "cooling": {
          "value": 0,
          "unit": "F",
          "unitType": 18
        }
      },
      "airAndPollen": [
        {
          "name": "AirQuality",
          "value": 0,
          "category": "Good",
          "categoryValue": 1,
          "type": "Ozone"
        },
        {
          "name": "Grass",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Mold",
          "value": 2330,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Ragweed",
          "value": 0,
          "category": "Low",
          "categoryValue": 1
        },
        {
          "name": "Tree",
          "value": 2022,
          "category": "Very High",
          "categoryValue": 4
        },
        {
          "name": "UVIndex",
          "value": 2,
          "category": "Low",
          "categoryValue": 1
        }
      ],
      "day": {
        "iconCode": 7,
        "iconPhrase": "Cloudy",
        "hasPrecipitation": false,
        "shortPhrase": "A couple of showers possible",
        "longPhrase": "Considerable cloudiness with a couple of showers possible",
        "precipitationProbability": 30,
        "thunderstormProbability": 5,
        "rainProbability": 30,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 33,
            "localizedDescription": "NNE"
          },
          "speed": {
            "value": 3.5,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 350,
            "localizedDescription": "N"
          },
          "speed": {
            "value": 10.4,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0,
        "hoursOfRain": 0,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 88
      },
      "night": {
        "iconCode": 38,
        "iconPhrase": "Mostly cloudy",
        "hasPrecipitation": true,
        "precipitationType": "Rain",
        "precipitationIntensity": "Light",
        "shortPhrase": "A shower early; mostly cloudy",
        "longPhrase": "A shower in spots in the evening; otherwise, rather cloudy",
        "precipitationProbability": 40,
        "thunderstormProbability": 8,
        "rainProbability": 40,
        "snowProbability": 0,
        "iceProbability": 0,
        "wind": {
          "direction": {
            "degrees": 125,
            "localizedDescription": "SE"
          },
          "speed": {
            "value": 4.6,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "windGust": {
          "direction": {
            "degrees": 170,
            "localizedDescription": "S"
          },
          "speed": {
            "value": 25.3,
            "unit": "mi/h",
            "unitType": 9
          }
        },
        "totalLiquid": {
          "value": 0.01,
          "unit": "in",
          "unitType": 1
        },
        "rain": {
          "value": 0.01,
          "unit": "in",
          "unitType": 1
        },
        "snow": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "ice": {
          "value": 0,
          "unit": "in",
          "unitType": 1
        },
        "hoursOfPrecipitation": 0.5,
        "hoursOfRain": 0.5,
        "hoursOfSnow": 0,
        "hoursOfIce": 0,
        "cloudCover": 74
      },
      "sources": [
        "AccuWeather"
      ]
    }
  ]
}
```
</details>