const sql = require("mssql");
const conn = require("../DB/conn");
const dal = require("../DAL");
const { ProfileModel } = require("../models/ProfileModel");
const webApi = require("../WebAPI/AccountMein");

function GenerateOTP() {
  const otp = Math.floor(100000 + Math.random() * 900000);
  return otp.toString();
}

var Profile = async (req, res) => {
  try {
    const UUID = req.query.UUID;
    const profile = new ProfileModel(UUID);

    if (!UUID) {
      return res
        .status(400)
        .render("../views/shared/_LayoutResponsive", {
          errorMessage: "UUID parameter is required",
          pageContent: "error404",
        });
    }

    var user = await profile.ProfileDetails(UUID);

    var mobileNumber = user[0].MobileNumber;

    var formattedMobileNumber = mobileNumber.slice(0, 5) + "XXXXX";

    if (!user) {
      return res
        .status(404)
        .render("../views/shared/_LayoutResponsive", {
          errorMessage: "Dashboard details not found",
          pageContent: "error404",
        });
    }

    res.render("../views/shared/_LayoutResponsive", {
      pageContent: "../Parle/DSR/profile",
      UUID: UUID,
      user: user,
      formattedMobileNumber: formattedMobileNumber,
    });
  } catch (err) {
    console.error("Error:", err);
    res
      .status(500)
      .render("../views/shared/_LayoutResponsive", {
        errorMessage: "Internal Server Error",
        pageContent: "error404",
      });
  }
};

var SendOTP = async (req, res) => {
  try {
    const UUID = req.query.UUID;
    const UserName = req.body.UserName;
    const UserCode = req.body.UserCode;
    const Email = req.body.Email;
    const Address = req.body.Address;
    const Mobile = req.body.Mobile;
    const profile = new ProfileModel(
      UUID,
      UserName,
      UserCode,
      Email,
      Address,
      Mobile
    );

    if (!UUID) {
      return res
        .status(400)
        .render("../views/shared/_LayoutResponsive", {
          errorMessage: "UUID parameter is required",
          pageContent: "error404",
        });
    }

    var user = await profile.ProfileDetails(UUID);

    if (!user) {
      return res
        .status(404)
        .render("../views/shared/_LayoutResponsive", {
          errorMessage: "Dashboard details not found",
          pageContent: "error404",
        });
    }

    var mobileNumber = user[0].MobileNumber;

    var formattedMobileNumber = mobileNumber.slice(0, 5) + "XXXXX";

    var otpCode = +GenerateOTP();

    const messageBody = `Dear User, OTP to update your profile details is: ${otpCode}. Team Accountmein.`;

    var sendSMS = webApi
      .sendMessages_InfiniAcctmnAcctmn(mobileNumber, messageBody)
      .then((result) => {
       if(result == 1){
        return;
       }
       else{
        return res
        .status(404)
        .render("../views/shared/_LayoutResponsive", {
          errorMessage: "Error in sending SMS",
          pageContent: "error404",
        });
       }
      })
      .catch((error) => {
        console.error("Error sending SMS:", error);
      });

    var APIData = await profile.InsertOTP(UUID, otpCode);
    
    var result = APIData[0];
    
    if(result == 0){
      return res
      .status(404)
      .render("../views/shared/_LayoutResponsive", {
        errorMessage: "OTP cannot insert",
        pageContent: "error404",
      });   
    }
    
    res.status(200).json({ result: result });

  } catch (err) {
    console.error("Error:", err);
    res
      .status(500)
      .render("../views/shared/_LayoutResponsive", {
        errorMessage: "Internal Server Error",
        pageContent: "error404",
      });
  }
};

var ValidateOTP = async (req, res) => {
  try {
      const UUID = req.query.UUID;
      const OTPNumber = req.query.OTPNumber;

      const profile = new ProfileModel(UUID);

      if (!UUID) {
          return res
              .status(400)
              .json({ error: "UUID parameter is required" }); // Send JSON response for errors
      }

      var data = await profile.ValidateOTP(UUID, OTPNumber);

      if (!data) {
          return res
              .status(404)
              .json({ error: "Error in OTP Verification" }); // Send JSON response for errors
      }

      var result = data[0];

      // Send the result as JSON response
      res.status(200).json({ result: result });

  } catch (err) {
      console.error("Error:", err);
      res
          .status(500)
          .json({ error: "Internal Server Error" }); // Send JSON response for errors
  }
}


var UpdateProfile = async (req, res) => {
  try {
    const UUID = req.query.UUID;
    
    const UserName = '';
   
    const UserCode = '';
    const Email = req.query.Email;
    const Address = req.query.Address;
    const Mobile = req.query.Mobile;
    
   // const ProfilePhoto = req.query.ProfilePhoto;
    const profile = new ProfileModel(
      UUID,
      UserName,
      UserCode,
      Email,
      Address,
      Mobile
     // ProfilePhoto
    );

    if (!UUID) {
      return res
        .status(400)
        .render("../views/shared/_LayoutResponsive", {
          errorMessage: "UUID parameter is required",
          pageContent: "error404",
        });
    }
  
    var data = await  profile.UpdateProfileDetails(UUID,Email,Address);

    if(data != null){
      res.redirect('/SOProfile?UUID='+UUID);
    }
    else
    {
      return res
      .status(400)
      .render("../views/shared/_LayoutResponsive", {
        errorMessage: "Error occur while updating details",
        pageContent: "error404",
      });
    }

  } catch (err) {
    console.error("Error:", err);
    res
      .status(500)
      .render("../views/shared/_LayoutResponsive", {
        errorMessage: "Internal Server Error",
        pageContent: "error404",
      });
  }
}

module.exports = {
  Profile: Profile,
  SendOTP: SendOTP,
  ValidateOTP: ValidateOTP,
  UpdateProfile: UpdateProfile
};
