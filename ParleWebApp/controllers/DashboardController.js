const sql = require('mssql');
const conn = require('../DB/conn');
const dal = require('../DAL');
const { Dashboard } = require('../models/DashboardModel');
var parentUUID = '';

var DashboardDetails = async (req,res) =>{
  try {
      const UUID = req.query.UUID; 

      const dashboard = new Dashboard(UUID);
   
      if (!UUID) {
        return res.status(400)
        .render("../views/shared/_LayoutResponsive", { errorMessage: "UUID parameter is required",pageContent: "error404" });
      }
      
      var data = await dashboard.DashboardDetails(UUID);
      var user = await dashboard.ProfileDetails(UUID);

      if (!data) {
        return res.status(404)
        .render("../views/shared/_LayoutResponsive", { errorMessage: "Dashboard details not found",pageContent: "error404" });
      }
      
      res.render("../views/shared/_LayoutResponsive", {
        pageContent: "../Parle/DSR/dashboard",
        data: data,
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
  DashboardDetails: DashboardDetails
}