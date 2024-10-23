using SchoolMSDataAccess.Contract;
using SchoolMSDataAccess.Entities.Model;
using SchoolMSDataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Respository
{
    public class TeacherRepository : BaseRepository,ITeacherRepository
    { 
        public TeacherRepository(IConnectionFactory connection) : base(connection)
        {
            
        }
        public int InsertTeacherData(TeacherModel model)
        {
            int result = 0;

            using (var dbconnect = connectionFactory.getDAL)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@Name",SqlDbType.NVarChar){Value=model.Name},
                    new SqlParameter("@Age",SqlDbType.Int){Value=model.Age},
                    new SqlParameter("@Sex",SqlDbType.NVarChar){Value=model.Sex},
                    new SqlParameter("@Image",SqlDbType.NVarChar){Value=model.ImagePath},
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value="Insert"}
                };

                result = dbconnect.SPExecuteScalar("SP_Teachers", sqlParameters);
            }
            return result;
        }
    }
}
