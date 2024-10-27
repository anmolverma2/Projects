const createError = require('http-errors');
const express = require('express');
const path = require('path');
const flash = require('express-flash');
const session = require('express-session');
const connection = require('./lib/db');
const usersRouter = require('./routes/users');

const app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(express.static(path.join(__dirname, 'public')));

app.use(session({ 
    cookie: { maxAge: 60000 },
    store: new session.MemoryStore(),
    saveUninitialized: true,
    resave: true,
    secret: 'secret'
}));

app.use(flash());

// Middleware to use the database connection
app.use((req, res, next) => {
  // Example: Fetch data from a database using the connection pool
  connection.pool.query('SELECT * FROM Employee', (err, result) => {
    if (err) {
      console.error('Error executing query:', err);
      req.dbError = err; // Store error in request object for further processing if needed
    } else {
      console.log('Database query result:', result);
      req.dbResult = result; // Store result in request object for further processing if needed
    }
    next();
  });
});

app.use('/users', usersRouter);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404));
});

// Error handler
app.use(function(err, req, res, next) {
  console.error(err);
  res.status(err.status || 500);
  res.send('An error occurred');
});

app.listen(3000, () => {
  console.log('Server started on port 3000');
});
