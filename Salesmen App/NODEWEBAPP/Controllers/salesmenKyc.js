var express = require("express");
var router = express.Router();
var dbConn = require("../lib/db");
const sql = require("mssql");
const dal = require("../DAL");
const multer = require("multer");
const fs = require("fs");
const ExcelJS = require("exceljs");
const PDFDocument = require("pdfkit");
const path = require("path");

router.get("/", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "ALLKYC"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Get_KYC_Insert]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("kyc/auditKyc", { data: data });
    } else {
      req.flash("error", err);
      // render to views/kyc/index.ejs
      res.render("kyc/auditKyc", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("kyc/auditKyc", { data: [] }); // Render with an empty array in case of errors
  }
});

router.get("/search", async (req, res, next) => {
  try {
    const DSRCode = req.query.DSRCode;

    // Construct the base query
    let query =
      "select b.SalesmanKYCId,a.DSRCode,a.DSRName,b.MobileNumber,b.KycStatus,convert(nvarchar(512),b.CreateDate,103) as CreateDate  from [MasterManagement].[DSR] a inner join [SalesmanManagement].[SalesmanKYC]  b on a.DSRId=b.DSRId where  b.IsDeleted=0";

    // Create an array to store the query parameters
    const queryParams = [];

    // Check and append search conditions for each field
    if (DSRCode) {
      query += " AND a.DSRCode LIKE @DSRCode";
      queryParams.push({
        name: "DSRCode",
        type: sql.NVarChar,
        value: `%${DSRCode}%`,
      });
    }

    // Add the final ORDER BY clause
    query += " ORDER BY a.DSRId DESC";

    const pool = await sql.connect(dbConn);

    const result = await pool
      .request()
      .input("DSRCode", sql.NVarChar, `%${DSRCode}%`)
      .query(query);

    sql.close();

    res.render("kyc/auditKyc", { data: result.recordset });
  } catch (error) {
    console.error("Error executing search query:", error);
    res.render("kyc/auditKyc", {
      data: "",
      error: "Error executing search query",
    });
  }
});

//#region Image upload
const storeImage = multer.diskStorage({
  destination: function (req, file, cb) {
    const uploadDir = "uploads/images/";
    // Check if the directory exists, if not, create it
    if (!fs.existsSync(uploadDir)) {
      fs.mkdirSync(uploadDir, { recursive: true });
    }
    cb(null, uploadDir); // Set the destination folder for images
  },
  filename: function (req, file, cb) {
    cb(
      null,
      file.fieldname + "-" + Date.now() + path.extname(file.originalname)
    );
  },
});

// Initialize multer upload for images
const uploadImage = multer({
  storage: storeImage,
  limits: { fileSize: 5000000 }, // 5MB file size limit
  fileFilter: function (req, file, cb) {
    checkFileType(file, cb);
  },
}).single("SalesmanPhoto"); // 'SalesmanPhoto' is the name attribute of the file input field in the form

// Check file type
function checkFileType(file, cb) {
  const filetypes = /jpeg|jpg|png|gif/;
  const extname = filetypes.test(path.extname(file.originalname).toLowerCase());
  const mimetype = filetypes.test(file.mimetype);

  if (mimetype && extname) {
    return cb(null, true);
  } else {
    cb("Error: Only images are allowed!");
  }
}

//#endregion

//#region Add Kyc
router.get("/addDsrKyc/:DSRId", async function (req, res, next) {
  let DId = parseInt(req.params.DSRId);
  var parameters = "";

  parameters = parameters + (await dal.SP_Parameters("@chvnDSRId", DId));
  parameters =
    parameters +
    (await dal.SP_Parameters("@chvnOperationType", "SELECTBYKYCID"));
  const dsrData = await dal.SPExecuteDataTable(
    "[WebApplication_SP].[SP_Get_KYC_Insert]",
    parameters
  );

  var parameters2 = "";

  parameters2 =
    parameters2 + (await dal.SP_Parameters("@chvnOperationType2", "COUNTRY"));
  const countryData = await dal.SPExecuteDataTable(
    "[WebApplication_SP].[SP_Country_State_City_Dropdown]",
    parameters2
  );

  if (dsrData != null) {
    var firstDsrRecord = dsrData[0];
    var DSRCode = firstDsrRecord.DSRCode;
    var DSRName = firstDsrRecord.DSRName;
    var DSRId = firstDsrRecord.DSRId;
    res.render("kyc/addDsrKyc", {
      title: "Add Salesman KYC",
      countryData: countryData,
      DSRId: DSRId,
      DSRCode: DSRCode,
      DSRName: DSRName,
      SalesmanKYCId: "",
      MobileNumber: "",
      EmailId: "",
      AddressLine1: "",
      AddressLine2: "",
      CountryName: "",
      StateName: "",
      CityName: "",
      PinCode: "",
      Gender: "",
      DateofBirth: null,
      PANNumber: "",
      SalesmanPhoto: "",
    });
  } else {
    //alert("No user found!");
    res.redirect("/salesmenKyc/KycRecords");
  }
});

router.post("/addkycData/:DSRId", async function (req, res, next) {
  try {
    uploadImage(req, res, async function (err) {
      if (err) {
        req.flash("error", err);
        return res.redirect("/salesmenKyc/KycRecords");
      } else {
        let DSRId = parseInt(req.params.DSRId);

        console.log(DSRId);
        const {
          DSRCode,
          DSRName,
          MobileNumber,
          EmailId,
          AddressLine1,
          AddressLine2,
          CountryName,
          StateName,
          CityName,
          PinCode,
          Gender,
          DateofBirth,
          PANNumber,
          SalesmanPhoto,
        } = req.body;

        const parameters = {
          chvnDSRId: DSRId,
          chvnDSRName: DSRName,
          chvnDSRCode: DSRCode,
          chvnMobileNumber: MobileNumber,
          chvnEmailId: EmailId,
          chvnAddressLine1: AddressLine1,
          chvnAddressLine2: AddressLine2,
          chvnCountryId: CountryName,
          chvnStateId: StateName,
          chvnCityId: CityName,
          chvnPinCode: PinCode,
          chvnGender: Gender,
          chvnDateofBirth: DateofBirth,
          chvnPANNumber: PANNumber,
          chvnSalesmanPhoto: "/uploads/images/" + req.file.filename, // Get the filename of the uploaded photos
          chvnOperationType: "INSERTKYC",
        };

        // Execute the stored procedure to insert data
        const result = await dal.executeStoredProcedure(
          "[WebApplication_SP].[SP_Get_KYC_Insert]",
          parameters
        );

        if (result && result.recordset) {
          req.flash("success", "KYC submitted successfully");
          res.redirect("/salesmenKyc/kycRecords");
        } else {
          req.flash("error", "Failed to add KYC data");
          res.render("/kyc/addDsrKyc", req.body);
        }
      }
    });
  } catch (error) {
    console.error(error);
    req.flash("error", "Error adding Salesman KYC");
    res.render("/kyc/kycRecords", req.body);
  }
});

//#endregion

//#region Audit Kyc

router.get("/viewKYC/:SalesmanKYCId", async function (req, res, next) {
  try {
    let SalesmanKYCId = parseInt(req.params.SalesmanKYCId); // Convert UserId to integer

    if (isNaN(SalesmanKYCId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid SalesmanKYCId ");
      return res.redirect("/salesmenKyc/");
    }

    // Call the stored procedure to fetch user details by ID with additional parameters
    var parameters = "";
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnSalesmanKYCId", SalesmanKYCId));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "GETKYCBYID"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Get_KYC_Insert]",
      parameters
    );

    console.log(data);

    if (data != null) {
      // User found, render to edit.ejs with user details
      res.render("kyc/viewDSRKyc", {
        title: "Edit User",
        SalesmanKYCId: data[0].SalesmanKYCId,
        DSRCode: data[0].DSRCode,
        DSRName: data[0].DSRName,
        MobileNumber: data[0].MobileNumber,
        EmailId: data[0].EmailId,
        AddressLine1: data[0].AddressLine1,
        AddressLine2: data[0].AddressLine2,
        PinCode: data[0].PinCode,
        Gender: data[0].Gender,
        PANNumber: data[0].PANNumber,
        KycStatus: data[0].KycStatus,
        RejectComment: data[0].RejectComment,
      });
    } else {
      // User not found
      req.flash("error", "KYC is not Enabled with mapped DSRCode = " + DSRCode);
      res.redirect("/salesmenKyc/");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error fetching KYC details");
    res.redirect("/salesmenKyc/");
  }
});

router.post("/UpdateAuditKyc/:SalesmanKYCId", async function (req, res, next) {
  try {
    let SalesmanKYCId = parseInt(req.params.SalesmanKYCId);

    if (isNaN(SalesmanKYCId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid SalesmanKYCId");
      return res.redirect("/kyc/auditKyc");
    }

    var parameters = "";
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnSalesmanKYCId", parseInt(SalesmanKYCId)));

    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "APPROVEKYC"));

    // Execute the stored procedure to update the record
    const result = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Get_KYC_Insert]",
      parameters
    );

    if (result != null) {
      req.flash("success", "KYC Approved successfully");
      res.redirect("/salesmenKyc/");
    } else {
      req.flash("error", "Failed to Audit KYC ");
      res.render("kyc/auditKyc", {
        ...req.body,
        SalesmanKYCId: req.params.SalesmanKYCId,
      });
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error in KYC Audit ");
    res.render("kyc/auditKyc", {
      ...req.body,
      SalesmanKYCId: req.params.SalesmanKYCId,
    });
  }
});

router.post("/RejectAuditKyc/:SalesmanKYCId", async function (req, res, next) {
  try {
    let SalesmanKYCId = parseInt(req.params.SalesmanKYCId);

    if (isNaN(SalesmanKYCId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid SalesmanKYCId");
      return res.redirect("/kyc/auditKyc");
    }

    const RejectComment = req.body.RejectComment;

    var parameters = "";
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnSalesmanKYCId", parseInt(SalesmanKYCId)));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnRejectComment", RejectComment));

    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "REJECTKYC"));

    // Execute the stored procedure to update the record
    const result = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Get_KYC_Insert]",
      parameters
    );

    if (result != null) {
      req.flash("success", "KYC Rejected successfully");
      res.redirect("/salesmenKyc/");
    } else {
      req.flash("error", "Failed to Audit KYC ");
      res.render("kyc/auditKyc", {
        ...req.body,
        SalesmanKYCId: req.params.SalesmanKYCId,
      });
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error in KYC Audit ");
    res.render("kyc/auditKyc", {
      ...req.body,
      SalesmanKYCId: req.params.SalesmanKYCId,
    });
  }
});

//#endregion

//#region Validations
function isValidPhoneNumber(phoneNumber) {
  const phoneRegex = /^[0-9]{10}$/;
  return phoneRegex.test(phoneNumber);
}

function isValidEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

function isValidPinCode(pinCode) {
  const pinCodeRegex = /^[1-9][0-9]{5}$/;
  return pinCodeRegex.test(pinCode);
}

function isValidPANNumber(panNumber) {
  const panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;
  return panRegex.test(panNumber);
}
//#endregion

//#region DSR KYC Records
router.get("/KycRecords", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "KYC"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Get_KYC_Insert]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("kyc/kycRecords", { data: data });
    } else {
      req.flash("error", err);
      // render to views/kyc/index.ejs
      res.render("kyc/kycRecords", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("kyc/kycRecords", { data: [] }); // Render with an empty array in case of errors
  }
});

router.get("/searchDSR", async (req, res, next) => {
  try {
    const DSRCode = req.query.DSRCode;

    let query =
      "select DSRId,DSRCode,DSRName from [MasterManagement].[DSR] where IsDeleted=0";

    const queryParams = [];

    if (DSRCode) {
      query += " AND DSRCode LIKE @DSRCode";
      queryParams.push({
        name: "DSRCode",
        type: sql.NVarChar,
        value: `%${DSRCode}%`,
      });
    }

    query += " ORDER BY DSRId DESC";

    const pool = await sql.connect(dbConn);

    const result = await pool
      .request()
      .input("DSRCode", sql.NVarChar, `%${DSRCode}%`)
      .query(query);

    sql.close();

    res.render("kyc/kycRecords", { data: result.recordset });
  } catch (error) {
    console.error("Error executing search query:", error);
    res.render("kyc/kycRecords", {
      data: "",
      error: "Error executing search query",
    });
  }
});

//#endregion

//#region Export KYC

router.get("/exportKyc", async (req, res) => {
  try {
    // Execute the stored procedure directly to fetch data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_Get_KYC_Insert]",
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

      headerRow.font = {
        color: { rgb: "FFFFFF" }, // White text color
      };
      headerRow.eachCell((cell, colNumber) => {
        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: { argb: "FF6495ED" }, // Light gray background color
        };
        cell.font = {
          bold: true,
        };

        // Set the width of the header cells
        worksheet.getColumn(colNumber).width =
          cell.value.length < 15 ? 15 : cell.value.length;
      });

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

      const filename = `Export_Salesman_KYC_${dateTimeString}.xlsx`;

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

//#endregion

//#region Remove KYC Bulk

router.get("/removeKyc", function (req, res, next) {
  // render to add.ejs
  res.render("kyc/removeKyc");
});

const storage = multer.diskStorage({
  destination: function (req, file, cb) {
    const uploadDir = "UsersUpload/";

    // Check if the 'uploads' directory exists, create it if not
    if (!fs.existsSync(uploadDir)) {
      fs.mkdirSync(uploadDir);
    }

    cb(null, uploadDir);
  },
  filename: function (req, file, cb) {
    const originalname = path.parse(file.originalname).name;
    const dateNow = new Date().toISOString().replace(/[-:]/g, "");

    cb(null, `${originalname}_${dateNow}${path.extname(file.originalname)}`);
  },
});

const upload = multer({ storage: storage });

router.post("/removeKyc", upload.single("excelFile"), async (req, res) => {
  if (!req.file) {
    const errorMessage = "No file uploaded.";
    return res.render("kyc/removeKyc", { errorMessage });
  }

  const filePath = req.file.path;
  const workbook = new ExcelJS.Workbook();

  try {
    await workbook.xlsx.readFile(filePath);
    const worksheet = workbook.getWorksheet(1); // Assuming data is on the first sheet

    if (worksheet.rowCount <= 1) {
      const errorMessage =
        "The uploaded Excel file is empty or does not contain data.";
      return res.render("kyc/removeKyc", { errorMessage });
    }

    const expectedColumnNames = ["SalesmanKYCId"];

    const headerRow = worksheet.getRow(1);
    const actualColumnNames = headerRow.values.map((value) => value.toString());

    if (!expectedColumnNames.every((col) => actualColumnNames.includes(col))) {
      const errorMessage =
        "Invalid column names in the Excel file. Please make sure the column names match: " +
        expectedColumnNames.join(", ");
      return res.render("kyc/removeKyc", { errorMessage });
    }

    const dataToInsert = [];

    worksheet.eachRow((row, rowNumber) => {
      if (rowNumber > 1) {
        // Skip header row
        const rowData = {
          SalesmanKYCId: row.getCell(1).value,
        };
        console.log(rowData);
        dataToInsert.push(rowData);
      }
    });

    const pool = await sql.connect(dbConn); // Use the provided database connection

    const tableName = "RemoveSalesmanKYCTemp";
    const procedureName = "[WebApplication_SP].[usp_Truncate_Temp_Table_Bulk]";

    const request = pool.request();
    request.input("TableName", sql.NVarChar(128), tableName);

    const result = await request.execute(procedureName);

    if (result.returnValue === -1) {
      const errorMessage = "Invalid Destination Table Name!.";
      return res.render("kyc/removeKyc", { errorMessage });
    } else {
      await Promise.all(
        dataToInsert.map(async (record) => {
          const { SalesmanKYCId } = record;
          const sql = `INSERT INTO RemoveSalesmanKYCTemp (SalesmanKYCId) VALUES ('${SalesmanKYCId}')`;

          const result = await pool.request().query(sql);
          console.log(`Record inserted`);
        })
      );
    }

    const errorData = [];
    const procedureName2 =
      "[WebApplication_SP].[usp_Remove_SalesmanKYC_BulkUpload]";
    // Execute the stored procedure
    const reslt = await pool.request().query(procedureName2);

    let renderData;

    if (reslt.recordset && reslt.recordset.length > 0) {
      // Errors occurred
      const errorData = reslt.recordset.map((record) => {
        const { SalesmanKYCId, Error } = record;
        return { SalesmanKYCId, Error };
      });
      renderData = {
        errorMessage:
          "Valid records upload successfully ! Please check the details of invalid records.",
        errorData,
      };
    } else {
      // No errors
      renderData = { successMessage: "KYC Removed Successfully." };
    }

    await pool.close(); // Close the connection pool

    return res.render("kyc/removeKyc", renderData);
  } catch (error) {
    console.error("Error processing Excel file or inserting data:", error);
    const errorMessage =
      "Error processing Excel file or inserting data into the database.";
    return res.render("kyc/removeKyc", { errorMessage });
  }
});

//#endregion

module.exports = router;
