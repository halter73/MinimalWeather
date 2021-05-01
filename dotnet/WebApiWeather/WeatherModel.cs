using System;

namespace WebApiWeather
{
    public class WeatherSnapshot
    {
        public DateTimeOffset DateTime { get; set; }
        public string Phrase { get; set; }
        public Temperature Temperature { get; set; }

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

    public class PhraseOnly
    {
        public string Phrase { get; set; }

        public string IconPhrase
        {
            set { Phrase = value; }
        }
    }

    public class FullDayForecast
    {
        public DateTimeOffset DateTime { get; set; }
        public MinMaxTemperature Temperature { get; set; }

        public PhraseOnly Day { get; set; }
        public PhraseOnly Night { get; set;  }

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
    }

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

            coordinate = new(lat, lon);
            return true;
        }

        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}
