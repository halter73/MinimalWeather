import * as express from 'express';

const app = express();

app.get('/', (req, res) => {
    throw new Error();
    res.send('Hello World!');
});

app.use((req, res, next) => {
    const err = new Error('Not Found');
    err[ 'status' ] = 404;
    next(err);
});

if (app.get('env') === 'development') {
    app.use((err, req, res, next) => {
        res.status(err[ 'status' ] || 500);
        res.send();
    });
}

const port = process.env.PORT || 3000

app.listen(port, function () {
    console.log(`Express server listening on port ${port}`);
});