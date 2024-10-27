var express = require("express");
var router = express.Router();
var app = express();
var dbConn = require("../lib/db");
const sql = require("mssql");
const dal = require("../DAL");
const multer = require("multer");
const fs = require("fs");
const ExcelJS = require("exceljs");
const PDFDocument = require("pdfkit");


//#region 

router.get("/login", function (req, res, next) {
  
  res.render("users/login", {
  
    errors: [], 
    username: ""
  });
});

router.post("/login", function(req, res, next) {

  const hardcodedUsername = "anmol";
  const hardcodedPassword = "anm123";

  const { username, password } = req.body;

  const errors = [];

  if (username !== hardcodedUsername || password !== hardcodedPassword) {
    errors.push("Invalid username or password");
  }

  if (errors.length > 0) {
    
    res.render("users/login", { errors, username });
  } else {
    
    res.render("users/"); 
  }
});

//#endregion


router.get("/", function (req, res, next) {
  res.render("users");
});

//#region Country

router.get("/searchCountry", async (req, res, next) => {
  try {
    const CountryName = req.query.CountryName;

    let query =
      "select CountryId,CountryName,IsEnabled from [MasterManagement].[Country] where Isdeleted=0 ";

    const queryParams = [];

    if (CountryName) {
      query += " AND CountryName LIKE @CountryName";
      queryParams.push({
        name: "CountryName",
        type: sql.NVarChar,
        value: `%${CountryName}%`,
      });
    }

    query += " ORDER BY CountryId DESC";

    const pool = await sql.connect(dbConn);

    const result = await pool
      .request()
      .input("CountryName", sql.NVarChar, `%${CountryName}%`)
      .query(query);

    sql.close();

    res.render("users/country", { data: result.recordset });
  } catch (error) {
    console.error("Error executing search query:", error);
    res.render("users/country", {
      data: "",
      error: "Error executing search query",
    });
  }
});

router.get("/country", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "COUNTRY"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("users/country", { data: data });
    } else {
      req.flash("error", err);
      // render to views/users/index.ejs
      res.render("users/country", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("users/country", { data: [] }); // Render with an empty array in case of errors
  }
});

router.get("/addCountry", function (req, res, next) {
  // render to add.ejs
  res.render("users/addCountry", {
    CountryName: "",
    IsEnabled: "",
  });
});

router.post("/addCountry", async function (req, res, next) {
  try {
    const { CountryName, IsEnabled } = req.body;

    // Validation
    if (!CountryName || CountryName.trim().length === 0) {
      req.flash("error", "Please enter a valid Country Name");
      return res.render("users/country", req.body);
    }

    const parameters = {
      chvnCountryName: CountryName,
      chvnIsEnabled: IsEnabled === "true" ? 1 : 0, // Convert string 'true' or 'false' to boolean 1 or 0
      chvnOperationType: "INSERTCOUNTRY",
    };

    // Execute the stored procedure to insert data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    if (result && result.recordset) {
      req.flash("success", "Country successfully added");
      res.redirect("/master/country");
    } else {
      req.flash("error", "Failed to add Country");
      res.render("/users/country", req.body);
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error adding Country record");
    res.render("/users/country", req.body);
  }
});

router.get("/editCountry/:CountryId", async function (req, res, next) {
  try {
    let CountryId = parseInt(req.params.CountryId); // Convert UserId to integer

    if (isNaN(CountryId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid CountryId");
      return res.redirect("/users/country");
    }

    // Call the stored procedure to fetch user details by ID with additional parameters
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnCountryId", CountryId));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "COUNTRYBYID"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    console.log(data);

    if (data != null) {
      // User found, render to edit.ejs with user details
      res.render("users/editCountry", {
        title: "Edit User",
        CountryId: data[0].CountryId,
        CountryName: data[0].CountryName,
        // IsEnabled: data[0].IsEnabled === "true" ? 1 : 0,
        IsEnabled: data[0].IsEnabled,
      });
    } else {
      // User not found
      req.flash("error", "Record not found with CountryId = " + CountryId);
      res.redirect("/users/country");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error fetching Country details");
    res.redirect("/users/country");
  }
});

router.post("/updateCountry/:CountryId", async function (req, res, next) {
  try {
    let CountryId = parseInt(req.params.CountryId); // Convert UserId to integer

    if (isNaN(CountryId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid CountryId");
      return res.redirect("/users/country");
    }

    const { CountryName, IsEnabled } = req.body;

    if (!CountryName || CountryName.trim().length === 0) {
      req.flash("error", "Please enter a valid Country Name");
      return res.render("users/country", req.body);
    }

    // Call the stored procedure to fetch user details by ID with additional parameters
    var parameters = "";
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnCountryId", parseInt(CountryId)));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnCountryName", CountryName));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnIsEnabled", IsEnabled === "on" ? 1 : 0));

    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "UPDATECOUNTRY"));

    // Execute the stored procedure to update the record
    const result = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    if (result != null) {
      req.flash("success", "Country successfully updated");
      res.redirect("/master/country");
    } else {
      req.flash("error", "Failed to update Country ");
      res.render("users/editCountry", {
        ...req.body,
        CountryId: req.params.CountryId,
      });
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error updating Country record");
    res.render("users/editCountry", {
      ...req.body,
      CountryId: req.params.CountryId,
    });
  }
});

router.get("/deleteCountry/:CountryId", async function (req, res, next) {
  try {
    let CountryId = parseInt(req.params.CountryId);

    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnCountryId", CountryId));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "DELETECOUNTRY"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );
    console.log(data);
    if (data != null) {
      req.flash("success", "Country deleted successfully");
      res.redirect("/master/country");
    } else {
      req.flash("No data found!");
      res.redirect("/master/country");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error deleting Country");
    res.redirect("/master/country");
  }
});

//#endregion

//#region State

router.get("/searchState", async (req, res, next) => {
  try {
    const StateName = req.query.StateName;

    let query =
      "select a.StateId,a.StateName,a.IsEnabled,b.CountryId,b.CountryName from [MasterManagement].[State] a inner join [MasterManagement].[Country] b on a.CountryId=b.CountryId where a.Isdeleted=0";

    const queryParams = [];

    if (StateName) {
      query += " AND a.StateName LIKE @StateName";
      queryParams.push({
        name: "StateName",
        type: sql.NVarChar,
        value: `%${StateName}%`,
      });
    }

    query += " ORDER BY a.StateId DESC";

    const pool = await sql.connect(dbConn);

    const result = await pool
      .request()
      .input("StateName", sql.NVarChar, `%${StateName}%`)
      .query(query);

    sql.close();

    res.render("users/state", { data: result.recordset });
  } catch (error) {
    console.error("Error executing search query:", error);
    res.render("users/state", {
      data: "",
      error: "Error executing search query",
    });
  }
});

router.get("/state", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "STATE"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("users/state", { data: data });
    } else {
      req.flash("error", err);
      // render to views/users/index.ejs
      res.render("users/state", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("users/state", { data: [] }); // Render with an empty array in case of errors
  }
});

router.get("/addState", async function (req, res, next) {
  // Render to add.ejs

  var parameters = "";

  parameters =
    parameters + (await dal.SP_Parameters("@chvnOperationType2", "COUNTRY"));
  const countryData = await dal.SPExecuteDataTable(
    "[WebApplication_SP].[SP_Country_State_City_Dropdown]",
    parameters
  );

  if (countryData != null) {
    res.render("users/addState", {
      title: "Add State",
      countryData: countryData,
      CountryName: "",
      StateName: "",
      IsEnabled: "",
    });
  } else {
    alert("No record found!");
    res.redirect("/master/state");
  }
});

router.post("/addState", async function (req, res, next) {
  try {
    const { CountryName, StateName, IsEnabled } = req.body;

    const parameters = {
      chvnCountryName: CountryName,
      chvnStateName: StateName,
      chvnIsEnabled: IsEnabled === "true" ? 1 : 0, // Convert string 'true' or 'false' to boolean 1 or 0
      chvnOperationType: "INSERTSTATE",
    };

    // Execute the stored procedure to insert data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    if (result && result.recordset) {
      req.flash("success", "State successfully added");
      res.redirect("/master/state");
    } else {
      req.flash("error", "Failed to add State");
      res.render("/users/state", req.body);
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error adding State record");
    res.render("/users/state", req.body);
  }
});

router.get("/editState/:StateId", async function (req, res, next) {
  try {
    let StateId = parseInt(req.params.StateId); // Convert UserId to integer

    if (isNaN(StateId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid State ");
      return res.redirect("/users/state");
    }

    // Call the stored procedure to fetch user details by ID with additional parameters
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnStateId", StateId));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "STATEBYID"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    console.log(data);

    if (data != null) {
      // User found, render to edit.ejs with user details
      res.render("users/editState", {
        title: "Edit User",
        StateId: data[0].StateId,
        CountryName: data[0].CountryName,
        StateName: data[0].StateName,
        // IsEnabled: data[0].IsEnabled === "true" ? 1 : 0,
        IsEnabled: data[0].IsEnabled,
      });
    } else {
      // User not found
      req.flash(
        "error",
        "State is not enable with mapped Country having stateName = " +
          stateName
      );
      res.redirect("/users/state");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error fetching State details");
    res.redirect("/users/state");
  }
});

router.post("/updateState/:StateId", async function (req, res, next) {
  try {
    let StateId = parseInt(req.params.StateId);
    // let CountryName = parseInt(req.params.CountryName); // Convert UserId to integer

    if (isNaN(StateId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid StateId");
      return res.redirect("/users/state");
    }

    const { CountryName, StateName, IsEnabled } = req.body;

    if (!StateName || StateName.trim().length === 0) {
      req.flash("error", "Please enter a valid State Name");
      return res.render("users/state", req.body);
    }

    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnStateId", parseInt(StateId)));
    // parameters =
    //   parameters + (await dal.SP_Parameters("@chvnCountryName", CountryName));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnStateName", StateName));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnIsEnabled", IsEnabled === "on" ? 1 : 0));

    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "UPDATESTATE"));

    // Execute the stored procedure to update the record
    const result = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    if (result != null) {
      req.flash("success", "State successfully updated");
      res.redirect("/master/state");
    } else {
      req.flash("error", "Failed to update State ");
      res.render("users/editState", {
        ...req.body,
        StateId: req.params.StateId,
      });
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error updating State record");
    res.render("users/editState", {
      ...req.body,
      StateId: req.params.StateId,
    });
  }
});

router.get("/deleteState/:StateId", async function (req, res, next) {
  try {
    let StateId = parseInt(req.params.StateId);

    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnStateId", StateId));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "DELETESTATE"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );
    console.log(data);
    if (data != null) {
      req.flash("success", "State deleted successfully");
      res.redirect("/master/state");
    } else {
      req.flash("No record found!");
      res.redirect("/master/state");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error deleting State");
    res.redirect("/master/state");
  }
});

//#endregion

//#region City

router.get("/searchCity", async (req, res, next) => {
  try {
    const CityName = req.query.CityName;

    let query =
      "SELECT a.CityName,a.CityId,b.StateId,b.StateName,c.CountryId,c.CountryName,a.IsEnabled from [MasterManagement].[City] a inner join [MasterManagement].[State] b on a.StateId=b.StateId inner join [MasterManagement].[Country] c on b.CountryId=c.CountryId where a.Isdeleted=0";

    const queryParams = [];

    if (CityName) {
      query += " AND a.CityName LIKE @CityName";
      queryParams.push({
        name: "CityName",
        type: sql.NVarChar,
        value: `%${CityName}%`,
      });
    }

    query += " ORDER BY a.CityId DESC";

    const pool = await sql.connect(dbConn);

    const result = await pool
      .request()
      .input("CityName", sql.NVarChar, `%${CityName}%`)
      .query(query);

    sql.close();

    res.render("users/city", { data: result.recordset });
  } catch (error) {
    console.error("Error executing search query:", error);
    res.render("users/city", {
      data: "",
      error: "Error executing search query",
    });
  }
});

router.get("/city", async function (req, res, next) {
  try {
    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "CITY"));
    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );
    console.log(data);
    if (data != null) {
      res.render("users/city", { data: data });
    } else {
      req.flash("error", err);
      // render to views/users/index.ejs
      res.render("users/city", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    req.flash("error", "Error fetching data");
    res.render("users/city", { data: [] }); // Render with an empty array in case of errors
  }
});

router.get("/addCity", async function (req, res, next) {
  // Render to add.ejs

  var parameters = "";

  parameters =
    parameters + (await dal.SP_Parameters("@chvnOperationType2", "COUNTRY"));
  const countryData = await dal.SPExecuteDataTable(
    "[WebApplication_SP].[SP_Country_State_City_Dropdown]",
    parameters
  );

  if (countryData != null) {
    res.render("users/addCity", {
      title: "Add City",
      countryData: countryData,
      CountryName: "",
      StateName: "",
      CityName: "",
      IsEnabled: "",
    });
  } else {
    //alert("No user found!");
    res.redirect("/master/city");
  }
});

router.post("/addCity", async function (req, res, next) {
  try {
    const { CountryName, StateName, CityName, IsEnabled } = req.body;

    const parameters = {
      chvnCountryName: CountryName,
      chvnStateName: StateName,
      chvnCityName: CityName,
      chvnIsEnabled: IsEnabled === "true" ? 1 : 0, // Convert string 'true' or 'false' to boolean 1 or 0
      chvnOperationType: "INSERTCITY",
    };

    // Execute the stored procedure to insert data
    const result = await dal.executeStoredProcedure(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    if (result && result.recordset) {
      req.flash("success", "City successfully added");
      res.redirect("/master/city");
    } else {
      req.flash("error", "Failed to add City");
      res.render("/users/city", req.body);
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error adding City record");
    res.render("/users/city", req.body);
  }
});

router.get("/editCity/:CityId", async function (req, res, next) {
  try {
    let CityId = parseInt(req.params.CityId); // Convert UserId to integer

    if (isNaN(CityId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid City ");
      return res.redirect("/users/city");
    }

    // Call the stored procedure to fetch user details by ID with additional parameters
    var parameters = "";
    parameters = parameters + (await dal.SP_Parameters("@chvnCityId", CityId));
    parameters =
      parameters + (await dal.SP_Parameters("@chvnOperationType", "CITYBYID"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    console.log(data);

    if (data != null) {
      // User found, render to edit.ejs with user details
      res.render("users/editCity", {
        title: "Edit User",
        CountryId: data[0].CountryId,
        StateId: data[0].StateId,
        CityId: data[0].CityId,
        CountryName: data[0].CountryName,
        StateName: data[0].StateName,
        CityName: data[0].CityName,
        // IsEnabled: data[0].IsEnabled === "true" ? 1 : 0,
        IsEnabled: data[0].IsEnabled,
      });
    } else {
      // User not found
      req.flash(
        "error",
        "State is not enable with mapped Country with stateName = " + stateName
      );
      res.redirect("/users/city");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error fetching State details");
    res.redirect("/users/city");
  }
});

router.post("/updateCity/:CityId", async function (req, res, next) {
  try {
    let CityId = parseInt(req.params.CityId);
    // let CountryName = parseInt(req.params.CountryName); // Convert UserId to integer

    if (isNaN(CityId)) {
      // Handle the case when UserId is not a valid number
      req.flash("error", "Invalid CityId");
      return res.redirect("/users/city");
    }

    const { CountryName, CountryId, StateId, StateName, CityName, IsEnabled } =
      req.body;

    if (!CityName || CityName.trim().length === 0) {
      req.flash("error", "Please enter a valid State Name");
      return res.render("users/state", req.body);
    }

    var parameters = "";
    parameters =
      parameters + (await dal.SP_Parameters("@chvnCityId", parseInt(CityId)));
    parameters =
      //   parameters + (await dal.SP_Parameters("@chvnStateId", StateName));
      // parameters =
      parameters + (await dal.SP_Parameters("@chvnCityName", CityName));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnIsEnabled", IsEnabled === "on" ? 1 : 0));

    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "UPDATECITY"));

    // Execute the stored procedure to update the record
    const result = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );

    if (result != null) {
      req.flash("success", "City successfully updated");
      res.redirect("/master/city");
    } else {
      req.flash("error", "Failed to update City ");
      res.render("users/editCity", {
        ...req.body,
        CityId: req.params.CityId,
      });
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error updating City record");
    res.render("users/editCity", {
      ...req.body,
      CityId: req.params.CityId,
    });
  }
});

router.get("/deleteCity/:CityId", async function (req, res, next) {
  try {
    let CityId = parseInt(req.params.CityId);

    var parameters = "";
    parameters = parameters + (await dal.SP_Parameters("@chvnCityId", CityId));
    parameters =
      parameters +
      (await dal.SP_Parameters("@chvnOperationType", "DELETECITY"));
    const data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Master_State_City_Currency]",
      parameters
    );
    console.log(data);
    if (data != null) {
      req.flash("success", "City deleted successfully");
      res.redirect("/master/city");
    } else {
      req.flash("No record found!");
      res.redirect("/master/city");
    }
  } catch (error) {
    console.error(error);
    req.flash("error", "Error deleting city");
    res.redirect("/master/city");
  }
});

//#endregion

router.get("/get_data", async function (req, res, next) {
  try {
    var type = req.query.type;

    var search_query = req.query.parent_value;

    var parameters = "";
    if (type == "load_state") {
      parameters =
        parameters + (await dal.SP_Parameters("@chvnOperationType2", "STATE"));
      parameters =
        parameters + (await dal.SP_Parameters("@chvnSearch", search_query));
    }
    if (type == "load_city") {
      parameters =
        parameters + (await dal.SP_Parameters("@chvnOperationType2", "CITY"));
      parameters =
        parameters + (await dal.SP_Parameters("@chvnSearch", search_query));
    }

    var data = await dal.SPExecuteDataTable(
      "[WebApplication_SP].[SP_Country_State_City_Dropdown]",
      parameters
    );

    console.log(data);
    if (data != null) {
      const data_arr = JSON.stringify(data);

      res.json(data_arr);
    } else {
      req.flash("error", err);
      // render to views/users/index.ejs

      //  res.render("users/city", {});
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    // req.flash("error", "Error fetching data");

    //   res.render("users/city", { data: [] }); // Render with an empty array in case of errors
  }
});

module.exports = router;
