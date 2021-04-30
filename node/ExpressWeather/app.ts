import * as express from 'express';

const app = express();

app.get('/', (req, res) => {
    res.send('Hello World!');
});

const port = process.env.PORT || 3000

app.listen(port, function () {
    console.log(`Express server listening on port ${port}`);
});