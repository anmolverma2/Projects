const dal = require('../DAL');

class Payment{
    constructor(UUID){
        this.UUID = UUID;
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
}

module.exports = {
    Payment: Payment
}