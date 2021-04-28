using System;
using System.Text.Json.Serialization;

namespace MinimalWeather
{
    public class WeatherSnapshot
    {
        public DateTime DateTime { get; set; }
        public string Phrase { get; set; }
        public Temperature Temperature { get; set; }
    }

    public class Temperature
    {
        public double Value { get; set; }
        public string Unit { get; set; }
    }

    public class WeatherResults
    {
        public WeatherSnapshot[] Results { get; set; }
    }
}
