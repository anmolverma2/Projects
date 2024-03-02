var express = require("express");
var router = express.Router();
var dbConn = require("../lib/db");
const sql = require("mssql");
const dal = require("../DAL");
const multer = require("multer");
const fs = require("fs");
const ExcelJS = require("exceljs");

// Get All Records
router.get("/", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "SELECT"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_User_Crud_Export_Bulk]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("users", { data: data });
    } else {
      req.flash("error", err);
      // render to views/users/index.ejs
      res.render("users", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("users", { data: [] }); // Render with an empty array in case of errors
  }
});

//Add Get
router.get("/add", function (req, res, next) {
  // render to add.ejs
  res.render("users/add", {
    UserName: "",
    EmailId: "",
    MobileNumber: "",
    Gender: "",
    Age: "",
    PaymentAmount: "",
    PaymentDate: "",
  });
});

// Add New User
router.post("/add", async function (req, res, next) {
  try {
    const {
      UserName,
      EmailId,
      MobileNumber,
      Gender,
      Age,
      PaymentAmount,
      // PaymentDate
    } = req.body;

    // Validation
    if (!UserName || UserName.trim().length === 0) {
      req.flash("error", "Please enter a valid UserName");
      return res.render("users/add", req.body);
    } else if (!EmailId || !isValidEmail(EmailId)) {
      req.flash("error", "Please enter a valid EmailId");
      return res.render("users/add", req.body);
    } else if (!MobileNumber || !isValidMobileNumber(MobileNumber)) {
      req.flash("error", "Please enter a valid MobileNumber");
      return res.render("users/add", req.body);
    } else if (!Gender || !isValidGender(Gender)) {
      req.flash("error", "Please select a valid Gender");
      return res.render("users/add", req.body);
    } else if (!Age || !isValidAge(Age)) {
      req.flash("error", "Please enter a valid Age");
      return res.render("users/add", req.body);
    } else if (!PaymentAmount || !isValidPaymentAmount(PaymentAmount)) {
      req.flash("error", "Please enter a valid PaymentAmount");
      return res.render("users/add", req.body);
    }
    // else if (!PaymentDate || !isValidDate(PaymentDate)) {
    //   req.flash("error", "Please select PaymentDate");
    //   return res.render("users/add", req.body);
    // }

    const parameters = {
      chvnUserName: UserName,
      chvnEmailId: EmailId,
      chvnMobileNumber: MobileNumber,
      chvnGender: Gender,
      chvnAge: Age,
      chvnPaymentAmount: PaymentAmount,
      //chvnPaymentDate: PaymentDate,
      chvnOperationType: "INSERT",
    };

    // Execute the stored procedure to insert data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_User_Crud_Export_Bulk]",
      parameters
    );

    if (result && result.recordset) {
      req.flash("success", "Client record successfully added");
      res.redirect("/users");
    } else {
      req.flash("error", "Failed to add Client record");
      res.render("users/add", req.body);
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error adding Client record");
    res.render("users/add", req.body);
  }
});

// display Edit User Page By UserId
router.get("/edit/:UserId", async function (req, res, next) {
  try {
    let UserId = parseInt(req.params.UserId); // Convert UserId to integer

    if (isNaN(UserId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid UserId");
      return res.redirect("/users");
    }

    // Call the stored procedure to fetch user details by ID with additional parameters
    var parameters = "";
    parameters = parameters + (await dal.SP_Parameters("@chvnUserId", UserId));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "SELECTBYID"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_User_Crud_Export_Bulk]",
      parameters
    );

    console.log(data);

    if (data != null) {
      // User found, render to edit.ejs with user details
      res.render("users/edit", {
        title: "Edit User",
        UserId: data[0].UserId,
        UserName: data[0].UserName,
        EmailId: data[0].EmailId,
        MobileNumber: data[0].MobileNumber,
        Gender: data[0].Gender,
        Age: data[0].Age,
        PaymentAmount: data[0].PaymentAmount,
        //        PaymentDate: data[0].PaymentDate
      });
    } else {
      // User not found
      req.flash("error", "User not found with UserId = " + UserId);
      res.redirect("/users");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error fetching user details");
    res.redirect("/users");
  }
});

router.post("/update/:UserId", async function (req, res, next) {
  try {
    let UserId = parseInt(req.params.UserId); // Convert UserId to integer

    if (isNaN(UserId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid UserId");
      return res.redirect("/users");
    }

    const {
      UserName,
      EmailId,
      MobileNumber,
      Gender,
      Age,
      PaymentAmount,
      //    PaymentDate
    } = req.body;

    // Validation (assuming functions like isValidEmail, isValidMobileNumber, etc., are defined)
    // Validation
    if (!UserName || UserName.trim().length === 0) {
      req.flash("error", "Please enter a valid UserName");
      return res.render("users/add", req.body);
    } else if (!EmailId || !isValidEmail(EmailId)) {
      req.flash("error", "Please enter a valid EmailId");
      return res.render("users/add", req.body);
    } else if (!MobileNumber || !isValidMobileNumber(MobileNumber)) {
      req.flash("error", "Please enter a valid MobileNumber");
      return res.render("users/add", req.body);
    } else if (!Gender || !isValidGender(Gender)) {
      req.flash("error", "Please select a valid Gender");
      return res.render("users/add", req.body);
    } else if (!Age || !isValidAge(Age)) {
      req.flash("error", "Please enter a valid Age");
      return res.render("users/add", req.body);
    } else if (!PaymentAmount || !isValidPaymentAmount(PaymentAmount)) {
      req.flash("error", "Please enter a valid PaymentAmount");
      return res.render("users/add", req.body);
    }
    //  else if (!PaymentDate || !isValidDate(PaymentDate)) {
    //   req.flash("error", "Please select PaymentDate");
    //   return res.render("users/add", req.body);
    // }

    // Call the stored procedure to fetch user details by ID with additional parameters
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnUserId", parseInt(UserId)));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnUserName", UserName));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnEmailId", EmailId));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnMobileNumber", MobileNumber));
    parameters = parameters + (await dal.SP_Parameters("@chvnGender", Gender));
    parameters = parameters + (await dal.SP_Parameters("@chvnAge", Age));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnPaymentAmount", PaymentAmount));
    //parameters = parameters + await dal.SP_Parameters('@chvnPaymentDate',PaymentDate);
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "UPDATE"));

    // Execute the stored procedure to update the record
    const result = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_User_Crud_Export_Bulk]",
      parameters
    );

    if (result != null) {
      req.flash("success", "Client record successfully updated");
      res.redirect("/users");
    } else {
      req.flash("error", "Failed to update Client record");
      res.render("users/edit", { ...req.body, UserId: req.params.UserId });
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error updating Client record");
    res.render("users/edit", { ...req.body, UserId: req.params.UserId });
  }
});

router.get("/delete/:UserId", async function (req, res, next) {
  try {
    let UserId = parseInt(req.params.UserId);

    var parameters = "";
    parameters = parameters + (await dal.SP_Parameters("@chvnUserId", UserId));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "DELETE"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_User_Crud_Export_Bulk]",
      parameters
    );
    console.log(data);
    if (data != null) {
      req.flash("success", "User deleted successfully");
      res.redirect("/users");
    } else {
      req.flash("No user found!");
      res.redirect("/users");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error deleting user");
    res.redirect("/users");
  }
});

router.get("/export", async (req, res) => {
  try {
    // Execute the stored procedure directly to fetch data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_User_Crud_Export_Bulk]",
      {
        chvnOperationType: "EXPORT",
      }
    );

    // Check if result contains data
    if (
      result &&
      result.recordset &&
      Array.isArray(result.recordset) &&
      result.recordset.length > 0
    ) {
      // Create Excel file with data
      const workbook = new ExcelJS.Workbook();
      const worksheet = workbook.addWorksheet("Sheet1");

      // Add column headers with space, color, and bold style
      const columnHeaders = Object.keys(result.recordset[0]);
      const headerRow = worksheet.addRow(columnHeaders);

      // Add data rows
      result.recordset.forEach((row) => {
        worksheet.addRow(Object.values(row));
      });

      // Set response headers for Excel file download
      const dateTimeString = new Date()
        .toLocaleString("en-GB", {
          day: "2-digit",
          month: "2-digit",
          year: "numeric",
          hour: "2-digit",
          minute: "2-digit",
          hour12: true, // Use 12-hour format
          hourCycle: "h12",
        })
        .replace(/\/|,| /g, "_"); // Format date and time for filename

      const filename = `Export_User_${dateTimeString}.xlsx`;

      res.setHeader(
        "Content-Type",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
      );
      res.setHeader(
        "Content-Disposition",
        `attachment; filename="${filename}"`
      );

      // Write the workbook to the response stream
      await workbook.xlsx.write(res);
      res.end();
    } else {
      // No data found or data structure issue, send an error response
      console.error("No data found or invalid data structure");
      res.status(404).send("No data found or invalid data structure");
    }
  } catch (error) {
    // Handle errors
    console.error("Error generating Excel file:", error);
    res.status(500).send("Error generating Excel file");
  }
});

// Validations
function isValidEmail(email) {
  // Simple email validation
  const emailRegex = /\S+@\S+\.\S+/;
  return emailRegex.test(email);
}

function isValidMobileNumber(number) {
  // Simple mobile number validation (could be more complex depending on your requirements)
  const numberRegex = /^[0-9]{10}$/;
  return numberRegex.test(number);
}

function isValidGender(gender) {
  // Simple gender validation (could be more complex depending on your requirements)
  const validGenders = ["Male", "Female", "Others"];
  return validGenders.includes(gender);
}

function isValidAge(age) {
  // Simple age validation (could be more complex depending on your requirements)
  return !isNaN(age) && parseInt(age) >= 18 && parseInt(age) <= 100; // Assuming age is between 18 and 100
}

function isValidPaymentAmount(amount) {
  // Simple payment amount validation (could be more complex depending on your requirements)
  return !isNaN(amount) && parseFloat(amount) > 0; // Assuming positive amounts only
}

module.exports = router;
