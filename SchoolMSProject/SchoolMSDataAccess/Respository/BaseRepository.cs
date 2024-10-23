using SchoolMSDataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Respository
{
    public class BaseRepository
    {
        protected readonly IConnectionFactory connectionFactory;
        public BaseRepository(IConnectionFactory connection)
        {
            this.connectionFactory = connection;
        }
    }
}
