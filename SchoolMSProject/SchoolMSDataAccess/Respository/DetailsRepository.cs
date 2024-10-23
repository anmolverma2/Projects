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
    public class DetailsRepository : BaseRepository,IDetailsRepository
    {
        public DetailsRepository(IConnectionFactory connection) : base(connection) 
        {
            
        }

        public List<DetailsModel> GetDetails()
        {
            List<DetailsModel> details = new List<DetailsModel>();
            DataSet ds = new DataSet();
            using (var dbconnect = connectionFactory.getDAL)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value = "List"},
                };
                ds = dbconnect.SPExecuteDataSet("SP_Details", sqlParameters, "ds");
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DetailsModel model = new DetailsModel();
                    model.TeacherName = dr["TeacherName"].ToString();
                    model.SubjectName = dr["SubjectName"].ToString();
                    model.Language = dr["Language"].ToString();
                    model.StudentName = dr["StudentName"].ToString();
                    model.Class = dr["Class"].ToString();

                    details.Add(model);
                }

                return details;
            }
            else
            {
                return null;
            }
        }
    }
}
