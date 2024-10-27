const sql = require('mssql');
const conn = require('./DB/conn');


async function SP_Parameters(key,value){
    try{      
        var Params = key + "='" + value + "',";
        return Params.toString();
    }
    catch(err){
        console.log(err);
    }
}

async function SP_ExecuteData(sp_name,parameters){
    if(parameters.endsWith(',')) {
        parameters = parameters.slice(0, -1);
    }
    let query = 'EXEC ' + sp_name + ' ' + parameters;
    try{
        let pool = await sql.connect(conn);
        let data = await pool.request().query(query);
        pool.close();
        return data.recordset;
    }
    catch(err){
        console.log(err);
    }

}

async function SP_ExecuteExport(sp_name,parameters){
    try{
        let pool = await sql.connect(conn);
        let request = await pool.request();

        // Add Parameters to request
        if(parameters){
            Object.keys(parameters).forEach((key) =>{
                request.input(key,parameters[key]);

            })
        }
        
        // Execute sp with above parameters
        let data = await request.execute(sp_name);
        return data;
    }
    catch(err){
        console.log(err);
    }
}

module.exports = {
    SP_Parameters : SP_Parameters,
    SP_ExecuteData : SP_ExecuteData,
    SP_ExecuteExport : SP_ExecuteExport

}