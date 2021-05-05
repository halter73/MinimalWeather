﻿using System;

namespace MinimalWeather
{
    public class WeatherSnapshot
    {
        public DateTimeOffset DateTime { get; set; }
        public string Phrase { get; set; }
        public Temperature Temperature { get; set; }
        public int RelativeHumidity { get; set; }
        public int? PrecipitationProbability { get; set; }

        public DateTimeOffset Date
        {
            set { DateTime = value; }
        }

        public string IconPhrase
        {
            set { Phrase = value; }
        }
    }

    public class Temperature
    {
        public double Value { get; set; }
        public string Unit { get; set; }
    }

    public class MinMaxTemperature
    {
        public Temperature Minimum { get; set; }
        public Temperature Maximum { get; set; }
    }

    public class DayOrNight
    {
        public string Phrase { get; set; }

        public int PrecipitationProbability { get; set; }

        public string IconPhrase
        {
            set { Phrase = value; }
        }
    }

    public class FullDayForecast
    {
        public DateTimeOffset DateTime { get; set; }
        public MinMaxTemperature Temperature { get; set; }

        public DayOrNight Day { get; set; }
        public DayOrNight Night { get; set;  }

        public DateTimeOffset Date
        {
            set { DateTime = value; }
        }
    }

    public class CurrentWeather
    {
        public WeatherSnapshot[] Results { get; set; }
    }

    public class HourlyForecast
    {
        public WeatherSnapshot[] Forecasts { get; set; }
    }

    public class DailyForecast
    {
        public FullDayForecast[] Forecasts { get; set; }
    }

    public class CombinedWeather
    {
        public WeatherSnapshot CurrentWeather { get; set; }
        public WeatherSnapshot[] HourlyForecasts { get; set; }
        public FullDayForecast[] DailyForecasts { get; set; }

        public string DebugProperty { get; set; }
    }

    public record TimeZoneResponse(TimeZone[] TimeZones);
    public record TimeZone(string Id, ReferenceTime ReferenceTime);
    public record ReferenceTime(string StandardOffset, string DaylightSavings);

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
}
