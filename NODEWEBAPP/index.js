var createError = require("http-errors");
var express = require("express");
var path = require("path");
var flash = require("express-flash");
var session = require("express-session");
var sql = require("mssql");
var connection = require("./lib/db");
var masterRouter = require("./Controllers/master");
var kycRouter = require("./Controllers/salesmenKyc");
var paymentRouter = require("./Controllers/salesmanPayment");
var dsrRouter = require("./Controllers/DSR");
var app = express();
var apiRouter = require("api"); // Import your api module or router

// view engine setup
app.set("views", path.join(__dirname, "views"));
app.set("view engine", "ejs");
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(express.static(path.join(__dirname, "public")));

app.use("/Assests",express.static("Assests"));

//app.use(express.static(path.join(__dirname, "Assests/css")));

// Serve static files from the 'Demo' folder
app.use(express.static(path.join(__dirname, "Demo")));
app.use("/api", apiRouter); // Use the imported router for '/api' path

app.use(
  session({
    cookie: { maxAge: 60000 },
    store: new session.MemoryStore(),
    saveUninitialized: true,
    resave: "true",
    secret: "secret",
  })
);

app.use(flash());
app.use("/master", masterRouter);
app.use("/salesmenKyc", kycRouter);
app.use("/salesmanPayment", paymentRouter);
app.use("/DSR", dsrRouter);

// catch 404 and forward to error handler
app.use(function (req, res, next) {
  next(createError(404));
});

app.listen(3000);
