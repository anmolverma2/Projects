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
    public class SubjectRepository : BaseRepository,ISubjectRepository
    {
        public SubjectRepository(IConnectionFactory connection) : base(connection) 
        {
            
        }
        public List<SubjectModel> GetSubjectsData()
        {
            List<SubjectModel> subjects = new List<SubjectModel>();
            DataSet ds = new DataSet();
            using (var dbconnect = connectionFactory.getDAL)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value = "Subject"},
                };
                ds = dbconnect.SPExecuteDataSet("SP_Subjects", sqlParameters, "ds");
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SubjectModel model = new SubjectModel();
                    model.Id = Convert.ToInt32(dr["Id"].ToString());
                    model.Name = dr["Name"].ToString();

                    subjects.Add(model);
                }

                return subjects;
            }
            else
            {
                return null;
            }
        }

        public List<TeacherModel> GetTeacherData()
        {
            List<TeacherModel> teacher = new List<TeacherModel>();
            DataSet ds = new DataSet();
            using (var dbconnect = connectionFactory.getDAL)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value = "Teacher"},
                };
                ds = dbconnect.SPExecuteDataSet("SP_Subjects", sqlParameters, "ds");
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TeacherModel model = new TeacherModel();
                    model.Id = Convert.ToInt32(dr["T_Id"].ToString());
                    model.Name = dr["Name"].ToString();

                    teacher.Add(model);
                }

                return teacher;
            }
            else
            {
                return null;
            }
        }

        public int InsertSubject(SubjectModel model)
        {
            int result = 0;

            using (var dbconnect = connectionFactory.getDAL)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@Name",SqlDbType.NVarChar){Value=model.Name},
                    new SqlParameter("@Language",SqlDbType.NVarChar){Value=model.Language},
                    new SqlParameter("@Class",SqlDbType.NVarChar){Value=model.Class},
                    new SqlParameter("@TeacherId",SqlDbType.NVarChar){Value=model.TeacherId},
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value="Insert"}
                };

                result = dbconnect.SPExecuteScalar("SP_Subjects", sqlParameters);
            }
            return result;
        }
    }
}
