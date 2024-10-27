var express = require("express");
var router = express.Router();
var dbConn = require("../lib/db");
const sql = require("mssql");
const dal = require("../DAL");
const multer = require("multer");
const fs = require("fs");
const ExcelJS = require("exceljs");
const PDFDocument = require("pdfkit");

//#region Payment List

router.get("/", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "PAYMENTRECORDS"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Salesman_Payment_Export]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("payment/paymentList", { data: data });
    } else {
      req.flash("error", err);
      // render to views/users/index.ejs
      res.render("payment/paymentList", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("payment/paymentList", { data: [] }); // Render with an empty array in case of errors
  }
});

router.get("/search", async (req, res, next) => {
  try {
    const DSRCode = req.query.DSRCode;

    let query =
      "SELECT a.SalesmanPaymentId,a.SalesmanKYCId,c.DSRCode,a.Incentive,a.UTRNumber,a.PaymentStatus,convert(nvarchar(520),a.CreateDate,103) as PaymentDate FROM [SalesmanManagement].[SalesmanPayment] a inner join [SalesmanManagement].[SalesmanKYC] b on a.SalesmanKYCId=b.SalesmanKYCId inner join MasterManagement.DSR c on b.DSRId=c.DSRId where a.IsDeleted=0 and b.IsDeleted=0";

    const queryParams = [];

    if (DSRCode) {
      query += " AND c.DSRCode LIKE @DSRCode";
      queryParams.push({
        name: "DSRCode",
        type: sql.NVarChar,
        value: `%${DSRCode}%`,
      });
    }

    query += " ORDER BY c.DSRId DESC";

    const pool = await sql.connect(dbConn);

    const result = await pool
      .request()
      .input("DSRCode", sql.NVarChar, `%${DSRCode}%`)
      .query(query);

    sql.close();

    res.render("payment/paymentList", { data: result.recordset });
  } catch (error) {
    console.error("Error executing search query:", error);
    res.render("payment/paymentList", {
      data: "",
      error: "Error executing search query",
    });
  }
});

//#endregion

//#region Change Payment Status

router.post(
  "/OnHoldStatus/:SalesmanPaymentId",
  async function (req, res, next) {
    try {
      let SalesmanPaymentId = parseInt(req.params.SalesmanPaymentId);

      if (isNaN(SalesmanPaymentId)) {
        // Handle the case when UserId is not a valid number
        req.flash("error", "Invalid SalesmanPaymentId");
        return res.redirect("/salesmanPayment/");
      }

      var parameters = "";
      parameters =
        parameters +
        (await dal.SP_Parameters(
          "@chvnPaymentId",
          parseInt(SalesmanPaymentId)
        ));

      parameters =
        parameters + (await dal.SP_Parameters("@chvnOperationType", "ONHOLD"));

      // Execute the stored procedure to update the record
      const result = await dal.SPExecuteDataTable(
        "[WebApplication_SP].[SP_Salesman_Payment_Export]",
        parameters
      );

      if (result != null) {
        req.flash("success", "Payment Status Updated Successfully");
        res.redirect("/salesmanPayment/");
      } else {
        req.flash("error", "Failed to Updated Status");
        res.render("payment/paymentList", {
          ...req.body,
          SalesmanKYCId: req.params.SalesmanPaymentId,
        });
      }
    } catch (error) {
      console.error(error);
      req.flash("error", "Error Status Update ");
      res.render("payment/paymentList", {
        ...req.body,
        SalesmanKYCId: req.params.SalesmanPaymentId,
      });
    }
  }
);

router.post(
  "/InitiateStatus/:SalesmanPaymentId",
  async function (req, res, next) {
    try {
      let SalesmanPaymentId = parseInt(req.params.SalesmanPaymentId);

      if (isNaN(SalesmanPaymentId)) {
        // Handle the case when UserId is not a valid number
        req.flash("error", "Invalid SalesmanPaymentId");
        return res.redirect("/salesmanPayment/");
      }

      var parameters = "";
      parameters =
        parameters +
        (await dal.SP_Parameters(
          "@chvnPaymentId",
          parseInt(SalesmanPaymentId)
        ));

      parameters =
        parameters +
        (await dal.SP_Parameters("@chvnOperationType", "INITIATE"));

      // Execute the stored procedure to update the record
      const result = await dal.SPExecuteDataTable(
        "[WebApplication_SP].[SP_Salesman_Payment_Export]",
        parameters
      );

      if (result != null) {
        req.flash("success", "Payment Status Updated Successfully");
        res.redirect("/salesmanPayment/");
      } else {
        req.flash("error", "Failed to Updated Status");
        res.render("payment/paymentList", {
          ...req.body,
          SalesmanKYCId: req.params.SalesmanPaymentId,
        });
      }
    } catch (error) {
      console.error(error);
      req.flash("error", "Error Status Update ");
      res.render("payment/paymentList", {
        ...req.body,
        SalesmanKYCId: req.params.SalesmanPaymentId,
      });
    }
  }
);
//#endregion

//#region Payment Export

router.get("/exportIncentive", async (req, res) => {
  try {
    // Execute the stored procedure directly to fetch data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_Salesman_Payment_Export]",
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

      const filename = `Export_Salesman_Payment_${dateTimeString}.xlsx`;

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

//#region Incentive Bulk

router.get("/incentiveUpload", function (req, res, next) {
  // render to add.ejs
  res.render("payment/incentiveBulk");
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

router.post(
  "/incentiveUpload",
  upload.single("excelFile"),
  async (req, res) => {
    if (!req.file) {
      const errorMessage = "No file uploaded.";
      return res.render("payment/incentiveBulk", { errorMessage });
    }

    const filePath = req.file.path;
    const workbook = new ExcelJS.Workbook();

    try {
      await workbook.xlsx.readFile(filePath);
      const worksheet = workbook.getWorksheet(1); // Assuming data is on the first sheet

      if (worksheet.rowCount <= 1) {
        const errorMessage =
          "The uploaded Excel file is empty or does not contain data.";
        return res.render("payment/incentiveBulk", { errorMessage });
      }

      const expectedColumnNames = [
        "SalesmanKYCID",
        "Month",
        "Year",
        "Incentive",
      ];

      const headerRow = worksheet.getRow(1);
      const actualColumnNames = headerRow.values.map((value) =>
        value.toString()
      );

      if (
        !expectedColumnNames.every((col) => actualColumnNames.includes(col))
      ) {
        const errorMessage =
          "Invalid column names in the Excel file. Please make sure the column names match: " +
          expectedColumnNames.join(", ");
        return res.render("payment/incentiveBulk", { errorMessage });
      }

      const dataToInsert = [];

      worksheet.eachRow((row, rowNumber) => {
        if (rowNumber > 1) {
          // Skip header row
          const rowData = {
            SalesmanKYCID: row.getCell(1).value,
            Month: row.getCell(2).value,
            Year: row.getCell(3).value,
            Incentive: row.getCell(4).value,
          };
          console.log(rowData);
          dataToInsert.push(rowData);
        }
      });

      const pool = await sql.connect(dbConn); // Use the provided database connection

      const tableName = "SalesmanPaymentTempTable";
      const procedureName =
        "[WebApplication_SP].[usp_Truncate_Temp_Table_Bulk]";

      const request = pool.request();
      request.input("TableName", sql.NVarChar(128), tableName);

      const result = await request.execute(procedureName);

      if (result.returnValue === -1) {
        const errorMessage = "Invalid Destination Table Name!.";
        return res.render("payment/incentiveBulk", { errorMessage });
      } else {
        await Promise.all(
          dataToInsert.map(async (record) => {
            const { SalesmanKYCID, Month, Year, Incentive } = record;

            // Handle blank values by converting them to NULL or empty strings
            const safeSalesmanKYCID = SalesmanKYCID || "";
            const safeMonth = Month || "";
            const safeYear = Year || "";
            const safeIncentive = Incentive || "";

            const sql = `INSERT INTO SalesmanPaymentTempTable (SalesmanKYCID, Month, Year, Incentive) 
                     VALUES ('${safeSalesmanKYCID}', '${safeMonth}', '${safeYear}', '${safeIncentive}')`;

            const result = await pool.request().query(sql);
            console.log(`Record inserted`);
          })
        );
      }

      const errorData = [];
      const procedureName2 =
        "[WebApplication_SP].[usp_Salesman_Payment_BulkUpload]";
      // Execute the stored procedure
      const reslt = await pool.request().query(procedureName2);

      let renderData;

      if (reslt.recordset && reslt.recordset.length > 0) {
        // Errors occurred
        const errorData = reslt.recordset.map((record) => {
          const { SalesmanKYCID, Incentive, Error } = record;
          return { SalesmanKYCID, Incentive, Error };
        });
        renderData = {
          errorMessage:
            "Valid records upload successfully ! Please check the details of invalid records.",
          errorData,
        };
      } else {
        // No errors
        renderData = { successMessage: "Incentive Successfully." };
      }

      await pool.close(); // Close the connection pool

      return res.render("payment/incentiveBulk", renderData);
    } catch (error) {
      console.error("Error processing Excel file or inserting data:", error);
      const errorMessage =
        "Error processing Excel file or inserting data into the database.";
      return res.render("payment/incentiveBulk", { errorMessage });
    }
  }
);

//#endregion

//#region UTR Bulk

router.get("/UTRUpload", function (req, res, next) {
  // render to add.ejs
  res.render("payment/UTRBulk");
});

router.post("/UTRUpload", upload.single("excelFile"), async (req, res) => {
  if (!req.file) {
    const errorMessage = "No file uploaded.";
    return res.render("payment/UTRBulk", { errorMessage });
  }

  const filePath = req.file.path;
  const workbook = new ExcelJS.Workbook();

  try {
    await workbook.xlsx.readFile(filePath);
    const worksheet = workbook.getWorksheet(1); // Assuming data is on the first sheet

    if (worksheet.rowCount <= 1) {
      const errorMessage =
        "The uploaded Excel file is empty or does not contain data.";
      return res.render("payment/UTRBulk", { errorMessage });
    }

    const expectedColumnNames = ["SalesmanPaymentID", "UTRNo", "UTRDate"];

    const headerRow = worksheet.getRow(1);
    const actualColumnNames = headerRow.values.map((value) => value.toString());

    if (!expectedColumnNames.every((col) => actualColumnNames.includes(col))) {
      const errorMessage =
        "Invalid column names in the Excel file. Please make sure the column names match: " +
        expectedColumnNames.join(", ");
      return res.render("payment/UTRBulk", { errorMessage });
    }

    const dataToInsert = [];

    worksheet.eachRow((row, rowNumber) => {
      if (rowNumber > 1) {
        // Skip header row
        const rowData = {
          SalesmanPaymentID: row.getCell(1).value,
          UTRNo: row.getCell(2).value,
          UTRDate: row.getCell(3).value,
        };
        console.log(rowData);
        dataToInsert.push(rowData);
      }
    });

    const pool = await sql.connect(dbConn);

    const tableName = "SalesmanUTRBulkTemp";
    const procedureName = "[WebApplication_SP].[usp_Truncate_Temp_Table_Bulk]";

    const request = pool.request();
    request.input("TableName", sql.NVarChar(128), tableName);

    const result = await request.execute(procedureName);

    if (result.returnValue === -1) {
      const errorMessage = "Invalid Destination Table Name!.";
      return res.render("payment/UTRBulk", { errorMessage });
    } else {
      await Promise.all(
        dataToInsert.map(async (record) => {
          const { SalesmanPaymentID, UTRNo, UTRDate } = record;
          const sql = `INSERT INTO SalesmanUTRBulkTemp (SalesmanPaymentID, UTRNo, UTRDate) VALUES ('${SalesmanPaymentID}', '${UTRNo}', '${UTRDate}')`;

          const result = await pool.request().query(sql);
          console.log(`Record inserted`);
        })
      );
    }

    const errorData = [];
    const procedureName2 = "[WebApplication_SP].[usp_Salesman_UTR_BulkUpload]";
    // Execute the stored procedure
    const reslt = await pool.request().query(procedureName2);

    let renderData;

    if (reslt.recordset && reslt.recordset.length > 0) {
      // Errors occurred
      const errorData = reslt.recordset.map((record) => {
        const { SalesmanPaymentID, UTRNo, Error } = record;
        return { SalesmanPaymentID, UTRNo, Error };
      });
      renderData = {
        errorMessage:
          "Valid records upload successfully ! Please check the details of invalid records.",
        errorData,
      };
    } else {
      // No errors
      renderData = { successMessage: "UTR Uploaded Successfully." };
    }

    await pool.close(); // Close the connection pool

    return res.render("payment/UTRBulk", renderData);
  } catch (error) {
    console.error("Error processing Excel file or inserting data:", error);
    const errorMessage =
      "Error processing Excel file or inserting data into the database.";
    return res.render("payment/UTRBulk", { errorMessage });
  }
});

//#endregion

//#region Invoice

const generateInvoicePDF = (pdfDoc, invoice) => {
  // Add a new page for each invoice
 // pdfDoc.addPage();

  pdfDoc
    .fontSize(16)
    .text("Anmol Industries", { align: "center" })
    .text("Sec - 44", { align: "center" })
    .text("Gurgaon , 122001", { align: "center" })
    .text("Phone: 0124-425-8569", { align: "center" })
    .moveDown();

  pdfDoc.fontSize(14).text("INVOICE", { align: "center" });
  pdfDoc.fontSize(12).text(`INVOICE # ${invoice.InvoiceNumber}`);
  pdfDoc.text(`DATE: ${invoice.InvoiceDate}`);
  pdfDoc.moveDown();

  pdfDoc.fontSize(12).text("TO:");
  pdfDoc.text(`UserName: ${invoice.PANNumber}`);
  pdfDoc.text(`EmailId: ${invoice.EmailId}`);
  pdfDoc.text(`MobileNumber: ${invoice.MobileNumber}`);
  pdfDoc.text(`Address: ${invoice.AddressLine1}, ${invoice.PinCode}`);
//  pdfDoc.text(`Date of Birth: ${invoice.DateofBirth}`);
  pdfDoc.moveDown();

  pdfDoc.fontSize(12).text("COMMENTS OR SPECIAL INSTRUCTIONS:");

  //pdfDoc.moveDown();
  pdfDoc.text(
    "If payment is not transferred to your account or any case of transaction failure or fraud immediately contact to your senior management to claim your payment."
  );
  pdfDoc.text(
    "Reports should be done within the 3 working days of transaction."
  );
  pdfDoc.moveDown();

  pdfDoc
    .fontSize(12)
    .text("Manager                       :                Anmol");
  pdfDoc.fontSize(12).text("PHONE. NUMBER       :                7289879110");
 // pdfDoc.fontSize(12).text("REQUISITIONER         :                ----------");
  pdfDoc.fontSize(12).text("PAYMENT METHOD    :                Bank Transfer");
  pdfDoc.fontSize(12).text("TERMS").bold;
  pdfDoc.moveDown();

  // pdfDoc.fontSize(12).text("Due on receipt");
  // pdfDoc.moveDown();

  pdfDoc.fontSize(12).text("PAYMENT DETAILS : ");
  pdfDoc.moveDown();
  pdfDoc.text(`PaymentAmount: ${invoice.Incentive}`);
  pdfDoc.text(`UTRNumber: ${invoice.UTRNumber}`);
  pdfDoc.text(`PaymentStatus: ${invoice.PaymentStatus}`);
  pdfDoc.text(`PaymentDate: ${invoice.PaymentDate}`);
  pdfDoc.text(`DSRId: ${invoice.DSRId}`);
  pdfDoc.text(`KycStatus: ${invoice.KycStatus}`);
  pdfDoc.text(`KYCDate: ${invoice.KYCDate}`);

  pdfDoc.moveDown();
  pdfDoc.text("Make all checks payable to Anmol Industries");
  pdfDoc.text(
    "If you have any questions concerning this invoice, contact Anmol, 7289879110, itservices.agv@byldgroup.com"
  );
  pdfDoc.moveDown();

  pdfDoc.fontSize(12).text("THANK YOU FOR YOUR BUSINESS!");
};

router.get("/downloadInvoice/:SalesmanPaymentId", async (req, res) => {
  try {
    let SalesmanPaymentId = parseInt(req.params.SalesmanPaymentId); // Convert UserId to integer

    // Execute the stored procedure directly to fetch data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_Salesman_Payment_Export]",
      { chvnOperationType: "PDF", chvnPaymentId: SalesmanPaymentId }
    );

    // Check if result contains data
    if (
      result &&
      result.recordset &&
      Array.isArray(result.recordset) &&
      result.recordset.length > 0
    ) {
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
        .replace(/\/|,| /g, "_");

      const pdfDoc = new PDFDocument();
      const filename = `DSR_Invoice_${dateTimeString}.pdf`;

      res.setHeader("Content-Type", "application/pdf");
      res.setHeader(
        "Content-Disposition",
        `attachment; filename="${filename}"`
      );

      pdfDoc.pipe(res);

      // Add content to the PDF
      // pdfDoc.fontSize(18).text("All Invoices", { align: "center" });
      // pdfDoc.moveDown();

      result.recordset.forEach((invoice) => {
        generateInvoicePDF(pdfDoc, invoice);
      });

      pdfDoc.end();
    } else {
      // No data found or data structure issue, send an error response
      console.error("No data found or invalid data structure");
      res.status(404).send("No data found or invalid data structure");
    }
  } catch (error) {
    // Handle errors
    console.error("Error generating PDF file:", error);
    res.status(500).send("Error generating PDF file");
  }
});

//#endregion

module.exports = router;
