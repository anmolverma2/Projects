var sql = require("mssql");
const config = {
  user: "user_name",
  password: "your_password",
  server: "server_name",
  database: "database_name",
  options: {
    encrypt: true,
    trustServerCertificate: true,
  },
  port: 1433,
};
const pool = new sql.ConnectionPool(config);
pool
  .connect()
  .then(() => {
    console.log("Connected to SQL Server...");
  })
  .catch((err) => {
    console.log("Database Connection Failed! Bad Config: ", err);
  });

module.exports = config;
