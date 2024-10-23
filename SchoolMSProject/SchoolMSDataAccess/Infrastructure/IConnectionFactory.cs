using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Infrastructure
{
    public interface IConnectionFactory : IDisposable
    {
        DAL getDAL {  get; }
    }
}
