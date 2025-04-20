using GadgetShopApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace GadgetShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string conString;
        public InventoryController(IConfiguration configuration)
        {
            _configuration = configuration;
            conString = _configuration.GetConnectionString("DbCon");
        }
        [HttpPost("SaveInventory")]
        public ActionResult SaveInventory([FromBody] InventoryModelDto request)
        {
            SqlConnection con = new SqlConnection
            {
                ConnectionString = conString
            };

            SqlCommand cmd = new SqlCommand
            {
                CommandText = "SP_Save_Update_Delete_Inventory",
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };

            cmd.Parameters.AddWithValue("@ProductId", request.ProductId);
            cmd.Parameters.AddWithValue("@ProductName", request.ProductName);
            cmd.Parameters.AddWithValue("@AvailableQty", request.AvailableQty);
            cmd.Parameters.AddWithValue("@ReorderPoint", request.ReorderPoint);
            cmd.Parameters.AddWithValue("@OperationType", "Save");

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();    

            return Ok();
        }

        [HttpGet("GetInventoryData")]
        public ActionResult GetInventoryData()
        {
            SqlConnection con = new SqlConnection
            {
                ConnectionString = conString
            };

            SqlCommand cmd = new SqlCommand
            {
                CommandText = "SP_GetInventoryData",
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };

            List<InventoryModelDto> response = new List<InventoryModelDto>();

            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    InventoryModelDto model = new InventoryModelDto();
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.ProductId = Convert.ToInt32(reader["ProductId"]);
                    model.ProductName = Convert.ToString(reader["ProductName"]);
                    model.AvailableQty = Convert.ToInt32(reader["AvailableQty"]);
                    model.ReorderPoint = Convert.ToInt32(reader["ReorderPoint"]);

                    response.Add(model);
                }
            }

            con.Close();

            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpPut("UpdateInventory")]
        public ActionResult UpdateInventory([FromBody] InventoryModelDto request)
        {
            SqlConnection con = new SqlConnection
            {
                ConnectionString = conString
            };

            SqlCommand cmd = new SqlCommand
            {
                CommandText = "SP_Save_Update_Delete_Inventory",
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };

            cmd.Parameters.AddWithValue("@Id", request.Id);
            cmd.Parameters.AddWithValue("@ProductId", request.ProductId);
            cmd.Parameters.AddWithValue("@ProductName", request.ProductName);
            cmd.Parameters.AddWithValue("@AvailableQty", request.AvailableQty);
            cmd.Parameters.AddWithValue("@ReorderPoint", request.ReorderPoint);
            cmd.Parameters.AddWithValue("@OperationType", "Update");

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            return Ok();
        }


        [HttpDelete]
        public ActionResult DeleteInventoryData(int Id)
        {
            SqlConnection con = new SqlConnection { ConnectionString = conString };
            SqlCommand cmd = new SqlCommand
            {
                CommandText = "SP_Save_Update_Delete_Inventory",
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };

            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Parameters.AddWithValue("@OperationType", "Delete");

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            return Ok();
        }


    }
}
