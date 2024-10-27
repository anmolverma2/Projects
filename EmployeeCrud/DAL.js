var Db = require('./dbConfig');
var express = require('express');
var bodyParser = require('body-parser');
const sql = require('mssql');
var app = express();

async function SPExecuteDataTable(SP_Name,parameters){
    if(parameters.endsWith(',')) {
        parameters = parameters.slice(0, -1);
    }
    var myQuery = "EXEC "+SP_Name+" "+parameters;
    try{
        let pool = await sql.connect(Db);
        let data = await pool.request().query(myQuery);
        pool.close();
        return data.recordset;
    }
    catch (error){
        console.log(error);
    }
}
async function SPExecuteDataSet(SP_Name,parameters){
    if(parameters.endsWith(',')) {
        parameters = parameters.slice(0, -1);
    }
    var myQuery = "EXEC "+SP_Name+" "+parameters;
    try{
        let pool = await sql.connect(Db);
        let data = await pool.request().query(myQuery);
        pool.close();
        return data.recordsets;
    }
    catch (error){
        console.log(error);
    }
}
async function SP_Parameters(Key,Value){
    var myParam = Key+"='"+Value+"',";
    return myParam.toString();
}

module.exports ={
    SP_Parameters:SP_Parameters,
    SPExecuteDataTable:SPExecuteDataTable,
    SPExecuteDataSet:SPExecuteDataSet
}