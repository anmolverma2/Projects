var express = require("express");
var router = express.Router();
var dbConn = require("../lib/db");
const multer = require("multer");
const path = require("path");
const sql = require("mssql");

// Add a route for searching users
router.get("/search", function (req, res, next) {
  const username = req.query.username;
  const fname = req.query.fname;
  const mobile = req.query.mobile;
  const country = req.query.country;
  const state = req.query.state;
  const city = req.query.city;

  // Construct the base query
  let query = "SELECT * FROM Employee WHERE 1=1";

  // Create an array to store the query parameters
  const queryParams = [];

  // Check and append search conditions for each field
  if (username) {
    query += " AND Name LIKE @username";
    queryParams.push({
      name: "username",
      type: "VarChar",
      value: `%${username}%`,
    });
  }

  if (fname) {
    query += " AND FathersName LIKE @fname";
    queryParams.push({ name: "fname", type: "VarChar", value: `%${fname}%` });
  }

  if (mobile) {
    query += " AND Mobile LIKE @mobile";
    queryParams.push({ name: "mobile", type: "VarChar", value: `%${mobile}%` });
  }

  // if (country) {
  //   query += " AND Country LIKE @country";
  //   queryParams.push({ name: 'country', type: 'VarChar', value: `%${country}%` });
  // }

  // if (state) {
  //   query += " AND State LIKE @state";
  //   queryParams.push({ name: 'state', type: 'VarChar', value: `%${state}%` });
  // }

  // if (city) {
  //   query += " AND City LIKE @city";
  //   queryParams.push({ name: 'city', type: 'VarChar', value: `%${city}%` });
  // }

  // Add the final ORDER BY clause
  query += " ORDER BY Id DESC";

  const request = new Request(query, function (err, rowCount, rows) {
    if (err) {
      req.flash("error", err.message);
      res.render("users", { data: "" });
    } else {
      res.render("users", { data: rows });
    }
  });

  // Adding parameters to the request
  queryParams.forEach((param) => {
    request.addParameter(param.name, param.type, param.value);
  });

  // Execute the request using the SQL Server connection
  req.sqlConnection.execSql(request);
});

// display user page
router.get("/", async function (req, res, next) {
  try {
    const pool = await sql.connect(dbConn.config); // Establish connection using the configuration from dbConn

    const result = await pool
      .request()
      .query("SELECT * FROM Employee ORDER BY Id DESC");

    res.render("users", { data: result.recordset }); // Pass the result recordset to the view
  } catch (err) {
    console.error("SQL error:", err.message);
    req.flash("error", err.message);
    res.render("users", { data: "" });
  }
});

// Check if the username already exists in the database
router.get("/check-username", function (req, res, next) {
  const username = req.query.username;

  const request = new Request(
    "SELECT COUNT(*) AS count FROM Employee WHERE Name = @username",
    function (err, rowCount, rows) {
      if (err) {
        res.json({
          success: false,
          message: "Error checking username availability",
        });
      } else {
        // If the count is greater than 0, the username is already taken
        if (rowCount > 0 && rows[0][0].value > 0) {
          res.json({ success: false, message: "Username is already taken" });
        } else {
          res.json({ success: true, message: "Username is available" });
        }
      }
    }
  );

  request.addParameter("username", TYPES.NVarChar, username);

  // Execute the request using the SQL Server connection
  req.sqlConnection.execSql(request);
});

// Define the data for countries, states, and cities
// Assuming you have established the SQL Server connection as req.sqlConnection

router.get("/states/:countryName", function (req, res, next) {
  const countryName = req.params.countryName;

  const query = `
    SELECT s.state_name
    FROM States s
    INNER JOIN Countries c ON s.country_id = c.country_id
    WHERE c.country_name = @countryName
  `;

  const request = new Request(query, function (err, rowCount, rows) {
    if (err) {
      res
        .status(500)
        .json({ success: false, message: "Error fetching states" });
    } else {
      const states = rows.map((row) => row[0].value);
      res.json({ success: true, states });
    }
  });

  request.addParameter("countryName", TYPES.NVarChar, countryName);

  // Execute the request using the SQL Server connection
  req.sqlConnection.execSql(request);
});

router.get("/add", function (req, res, next) {
  // Fetch country data from the database
  const query = "SELECT country_name FROM Countries";

  const request = new Request(query, function (err, rowCount, rows) {
    if (err) {
      res
        .status(500)
        .json({ success: false, message: "Error fetching country data" });
    } else {
      const countries = rows.map((row) => row[0].value);

      // Render the add.ejs template and pass country data
      res.render("users/add", {
        Name: "",
        FathersName: "",
        Email: "",
        Mobile: "",
        Country: "",
        State: "",
        City: "",
        countries: countries,
        states: [], // Initially empty, will be populated based on the selected country
        cities: [], // Initially empty, will be populated based on the selected state
      });
    }
  });

  // Execute the request using the SQL Server connection
  req.sqlConnection.execSql(request);
});

// add a new user
router.post("/add", function (req, res, next) {
  let Name = req.body.Name;
  let FathersName = req.body.FathersName;
  let Email = req.body.Email;
  let Mobile = req.body.Mobile;
  let Country = req.body.Country;
  let State = req.body.State;
  let City = req.body.City;
  let errors = {};

  if (Name.length === 0) {
    errors.Name = "Name is required";
  }

  if (FathersName.length === 0) {
    errors.FathersName = "Father's Name is required";
  }

  // Email Validation
  if (!/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,})+$/.test(Email)) {
    errors.Email = "Invalid email format";
  }

  // Mobile Number Validation (Assuming a 10-digit format for a mobile number)
  if (!/^[0-9]{10}$/.test(Mobile)) {
    errors.Mobile =
      "Mobile number must be 10 digits long and contain only numbers";
  }

  if (Country.length === 0) {
    errors.Country = "Country is required";
  }

  if (State.length === 0) {
    errors.State = "State is required";
  }

  if (City.length === 0) {
    errors.City = "City is required";
  }

  // Check if there are any errors
  if (Object.keys(errors).length > 0) {
    // Set flash message
    req.flash("error", "Please enter valid records");

    // Render to add.ejs with error messages
    res.render("users/add", {
      errors: errors, // Pass the error messages to the template
      Name: Name,
      FathersName: FathersName,
      Email: Email,
      Mobile: Mobile,
      Country: Country,
      State: State,
      City: City,
    });
  } else {
    const form_data = {
      Name: Name,
      FathersName: FathersName,
      Email: Email,
      Mobile: Mobile,
      Country: Country,
      State: State,
      City: City,
    };

    const query = `
      INSERT INTO Employee (Name, FathersName, Email, Mobile, Country, State, City)
      VALUES (@Name, @FathersName, @Email, @Mobile, @Country, @State, @City)
    `;

    const request = new Request(query, function (err, rowCount) {
      if (err) {
        req.flash("error", err.message);

        // Render to add.ejs
        res.render("users/add", {
          Name: form_data.Name,
          FathersName: form_data.FathersName,
          Email: form_data.Email,
          Mobile: form_data.Mobile,
          Country: form_data.Country,
          State: form_data.State,
          City: form_data.City,
        });
      } else {
        req.flash("success", "User successfully added");
        res.redirect("/users");
      }
    });

    // Adding parameters to the request
    request.addParameter("Name", TYPES.NVarChar, form_data.Name);
    request.addParameter("FathersName", TYPES.NVarChar, form_data.FathersName);
    request.addParameter("Email", TYPES.NVarChar, form_data.Email);
    request.addParameter("Mobile", TYPES.NVarChar, form_data.Mobile);
    request.addParameter("Country", TYPES.NVarChar, form_data.Country);
    request.addParameter("State", TYPES.NVarChar, form_data.State);
    request.addParameter("City", TYPES.NVarChar, form_data.City);

    // Execute the request using the SQL Server connection
    req.sqlConnection.execSql(request);
  }
});

// display edit user page
router.get("/edit/:id", function (req, res, next) {
  const Id = req.params.id;

  const query = "SELECT * FROM Employee WHERE Id = @Id";

  const request = new Request(query, function (err, rowCount, rows) {
    if (err) {
      req.flash("error", "Error fetching user with Id = " + Id);
      res.redirect("/users");
    } else {
      if (rowCount <= 0) {
        req.flash("error", "User not found with Id = " + Id);
        res.redirect("/users");
      } else {
        const user = {
          title: "Edit User",
          Id: rows[0].Id.value,
          Name: rows[0].Name.value,
          FathersName: rows[0].FathersName.value,
          Email: rows[0].Email.value,
          Mobile: rows[0].Mobile.value,
          Country: rows[0].Country.value,
          State: rows[0].State.value,
          City: rows[0].City.value,
        };

        // render to edit.ejs
        res.render("users/edit", user);
      }
    }
  });

  // Adding parameters to the request
  request.addParameter("Id", TYPES.Int, Id);

  // Execute the request using the SQL Server connection
  req.sqlConnection.execSql(request);
});

// update user data
router.post("/update/:id", function (req, res, next) {
  let Id = req.params.id;
  let Name = req.body.Name;
  let FathersName = req.body.FathersName;
  let Email = req.body.Email;
  let Mobile = req.body.Mobile;
  let Country = req.body.Country;
  let State = req.body.State;
  let City = req.body.City;
  let errors = false;

  if (
    Name.length === 0 ||
    FathersName.length === 0 ||
    Email.length === 0 ||
    Mobile.length === 0 ||
    Country.length === 0 ||
    State.length === 0 ||
    City.length === 0
  ) {
    errors = true;

    // Set flash message
    req.flash("error", "Please enter all required fields");

    // Render to edit.ejs with flash message
    res.render("users/edit", {
      Id: req.params.id,
      Name: Name,
      FathersName: FathersName,
      Email: Email,
      Mobile: Mobile,
      Country: Country,
      State: State,
      City: City,
    });
  }

  // If no error
  if (!errors) {
    const form_data = {
      Name: Name,
      FathersName: FathersName,
      Email: Email,
      Mobile: Mobile,
      Country: Country,
      State: State,
      City: City,
    };

    const query = `
      UPDATE Employee
      SET Name = @Name, FathersName = @FathersName, Email = @Email, Mobile = @Mobile, Country = @Country, State = @State, City = @City
      WHERE Id = @Id
    `;

    const request = new Request(query, function (err, rowCount) {
      if (err) {
        req.flash("error", err.message);

        // Render to edit.ejs
        res.render("users/edit", {
          Id: req.params.id,
          Name: form_data.Name,
          FathersName: form_data.FathersName,
          Email: form_data.Email,
          Mobile: form_data.Mobile,
          Country: form_data.Country,
          State: form_data.State,
          City: form_data.City,
        });
      } else {
        req.flash("success", "User successfully updated");
        res.redirect("/users");
      }
    });

    // Adding parameters to the request
    request.addParameter("Name", TYPES.NVarChar, form_data.Name);
    request.addParameter("FathersName", TYPES.NVarChar, form_data.FathersName);
    request.addParameter("Email", TYPES.NVarChar, form_data.Email);
    request.addParameter("Mobile", TYPES.NVarChar, form_data.Mobile);
    request.addParameter("Country", TYPES.NVarChar, form_data.Country);
    request.addParameter("State", TYPES.NVarChar, form_data.State);
    request.addParameter("City", TYPES.NVarChar, form_data.City);
    request.addParameter("Id", TYPES.Int, Id);

    // Execute the request using the SQL Server connection
    req.sqlConnection.execSql(request);
  }
});

// delete user
router.get("/delete/:id", function (req, res, next) {
  const Id = req.params.id;

  const query = "DELETE FROM Employee WHERE Id = @Id";

  const request = new Request(query, function (err, rowCount) {
    if (err) {
      req.flash("error", "Error deleting user with ID = " + Id);
    } else {
      req.flash("success", "User successfully deleted! ID = " + Id);
    }
    res.redirect("/users");
  });

  // Adding parameters to the request
  request.addParameter("Id", TYPES.Int, Id);

  // Execute the request using the SQL Server connection
  req.sqlConnection.execSql(request);
});

// //export

const ExcelJS = require("exceljs");

router.get("/export", (req, res) => {
  const query =
    "SELECT Id, Name, FathersName, Email, Mobile, Country, State, City FROM Employee ORDER BY Id DESC";

  const request = new Request(query, (err, rowCount, rows) => {
    if (err) {
      console.error("Error fetching data from SQL Server:", err);
      return res.status(500).send("Error fetching data from SQL Server");
    }

    console.log("Fetched data from SQL Server:", rows);

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet("Sheet1");

    // Styling options
    const headerFont = { bold: true, color: { argb: "FFFFFFFF" } };
    const headerFill = {
      type: "pattern",
      pattern: "solid",
      fgColor: { argb: "FF2F75B5" },
    };
    const centeredAlignment = { vertical: "middle", horizontal: "center" };

    const columnHeaders = [
      "ID",
      "Name",
      "Father's Name",
      "Email",
      "Mobile",
      "Country",
      "State",
      "City",
    ];
    const headerRow = worksheet.addRow(columnHeaders);
    headerRow.eachCell((cell) => {
      cell.font = headerFont;
      cell.fill = headerFill;
      cell.alignment = centeredAlignment;
    });

    rows.forEach((row) => {
      const values = Object.values(row);
      worksheet.addRow(values);
    });

    // Adjust column widths based on content
    worksheet.columns.forEach((column) => {
      const maxLength = column.values.reduce((acc, value) => {
        const cellLength = value ? value.toString().length : 0;
        return Math.max(acc, cellLength);
      }, 0);
      column.width = maxLength < 15 ? 15 : maxLength; // Set a minimum width
    });

    const currentDate = new Date();
    const monthNames = [
      "January",
      "February",
      "March",
      "April",
      "May",
      "June",
      "July",
      "August",
      "September",
      "October",
      "November",
      "December",
    ];

    const formattedDate =
      currentDate.getDate() +
      "-" +
      monthNames[currentDate.getMonth()] +
      "-" +
      currentDate.getFullYear();

    const filename = `Export_${formattedDate}.xlsx`;

    res.setHeader(
      "Content-Type",
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    );
    res.setHeader("Content-Disposition", `attachment; filename="${filename}"`);

    workbook.xlsx
      .write(res)
      .then(() => {
        res.end();
      })
      .catch((error) => {
        console.error("Error generating Excel file:", error);
        res.status(500).send("Error generating Excel file");
      });
  });

  // Execute the request using the SQL Server connection
  req.sqlConnection.execSql(request);
});

// Handle bulk upload
const fs = require("fs");

router.get("/bulkupload", function (req, res, next) {
  // render to add.ejs
  res.render("users/bulk-upload");
});

const storage = multer.diskStorage({
  destination: function (req, file, cb) {
    const uploadDir = "uploads/";

    // Check if the 'uploads' directory exists, create it if not
    if (!fs.existsSync(uploadDir)) {
      fs.mkdirSync(uploadDir);
    }

    cb(null, uploadDir);
  },
  filename: function (req, file, cb) {
    cb(
      null,
      file.fieldname + "-" + Date.now() + path.extname(file.originalname)
    );
  },
});

const upload = multer({ storage: storage });

router.post("/bulk-upload", upload.single("excelFile"), (req, res) => {
  if (!req.file) {
    // No file was uploaded, show an error message
    const errorMessage = "No file uploaded.";
    return res.render("users/bulk-upload", { errorMessage });
  }

  const filePath = req.file.path;

  // Read the uploaded Excel file
  const workbook = new ExcelJS.Workbook();
  workbook.xlsx
    .readFile(filePath)
    .then(() => {
      const worksheet = workbook.getWorksheet(1); // Assuming data is on the first sheet

      if (worksheet.rowCount <= 1) {
        const errorMessage =
          "The uploaded Excel file is empty or does not contain data.";
        return res.render("users/bulk-upload", { errorMessage });
      }

      const expectedColumnNames = [
        "Name",
        "FathersName",
        "Email",
        "Mobile",
        "Country",
        "State",
        "City",
      ];

      // Validate column names
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
        return res.render("users/bulk-upload", { errorMessage });
      }

      const dataToInsert = [];

      worksheet.eachRow((row, rowNumber) => {
        if (rowNumber > 1) {
          // Skip header row
          const rowData = {
            Name: row.getCell(1).value,
            FathersName: row.getCell(2).value,
            Email: row.getCell(3).value,
            Mobile: row.getCell(4).value,
            Country: row.getCell(5).value,
            State: row.getCell(6).value,
            City: row.getCell(7).value,
          };
          dataToInsert.push(rowData);
        }
      });

      // Insert data into the database
      const insertQuery =
        "INSERT INTO Employee (Name, FathersName, Email, Mobile, Country, State, City) VALUES @dataToInsert";
      const request = new dbConn.Request(insertQuery);

      request.addParameter(
        "dataToInsert",
        dbConn.TYPES.NVarChar,
        JSON.stringify(dataToInsert)
      );
      request.on("requestCompleted", (rowCount) => {
        console.log(`${rowCount} row(s) inserted.`);
        const successMessage = `${rowCount} row(s) inserted successfully.`;
        return res.render("users/bulk-upload", { successMessage });
      });
      request.on("error", (error) => {
        console.error("Error inserting data:", error);
        const errorMessage = "Error inserting data into the database.";
        return res.render("users/bulk-upload", { errorMessage });
      });

      dbConn.execSql(request);
    })
    .catch((error) => {
      console.error("Error reading Excel file:", error);
      const errorMessage = "Error reading Excel file.";
      return res.render("users/bulk-upload", { errorMessage });
    });
});

const PDFDocument = require("pdfkit");

const app = express();

// // Define a route for the EJS view
app.get("/invoice", (req, res) => {
  const invoiceNumber = req.query.invoiceNumber || "INV-123";
  const customerName = req.query.customerName || "John Doe";

  res.render("invoice", { invoiceNumber, customerName });
});

app.get("/pdf/:id", function (req, res) {
  var id = req.params.id;
  var iname;
  var sql = `SELECT * FROM users WHERE id ='${id}'`;
  conn.query(sql, function (error, result) {
    if (error) throw error;

    result.forEach((row) => {
      iname = row.name;
    });
    var fonts = {
      Roboto: {
        normal: "fonts/roboto.regular.ttf",
        bold: "fonts/roboto.medium.ttf",
        italics: "fonts/roboto.italic.ttf",
        bolditalics: "fonts/roboto.mediumItalic.ttf",
      },
    };

    var PdfPrinter = require("pdfmake");
    var printer = new PdfPrinter(fonts);

    var date_time = new Date();
    var data = [];
    data["invoicenumber"] = 1121;
    data["buyeraddress"] = "House Number 123, Street - 2 , Gurgaon";
    data["item"] = "Laptop";
    data["price"] = "5500";
    data["name"] = iname;
    data["date"] = date_time;

    var docDefinition = {
      content: [
        {
          fontSize: 11,
          table: {
            widths: ["50%", "50%"],
            body: [
              [
                {
                  text: "Status: unpaid",
                  border: [false, false, false, true],
                  margin: [-5, 0, 0, 10],
                },
                {
                  text: "Invoice# " + data.invoicenumber,
                  alignment: "right",
                  border: [false, false, false, true],
                  margin: [0, 0, 0, 10],
                },
              ],
            ],
          },
        },
        {
          layout: "noBorders",
          fontSize: 11,
          table: {
            widths: ["50%", "50%"],
            body: [
              [
                { text: data.name, margin: [0, 10, 0, 0] },
                {
                  text: "Invoice date: " + data.date,
                  alignment: "right",
                  margin: [0, 10, 0, 0],
                },
              ],
              ["Yoma Technologies Pvt. Ltd.", ""],
            ],
          },
        },
        {
          fontSize: 11,
          table: {
            widths: ["50%", "50%"],
            body: [
              [
                {
                  text: " ",
                  border: [false, false, false, true],
                  margin: [0, 0, 0, 10],
                },
                {
                  text: "Payment amount: Rs. " + data.price,
                  alignment: "right",
                  border: [false, false, false, true],
                  margin: [0, 0, 0, 10],
                },
              ],
            ],
          },
        },

        {
          fontSize: 11,
          table: {
            widths: ["5%", "56%", "13%", "13%", "13%"],
            body: [
              [
                { text: "Pos", border: [false, true, false, true] },
                { text: "Item", border: [false, true, false, true] },
                { text: "Price", border: [false, true, false, true] },
                {
                  text: "Quantity",
                  alignment: "center",
                  border: [false, true, false, true],
                },
                { text: "Total", border: [false, true, false, true] },
              ],
              [
                { text: "1", border: [false, true, false, true] },
                { text: data.item, border: [false, true, false, true] },
                {
                  text: "Rs. " + data.price,
                  border: [false, true, false, true],
                },
                {
                  text: "1",
                  alignment: "center",
                  border: [false, true, false, true],
                },
                { text: "150", border: [false, true, false, true] },
              ],
            ],
          },
        },
        {
          layout: "noBorders",
          fontSize: 11,
          margin: [0, 0, 5, 0],
          table: {
            widths: ["88%", "12%"],
            body: [
              [
                { text: "Subtotal:", alignment: "right", margin: [0, 5, 0, 0] },
                { text: "Rs. " + data.price, margin: [0, 5, 0, 0] },
              ],
              [{ text: "Tax %:", alignment: "right" }, "0.00"],
            ],
          },
        },
        {
          fontSize: 11,
          table: {
            widths: ["88%", "12%"],
            body: [
              [
                {
                  text: "Total:",
                  alignment: "right",
                  border: [false, false, false, true],
                  margin: [0, 0, 0, 10],
                },
                {
                  text: "Rs. " + data.price,
                  border: [false, false, false, true],
                  margin: [0, 0, 0, 10],
                },
              ],
            ],
          },
        },
      ],
    };
    var options = {};

    var pdfDoc = printer.createPdfKitDocument(docDefinition, options);
    res.setHeader("Content-Disposition", "attachment; filename=Invoice.pdf");
    res.setHeader("Content-Type", "application/pdf");
    pdfDoc.pipe(res);
    pdfDoc.end();
  });
});

module.exports = router;
