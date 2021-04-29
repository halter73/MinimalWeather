using System;

namespace MinimalWeather
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
}
