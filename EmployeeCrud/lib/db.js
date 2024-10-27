const sql = require("mssql");

// Configuration for SQL Server connection
const config = {
  user: "your_username",
  password: "your_password",
  server: "server_name",
  database: "database_name",
  options: {
    encrypt: true,
    trustServerCertificate: true,
  },
  port: 1433,
};

// Create a connection pool
const pool = new sql.ConnectionPool(config);

// Connect to the SQL Server using the pool
pool
  .connect()
  .then(() => {
    console.log("Connected to SQL Server...");
  })
  .catch((err) => {
    console.log("Database Connection Failed! Bad Config: ", err);
  });

module.exports = {
  config: config,
  pool: pool,
};
