using Microsoft.Data.SqlClient;
using Microsoft.Data;
using System.Data;

namespace MicroServicesProject.Infrastructure
{
    public class DAL 
    {
        readonly SqlConnection con;


        public string errortext = "";
        public DAL(SqlConnection _con)
        {
            con = _con;
        }
        public void connect()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
        }

        public void disconnect()
        {
            if (con.State == ConnectionState.Open)
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
            catch (Exception ex)
            {
                errortext += ex.Message + con.ConnectionString;
                return false;
            }
            return true;
        }

        public DataSet SPExecuteDataSet(string proc, SqlParameter[] parameters, string virtualtable)
        {
            try
            {
                if (proc != null)
                {
                    this.connect();
                    SqlCommand cmd = new SqlCommand(proc, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataSet ds = new DataSet();
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds, virtualtable);
                    return ds;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
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
                if (proc != null)
                {
                    this.connect();
                    SqlCommand cmd = new SqlCommand(proc, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParameters != null)
                    {
                        foreach (SqlParameter parameter in sqlParameters)
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
            catch (Exception ex)
            {
                errortext += ex.Message;
                return 0;
            }
            finally
            {
                this.disconnect();
            }

        }


        public IEnumerable<T> SPExecuteToList<T>(string proc, SqlParameter[] parameters, string virtualtable) where T : new()
        {
            try
            {
                if (!string.IsNullOrEmpty(proc))
                {
                    this.connect();
                    SqlCommand cmd = new SqlCommand(proc, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds, virtualtable);

                    if (ds.Tables[virtualtable] != null)
                    {
                        List<T> list = new List<T>();
                        foreach (DataRow row in ds.Tables[virtualtable].Rows)
                        {
                            T obj = new T();
                            foreach (var prop in typeof(T).GetProperties())
                            {
                                if (row.Table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                                {
                                    prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType), null);
                                }
                            }
                            list.Add(obj);
                        }
                        return list;
                    }
                    else
                    {
                        return new List<T>();
                    }
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (Exception ex)
            {
                errortext += ex.Message;
                return new List<T>();
            }
            finally
            {
                this.disconnect();
            }
        }

    }
}
