using SchoolMSDataAccess.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Contract
{
    public interface IStudentRepository
    {
        List<StudentModel> GetStudentData(string name, string className);
        int InsertStudentData(StudentModel model);
    }
}
