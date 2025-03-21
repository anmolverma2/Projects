using Microsoft.Data.SqlClient;
using Microsoft.Data;
namespace MicroServicesProject.Infrastructure
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string connectionString;
        public ConnectionFactory(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DBConn");
        }
        public DAL GetDAL
        {
            get
            {
                SqlConnection con = new SqlConnection(connectionString);
                DAL dal = new DAL(con);
                return dal;
            }
        }
    }
}
