const dal = require("../DAL");
const https = require("https");

const API_KEY = "api-key-here";

function sendMessages_InfiniAcctmnAcctmn(tomessage, body) {
  return new Promise((resolve, reject) => {
    try {
      const URL = `https://alerts.solutionsinfini.com/api/v4/?api_key=${encodeURIComponent(
        API_KEY
      )}&method=sms&message=${encodeURIComponent(body)}&to=${encodeURIComponent(
        tomessage
      )}&sender=ACCTMN&unicode=0`;

      const options = {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
      };

      const req = https.request(URL, options, (res) => {
        let data = "";

        res.on("data", (chunk) => {
          data += chunk;
        });

        res.on("end", () => {
          try {
            const infiniresponse = JSON.parse(data);
            if (infiniresponse.status === "OK") {
              resolve(1);
            } else if (infiniresponse.status === "E612") {
              resolve(0);
            } else {
              resolve(0);
            }
          } catch (error) {
            reject(error);
          }
        });
      });

      req.on("error", (error) => {
        reject(error);
      });

      req.end();
    } catch (error) {
      reject(error);
    }
  });
}

module.exports = {
  sendMessages_InfiniAcctmnAcctmn: sendMessages_InfiniAcctmnAcctmn,
};
