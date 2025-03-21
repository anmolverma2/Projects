using System.Data;
using Microsoft.Data.SqlClient;

namespace MicroServicesProject.Infrastructure
{
    public interface IDAL
    {
        DataSet SPExecuteDataSet(string proc, SqlParameter[] parameters, string virtualtable);
        int SPExecuteScalar(string proc, SqlParameter[] sqlParameters);
        IEnumerable<T> SPExecuteToList<T>(string proc, SqlParameter[] parameters, string virtualtable) where T : new();
    }
}
