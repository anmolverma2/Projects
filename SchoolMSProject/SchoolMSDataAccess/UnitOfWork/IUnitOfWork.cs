using SchoolMSDataAccess.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository studentRepository { get; }
        ITeacherRepository teacherRepository { get; }
        ISubjectRepository subjectRepository { get; }
        IDetailsRepository detailRepository { get; }
    }
}
