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
