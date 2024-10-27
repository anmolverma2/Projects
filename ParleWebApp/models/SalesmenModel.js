const dal = require('../DAL');

class Salesmen {
    constructor(UUID){
        this.UUID = UUID;
    }
    async DSRDetails(UUID){
        try{
            let parameters = '';
            parameters += await dal.SP_Parameters('@chvnUUID',UUID);
            parameters += await dal.SP_Parameters('@chvnOperationType','GetDSRKYCDetailDetails');
    
            var result = await dal.SP_ExecuteData("[WebApplication_SP].[usp_GetParleSalesmanKYCPersonalDetails]",parameters);
            return result;
        }
        catch(err){
            console.log(err);
        }
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

    async GetParentUUID(UUID){
        try{
            let parameters = '';
            parameters += await dal.SP_Parameters('@chvnUUID',UUID);
    
            var result = await dal.SP_ExecuteData("[WebApplication_SP].[usp_GetParleParentUUID]",parameters);
            return result;
        }
        catch(err){
            console.log(err);
        }
    }
    
}

module.exports = {
    Salesmen:Salesmen

}