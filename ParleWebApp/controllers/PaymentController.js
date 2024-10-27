const sql = require('mssql');
const conn = require('../DB/conn');
const dal = require('../DAL');
const { Payment } = require('../models/PaymentModel');
var parentUUID = '';

var SalesmenPayment = async (req,res) =>{
    try {
        const UUID = req.query.UUID == '' ? parentUUID : req.query.UUID; 
        const payment = new Payment(UUID);
     
        if (!UUID) {
          return res.status(400)
          .render("../views/shared/_LayoutResponsive", { errorMessage: "UUID parameter is required",pageContent: "error404" });
        }
        
        //var data = await payment.DSRDetails(UUID);
        var user = await payment.ProfileDetails(UUID);

        if (!user) {
          return res.status(404)
          .render("../views/shared/_LayoutResponsive", { errorMessage: "DSR details not found",pageContent: "error404" });
        }
        
        res.render("../views/shared/_LayoutResponsive", {
          pageContent: "../Parle/DSR/payment",
        //  data: data,
          user: user,
          UUID: UUID
        });
      } catch (err) {
        console.error("Error:", err);
        res.status(500)
        .render("../views/shared/_LayoutResponsive", { errorMessage: "Internal Server Error",pageContent: "error404" });
      }

}

module.exports = {
    SalesmenPayment: SalesmenPayment
}