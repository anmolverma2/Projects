using SchoolMSDataAccess.Contract;
using SchoolMSDataAccess.Infrastructure;
using SchoolMSDataAccess.Respository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IConnectionFactory _connectionFactory;
        private bool disposedValue;

        public UnitOfWork(IConnectionFactory connection)
        {
            _connectionFactory = connection;
        }

        private IStudentRepository _studentRepository;
        public IStudentRepository studentRepository
        {
            get
            {
                if (_studentRepository == null) {
                    _studentRepository = new StudentRepository(_connectionFactory);
                }
                return _studentRepository;
            }
        }


        private ITeacherRepository _teacherRepository;
        public ITeacherRepository teacherRepository
        {
            get
            {
                if (_teacherRepository == null)
                {
                    _teacherRepository = new TeacherRepository(_connectionFactory);
                }
                return _teacherRepository;
            }
        }

        private ISubjectRepository _subjectRepository;
        public ISubjectRepository subjectRepository
        {
            get
            {
                if (_subjectRepository == null)
                {
                    _subjectRepository = new SubjectRepository(_connectionFactory);
                }
                return _subjectRepository;
            }
        }
        
        private IDetailsRepository _detailRepository;
        public IDetailsRepository detailRepository
        {
            get
            {
                if (_detailRepository == null)
                {
                    _detailRepository = new DetailsRepository(_connectionFactory);
                }
                return _detailRepository;
            }
        }

        #region Dispose Pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnitOfWork()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
