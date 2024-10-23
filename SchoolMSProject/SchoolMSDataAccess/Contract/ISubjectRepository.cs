using SchoolMSDataAccess.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Contract
{
    public interface ISubjectRepository
    {
        List<SubjectModel> GetSubjectsData();
        List<TeacherModel> GetTeacherData();
        int InsertSubject(SubjectModel model);
    }
}
