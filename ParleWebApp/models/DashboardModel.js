const dal = require("../DAL");

class Dashboard {
    constructor(UUID){
        this.UUID = UUID;
    }
    async DashboardDetails(UUID){
        try{
            let parameters = '';
            parameters += await dal.SP_Parameters('@chvnUUID',UUID);
            parameters += await dal.SP_Parameters('@chvnOperationType','GetDashboardDetails');
    
            var result = await dal.SP_ExecuteData("[WebApplication_SP].[usp_GetParleDashboardDetailsData]",parameters);
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

}

module.exports = {
    Dashboard: Dashboard
}