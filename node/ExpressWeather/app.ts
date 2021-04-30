import * as cors from 'cors';
import * as express from 'express';
import got from 'got';

const app = express();

var baseUrl = "https://atlas.microsoft.com/weather/";
var baseQuery = `api-version=1.0&subscription-key=${process.env["SubscriptionKey"]}&unit=imperial`;

app.get('/weather/:lat,:lon', cors(), async (req, res, next) => {
    try {
        const lat = parseFloat(req.params.lat);
        const lon = parseFloat(req.params.lon);

        const currentQuery = got(`${baseUrl}currentConditions/json?${baseQuery}&query=${lat},${lon}`);
        const hourlyQuery = got(`${baseUrl}forecast/hourly/json?${baseQuery}&query=${lat},${lon}&duration=24`);
        const dailyQuery = got(`${baseUrl}forecast/daily/json?${baseQuery}&query=${lat},${lon}&duration=10`);

        const [currentResponse, hourlyResponse, dailyResponse] = await Promise.all([currentQuery, hourlyQuery, dailyQuery]);

        const currentWeather = JSON.parse(currentResponse.body);
        const hourlyForecast = JSON.parse(hourlyResponse.body);
        const dailyForecast = JSON.parse(dailyResponse.body);

        await res.json({
            currentWeather: currentWeather.results[0],
            hourlyForcasts: hourlyForecast.forecasts,
            dailyForecasts: dailyForecast.forecasts,
        });
    } catch (err) {
        next(err);
    }
});

const port = process.env.PORT || 3000

app.listen(port, function () {
    console.log(`Express server listening on port ${port}`);
});
