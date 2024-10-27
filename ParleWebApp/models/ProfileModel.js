const dal = require('../DAL');

class ProfileModel {
    constructor(UUID,UserName,UserCode,Email,Address,Mobile,ProfilePhoto){
        this.UUID = UUID;
        this.UserName= UserName;
        this.UserCode = UserCode;
        this.Email = Email;
        this.Address = Address;
        this.Mobile = Mobile;
        this.ProfilePhoto = ProfilePhoto;
    }
    async ProfileDetails(UUID){
        try{
            let parameters = '';
            parameters += await dal.SP_Parameters('@chvnUUID',UUID);
    
            var result = await dal.SP_ExecuteData("[WebApplication_SP].[usp_GetParleSOProfileDetails]",parameters);
            return result;
        }
        catch(err){
            console.log(err);
        }
    }

    async InsertOTP(UUID,OTPNumber){
        try{
            let parameters = '';
            parameters += await dal.SP_Parameters('@chvUniqueCode',UUID);
            parameters += await dal.SP_Parameters('@chvOTPNumber',OTPNumber);
            parameters += await dal.SP_Parameters('@chvnOperationType','UPDATEOTP');
    
            var result = await dal.SP_ExecuteData("[WebApplication_SP].[usp_ParleSO_OTPUpdate]",parameters);
            return result;
        }
        catch(err){
            console.log(err);
        }
    }

    async ValidateOTP(UUID,OTPNumber){
        try{
            let parameters = '';
            parameters += await dal.SP_Parameters('@chvUniqueCode',UUID);
            parameters += await dal.SP_Parameters('@chvOTPNumber',OTPNumber);
            parameters += await dal.SP_Parameters('@chvnOperationType','VALIDATEOTP');
    
            var result = await dal.SP_ExecuteData("[WebApplication_SP].[usp_ParleSO_OTPUpdate]",parameters);
            return result;
        }
        catch(err){
            console.log(err);
        }
    }

    async UpdateProfileDetails(UUID,Email,Address){
        try{
            let parameters = '';
            parameters += await dal.SP_Parameters('@chvnUUID',UUID);
            parameters += await dal.SP_Parameters('@chvnEmail',Email);
            parameters += await dal.SP_Parameters('@chvnAddress',Address);
           // parameters += await dal.SP_Parameters('@chvnProfilePhoto',ProfilePhoto);
    
            var result = await dal.SP_ExecuteData("[WebApplication_SP].[usp_UpdateParleSalesmanKYCPersonalDetails]",parameters);
            return result;
        }
        catch(err){
            console.log(err);
        }
    }
}

module.exports = {
    ProfileModel: ProfileModel
}