var express = require("express");
var router = express.Router();
var dbConn = require("../lib/db");
const sql = require("mssql");
const dal = require("../DAL");
const multer = require("multer");
const fs = require("fs");
const ExcelJS = require("exceljs");
const PDFDocument = require("pdfkit");

router.get("/", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "DSR"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_DSR_List_Export]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("DSR/manageDSR", { data: data });
    } else {
      req.flash("error", err);
      // render to views/DSR/index.ejs
      res.render("DSR/manageDSR", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("DSR/manageDSR", { data: [] }); // Render with an empty array in case of errors
  }
});

router.get("/searchDSR", async (req, res, next) => {
  try {
    const DSRCode = req.query.DSRCode;

    let query =
      "select DSRCode,DSRName,MobileNumber,AadharNumber,convert(nvarchar(1024),CreateDate,103) as CreateDate from [MasterManagement].[DSR] where IsDeleted=0 ";

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

    res.render("DSR/manageDSR", { data: result.recordset });
  } catch (error) {
    console.error("Error executing search query:", error);
    res.render("DSR/manageDSR", {
      data: "",
      error: "Error executing search query",
    });
  }
});

router.get("/exportDSR", async (req, res) => {
  try {
    // Execute the stored procedure directly to fetch data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_DSR_List_Export]",
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

      const filename = `Export_DSR_${dateTimeString}.xlsx`;

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

//#region DSR Bulk Upload

router.get("/dsrBulk", function (req, res, next) {
  // render to add.ejs
  res.render("DSR/dsrBulk");
});

const path = require("path");

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

router.post("/dsrBulk", upload.single("excelFile"), async (req, res) => {
  if (!req.file) {
    const errorMessage = "No file uploaded.";
    return res.render("DSR/dsrBulk", { errorMessage });
  }

  const filePath = req.file.path;
  const workbook = new ExcelJS.Workbook();

  try {
    await workbook.xlsx.readFile(filePath);
    const worksheet = workbook.getWorksheet(1); // Assuming data is on the first sheet

    if (worksheet.rowCount <= 1) {
      const errorMessage =
        "The uploaded Excel file is empty or does not contain data.";
      return res.render("DSR/dsrBulk", { errorMessage });
    }

    const expectedColumnNames = [
      "DSRCode",
      "DSRName",
      "MobileNumber",
      "AadharNumber",
      "City",
      "State",
    ];

    const headerRow = worksheet.getRow(1);
    const actualColumnNames = headerRow.values.map((value) => value.toString());

    if (!expectedColumnNames.every((col) => actualColumnNames.includes(col))) {
      const errorMessage =
        "Invalid column names in the Excel file. Please make sure the column names match: " +
        expectedColumnNames.join(", ");
      return res.render("DSR/dsrBulk", { errorMessage });
    }

    const dataToInsert = [];

    worksheet.eachRow((row, rowNumber) => {
      if (rowNumber > 1) {
        // Skip header row
        const rowData = {
          DSRCode: row.getCell(1).value,
          DSRName: row.getCell(2).value,
          MobileNumber: row.getCell(3).value,
          AadharNumber: row.getCell(4).value,
          City: row.getCell(5).value,
          State: row.getCell(6).value,
        };
        console.log(rowData);
        dataToInsert.push(rowData);
      }
    });

    const pool = await sql.connect(dbConn); // Use the provided database connection

    const tableName = "ManageDSRTempTable";
    const procedureName = "[WebApplication_SP].[usp_Truncate_Temp_Table_Bulk]";

    const request = pool.request();
    request.input("TableName", sql.NVarChar(128), tableName);

    const result = await request.execute(procedureName);

    if (result.returnValue === -1) {
      const errorMessage = "Invalid Destination Table Name!.";
      return res.render("DSR/dsrBulk", { errorMessage });
    } else {
      await Promise.all(
        dataToInsert.map(async (record) => {
          const { DSRCode, DSRName, MobileNumber, AadharNumber, City, State } =
            record;

          // Handle blank values by converting them to empty strings ''
          const safeDSRCode = DSRCode || "";
          const safeDSRName = DSRName || "";
          const safeMobileNumber = MobileNumber || "";
          const safeAadharNumber = AadharNumber || "";
          const safeCity = City || "";
          const safeState = State || "";

          const sql = `INSERT INTO ManageDSRTempTable (DSRCode, DSRName, MobileNumber, AadharNumber, City, State) VALUES ('${safeDSRCode}', '${safeDSRName}', '${safeMobileNumber}', '${safeAadharNumber}', '${safeCity}', '${safeState}')`;

          const result = await pool.request().query(sql);
          console.log(`Record inserted`);
        })
      );
    }

    const errorData = [];
    const procedureName2 = "[WebApplication_SP].[usp_Manage_DSR_BulkUpload]";
    // Execute the stored procedure
    const reslt = await pool.request().query(procedureName2);

    let renderData;

    if (reslt.recordset && reslt.recordset.length > 0) {
      // Errors occurred
      const errorData = reslt.recordset.map((record) => {
        const { DSRCode, DSRName, Error } = record;
        return { DSRCode, DSRName, Error };
      });
      renderData = {
        errorMessage:
          "Valid records upload successfully ! Please check the details of invalid records.",
        errorData,
      };
    } else {
      // No errors
      renderData = { successMessage: "DSR Uploaded Successfully." };
    }

    await pool.close(); // Close the connection pool

    return res.render("DSR/dsrBulk", renderData);
  } catch (error) {
    console.error("Error processing Excel file or inserting data:", error);
    const errorMessage =
      "Error processing Excel file or inserting data into the database.";
    return res.render("DSR/dsrBulk", { errorMessage });
  }
});

//#endregion

//#region Update DSR

router.get("/UpdateDSR", function (req, res, next) {
  // render to add.ejs
  res.render("DSR/updateDSR");
});

router.post("/UpdateDSR", upload.single("excelFile"), async (req, res) => {
  if (!req.file) {
    const errorMessage = "No file uploaded.";
    return res.render("DSR/updateDSR", { errorMessage });
  }

  const filePath = req.file.path;
  const workbook = new ExcelJS.Workbook();

  try {
    await workbook.xlsx.readFile(filePath);
    const worksheet = workbook.getWorksheet(1); // Assuming data is on the first sheet

    if (worksheet.rowCount <= 1) {
      const errorMessage =
        "The uploaded Excel file is empty or does not contain data.";
      return res.render("DSR/updateDSR", { errorMessage });
    }

    const expectedColumnNames = [
      "DSRCode",
      "DSRName",
      "MobileNumber",
      "AadharNumber",
      "City",
      "State",
    ];

    const headerRow = worksheet.getRow(1);
    const actualColumnNames = headerRow.values.map((value) => value.toString());

    if (!expectedColumnNames.every((col) => actualColumnNames.includes(col))) {
      const errorMessage =
        "Invalid column names in the Excel file. Please make sure the column names match: " +
        expectedColumnNames.join(", ");
      return res.render("DSR/updateDSR", { errorMessage });
    }

    const dataToInsert = [];

    worksheet.eachRow((row, rowNumber) => {
      if (rowNumber > 1) {
        // Skip header row
        const rowData = {
          DSRCode: row.getCell(1).value,
          DSRName: row.getCell(2).value,
          MobileNumber: row.getCell(3).value,
          AadharNumber: row.getCell(4).value,
          City: row.getCell(5).value,
          State: row.getCell(6).value,
        };
        console.log(rowData);
        dataToInsert.push(rowData);
      }
    });

    const pool = await sql.connect(dbConn); // Use the provided database connection

    const tableName = "ManageDSRTempTable";
    const procedureName = "[WebApplication_SP].[usp_Truncate_Temp_Table_Bulk]";

    const request = pool.request();
    request.input("TableName", sql.NVarChar(128), tableName);

    const result = await request.execute(procedureName);

    if (result.returnValue === -1) {
      const errorMessage = "Invalid Destination Table Name!.";
      return res.render("DSR/updateDSR", { errorMessage });
    } else {
      await Promise.all(
        dataToInsert.map(async (record) => {
          const { DSRCode, DSRName, MobileNumber, AadharNumber, City, State } =
            record;

          // Handle blank values by converting them to empty strings ''
          const safeDSRCode = DSRCode || "";
          const safeDSRName = DSRName || "";
          const safeMobileNumber = MobileNumber || "";
          const safeAadharNumber = AadharNumber || "";
          const safeCity = City || "";
          const safeState = State || "";

          const sql = `INSERT INTO ManageDSRTempTable (DSRCode, DSRName, MobileNumber, AadharNumber, City, State) VALUES ('${safeDSRCode}', '${safeDSRName}', '${safeMobileNumber}', '${safeAadharNumber}', '${safeCity}', '${safeState}')`;

          const result = await pool.request().query(sql);
          console.log(`Record inserted`);
        })
      );
    }

    const errorData = [];
    const procedureName2 = "[WebApplication_SP].[usp_Update_DSR_BulkUpload]";
    // Execute the stored procedure
    const reslt = await pool.request().query(procedureName2);

    let renderData;

    if (reslt.recordset && reslt.recordset.length > 0) {
      // Errors occurred
      const errorData = reslt.recordset.map((record) => {
        const { DSRCode, DSRName, MobileNumber, Error } = record;
        return { DSRCode, DSRName, MobileNumber, Error };
      });
      renderData = {
        errorMessage:
          "Valid records upload successfully ! Please check the details of invalid records.",
        errorData,
      };
    } else {
      // No errors
      renderData = { successMessage: "DSR Uploaded Successfully." };
    }

    await pool.close(); // Close the connection pool

    return res.render("DSR/updateDSR", renderData);
  } catch (error) {
    console.error("Error processing Excel file or inserting data:", error);
    const errorMessage =
      "Error processing Excel file or inserting data into the database.";
    return res.render("DSR/updateDSR", { errorMessage });
  }
});

//#endregion

//#region Remove KYC Bulk

router.get("/RemoveDSR", function (req, res, next) {
  // render to add.ejs
  res.render("DSR/removeDSR");
});

router.post("/RemoveDSR", upload.single("excelFile"), async (req, res) => {
  if (!req.file) {
    const errorMessage = "No file uploaded.";
    return res.render("DSR/removeDSR", { errorMessage });
  }

  const filePath = req.file.path;
  const workbook = new ExcelJS.Workbook();

  try {
    await workbook.xlsx.readFile(filePath);
    const worksheet = workbook.getWorksheet(1); // Assuming data is on the first sheet

    if (worksheet.rowCount <= 1) {
      const errorMessage =
        "The uploaded Excel file is empty or does not contain data.";
      return res.render("DSR/removeDSR", { errorMessage });
    }

    const expectedColumnNames = ["DSRId"];

    const headerRow = worksheet.getRow(1);
    const actualColumnNames = headerRow.values.map((value) => value.toString());

    if (!expectedColumnNames.every((col) => actualColumnNames.includes(col))) {
      const errorMessage =
        "Invalid column names in the Excel file. Please make sure the column names match: " +
        expectedColumnNames.join(", ");
      return res.render("DSR/removeDSR", { errorMessage });
    }

    const dataToInsert = [];

    worksheet.eachRow((row, rowNumber) => {
      if (rowNumber > 1) {
        // Skip header row
        const rowData = {
          DSRId: row.getCell(1).value,
        };
        console.log(rowData);
        dataToInsert.push(rowData);
      }
    });

    const pool = await sql.connect(dbConn);

    const tableName = "RemoveDSRTemp";
    const procedureName = "[WebApplication_SP].[usp_Truncate_Temp_Table_Bulk]";

    const request = pool.request();
    request.input("TableName", sql.NVarChar(128), tableName);

    const result = await request.execute(procedureName);

    if (result.returnValue === -1) {
      const errorMessage = "Invalid Destination Table Name!.";
      return res.render("DSR/removeDSR", { errorMessage });
    } else {
      await Promise.all(
        dataToInsert.map(async (record) => {
          const { DSRId } = record;
          const sql = `INSERT INTO RemoveDSRTemp (DSRId) VALUES ('${DSRId}')`;

          const result = await pool.request().query(sql);
          console.log(`Record inserted`);
        })
      );
    }

    const errorData = [];
    const procedureName2 = "[WebApplication_SP].[usp_Remove_DSR_BulkUpload]";
    // Execute the stored procedure
    const reslt = await pool.request().query(procedureName2);

    let renderData;

    if (reslt.recordset && reslt.recordset.length > 0) {
      // Errors occurred
      const errorData = reslt.recordset.map((record) => {
        const { DSRId, Error } = record;
        return { DSRId, Error };
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

    return res.render("DSR/removeDSR", renderData);
  } catch (error) {
    console.error("Error processing Excel file or inserting data:", error);
    const errorMessage =
      "Error processing Excel file or inserting data into the database.";
    return res.render("DSR/removeDSR", { errorMessage });
  }
});

//#endregion

module.exports = router;
