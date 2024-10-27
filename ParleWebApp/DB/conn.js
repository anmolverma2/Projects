const sql = require("mssql");

const config = {
  user: "username",
  password: "password",
  server: "server-name",
  database: "database-name",
  port: undefined,
  options: {
    encrypt: true,
    trustServerCertificate: true,
  },
};

sql
  .connect(config)
  .then(() => {
    console.log("Connected successfully");
  })
  .catch((err) => {
    console.log("Bad Config: ", err);
  });

module.exports = config;
