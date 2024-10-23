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
    public class StudentRepository : BaseRepository,IStudentRepository
    {
        public StudentRepository(IConnectionFactory connection) : base(connection)
        {
            
        }

       public List<StudentModel> GetStudentData(string name,string className)
        {
            List<StudentModel> student = new List<StudentModel>();
            DataSet ds = new DataSet();
            using(var dbconnect = connectionFactory.getDAL)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@className",SqlDbType.NVarChar){Value = className},
                    new SqlParameter("@Name",SqlDbType.NVarChar){Value = name},
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value = "List"},
                };
                ds = dbconnect.SPExecuteDataSet("SP_Students", sqlParameters, "ds");
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0) {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StudentModel model = new StudentModel();
                    model.Id = Convert.ToInt32(dr["Std_Id"]);
                    model.Name = dr["Name"].ToString();
                    model.Age = Convert.ToInt32(dr["Age"]);
                    model.Image = dr["ImagePath"].ToString();
                    model.Class = dr["Class"].ToString();
                    model.RollNumber = dr["RollNumber"].ToString();

                    student.Add(model);
                }

                return student;
            }
            else
            {
                return null;
            }
        }

        public int InsertStudentData(StudentModel model)
        {
            int result = 0;

            using (var dbconnect = connectionFactory.getDAL)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@Name",SqlDbType.NVarChar){Value=model.Name},
                    new SqlParameter("@Age",SqlDbType.Int){Value=model.Age},
                    new SqlParameter("@Class",SqlDbType.NVarChar){Value=model.Class},
                    new SqlParameter("@RollNumber",SqlDbType.NVarChar){Value=model.RollNumber},
                    new SqlParameter("@Image",SqlDbType.NVarChar){Value=model.Image},
                    new SqlParameter("@SubjectId",SqlDbType.Int){Value=model.SubjectId},
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value="Insert"}
                };

                result = dbconnect.SPExecuteScalar("SP_Students", sqlParameters);
            }
            return result;
        }

    }
}
