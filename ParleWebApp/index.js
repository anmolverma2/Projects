const conn = require("./DB/conn");
const sql = require("mssql");
const express = require("express");
const bodyParser = require("body-parser");
const path = require("path");
var session = require("express-session");
const salesmenController = require("./controllers/SalesmenController");
const dashboardController = require("./controllers/DashboardController");
const profileController = require("./controllers/ProfileController");
const paymentController = require("./controllers/PaymentController");


const app = express();
const route = express.Router();

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(express.static(path.resolve("../", "public")));

app.use("/public", express.static("public"));
app.use("/uploads",express.static("uploads"));

app.set("view engine", "ejs");
app.set("views", "views");

app.use(
  session({
    cookie: { maxAge: 60000 },
    store: new session.MemoryStore(),
    saveUninitialized: true,
    resave: "true",
    secret: "secret",
  })
);
  

app.get("/Dashboard", dashboardController.DashboardDetails);

 app.get("/SOProfile", profileController.Profile);
 
 app.post("/InsertOTP",profileController.SendOTP);

 app.post("/VerifyOTP",profileController.ValidateOTP);
 
 app.post("/UpdateSOProfileDetails",profileController.UpdateProfile);

 app.get("/GetParentID", salesmenController.ParentID);

 app.get("/SalesmenKYC", salesmenController.SalesmenKYC);

 app.get("/UploadDocument", salesmenController.UploadDocument);
 
 app.get("/SalesmenPayment",paymentController.SalesmenPayment);

 app.post("/PersonalDetail",salesmenController.PersonalDetail);

 app.post("/uploadImage", salesmenController.uploadImage);


var port = process.env.PORT || 2000;
app.listen(port, () => {
  console.log(`Server is running at port : ${port}`);
});
