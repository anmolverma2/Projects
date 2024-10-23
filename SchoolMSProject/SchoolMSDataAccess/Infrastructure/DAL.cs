using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SchoolMSDataAccess.Infrastructure
{
    public class DAL : IDisposable
    {
        readonly SqlConnection con;
        public string errortext = "";
        private bool disposedValue;

        public DAL(SqlConnection conn)
        {
            con = conn;
        }

        public void connect()
        {
            if(con.State == ConnectionState.Closed)
            {
                con.Open();
            }
        }

        public void disconnect()
        {
            if(con.State == ConnectionState.Open)
            {
                con.Close();
                con.Dispose();
                SqlConnection.ClearPool(con);
            }
        }

        public bool checkConnection()
        {
            try
            {
                con.Open();
                con.Close();
            }
            catch(Exception ex)
            {
                errortext += ex.Message + con.ConnectionString;
                return false;
            }
            return true;
        }

        public DataSet SPExecuteDataSet(string proc, SqlParameter[] parameters,string virtualtable)
        {
            try
            {
                if(proc != null)
                {
                    this.connect();
                    SqlCommand cmd = new SqlCommand(proc,con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataSet ds = new DataSet();
                    if(parameters != null)
                    {
                        foreach(SqlParameter parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds,virtualtable);
                    return ds;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                errortext += ex.Message;
                return null;
            }
            finally
            {
                this.disconnect();
            }

        }

        public int SPExecuteScalar(string proc, SqlParameter[] sqlParameters)
        {
            int result = 0;
            try
            {
                if(proc != null)
                {
                    this.connect();
                    SqlCommand cmd = new SqlCommand(proc,con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if(sqlParameters != null)
                    {
                        foreach(SqlParameter parameter in sqlParameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    result = (int)cmd.ExecuteScalar();

                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex) { 
                errortext += ex.Message;
                return 0;
            }
            finally
            {
                this.disconnect();
            }

        }

        #region Disposable Support
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
        // ~DAL()
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
