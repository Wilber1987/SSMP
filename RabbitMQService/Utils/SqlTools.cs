using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.Utils
{
    public static class SqlTools
    {
        public static string CnxServer { get; set; }


        public static DataTable AdoSqlGetDataTable(string sQuery, string alternativeCnx = "")
        {


            SqlConnection sqlConnection;
            if (string.IsNullOrEmpty(alternativeCnx))
            {
                sqlConnection =
                  new SqlConnection(CnxServer);
            }
            else
            {
                sqlConnection =
                  new SqlConnection(alternativeCnx);
            }



            sQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;" + sQuery;
            var DsResult = new DataSet();
            var Result = new DataTable();



            sqlConnection.Open();

            try
            {
                using (SqlDataAdapter dbAdapter = new SqlDataAdapter(sQuery, sqlConnection))
                {
                    dbAdapter.Fill(DsResult, "Table");
                    if (DsResult.Tables.Count > 0)
                    {
                        Result = DsResult.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                //Exception customException = new Exception();
                //if (ExceptionPolicy.HandleException(ex, "Business Layer Policy", customException))
                //{
                //    throw customException;
                //}
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                DsResult.Dispose();
                sqlConnection.Close();
                //dbCxn.Close();
            }

            return Result;

        }

        //public static IEnumerable<T> ExecSqlGetList<T>(string query, string alternativeCnx = "") where T : new()
        //{

        //    var obj = AdoSqlGetDataTable(query, alternativeCnx);
        //    var res = obj.ToList<T>();
        //    return res;
        //}

        public static List<T> SelectFromStoredProcedure<T>(string storedProcedureName, List<SqlParameter> parameters, string alternativeCnx = "") where T : new()
        {
            var result = new List<T>();

            SqlConnection sqlConnection;
            if (string.IsNullOrEmpty(alternativeCnx))
            {
                sqlConnection = new SqlConnection(CnxServer);
            }
            else
            {
                sqlConnection = new SqlConnection(alternativeCnx);
            }


            try
            {
                using (var connection = sqlConnection)
                {
                    using (var command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.Value));
                            }
                        }

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    T obj = new T();
                                    foreach (var prop in typeof(T).GetProperties())
                                    {
                                        if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                        {
                                            prop.SetValue(obj, reader[prop.Name]);
                                        }
                                    }
                                    result.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, or handle it in another way as needed)
                Console.WriteLine(ex.Message);
            }

            return result;
        }



        public static int ExecNonQuery(string sQuery, string alternativeCnx = "")
        {
            SqlConnection sqlConnection;
            if (string.IsNullOrEmpty(alternativeCnx))
            {
                sqlConnection =
                  new SqlConnection(CnxServer);
            }
            else
            {
                sqlConnection =
                  new SqlConnection(alternativeCnx);
            }

            int rowsAffected = -1;

            try
            {
                using (SqlCommand cmd = new SqlCommand(sQuery, sqlConnection))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlConnection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                //var demo = new Exception("SqlTools: ExecNonQuery Function fails");
                string innerMessage = string.Empty;
                if (ex.InnerException != null)
                    innerMessage = ex.InnerException.Message;

                var message = string.Format("{0}. {1}", ex.Message, innerMessage);
                throw new Exception(message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return rowsAffected;

        }

        public static string SQLExecute_TCv4WithParams(string sQuery, List<SqlParameter> parametros, string alternativeCnx = "")
        {
            sQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;" + sQuery;
            SqlConnection sqlConnection;
            if (string.IsNullOrEmpty(alternativeCnx))
            {
                sqlConnection =
                  new SqlConnection(CnxServer);
            }
            else
            {
                sqlConnection =
                  new SqlConnection(alternativeCnx);
            }

            object Result = null;
            SqlCommand dbCmd = null;

            using (var dbCxn = sqlConnection as SqlConnection)
            {


                dbCxn.Open();
                dbCmd = new SqlCommand(sQuery, dbCxn);
                try
                {
                    foreach (var param in parametros)
                    {
                        dbCmd.Parameters.Add(param);
                    }
                    dbCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    dbCxn.Close();
                    throw ex;
                }
                finally
                {
                    dbCxn.Close();
                }
            }
            return "";
        }

        public static object SQLGetWithParams(string sQuery, List<SqlParameter> parametros, string alternativeCnx = "")
        {
            sQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;" + sQuery;
            SqlConnection sqlConnection;
            if (string.IsNullOrEmpty(alternativeCnx))
            {
                sqlConnection =
                  new SqlConnection(CnxServer);
            }
            else
            {
                sqlConnection =
                  new SqlConnection(alternativeCnx);
            }

            object Result = null;
            SqlCommand dbCmd = null;
            using (var dbCxn = sqlConnection as SqlConnection)
            {
                dbCxn.Open();
                dbCmd = new SqlCommand(sQuery, dbCxn);
                try
                {
                    foreach (var param in parametros)
                    {
                        dbCmd.Parameters.Add(param);
                    }
                    Result = dbCmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    dbCxn.Close();
                }
            }
            return Result;
        }


        /// <summary>
        /// Funcion que ejecuta un escalar
        /// </summary>
        /// <param name="sQuery">Comando que se va a ejecutar</param>
        /// <returns>string con el valor resultado de la consulta</returns>
        public static object SQLGet(string sQuery, string alternativeCnx = "")
        {
            try
            {
                string sqlConnection;
                if (string.IsNullOrEmpty(alternativeCnx))
                {
                    sqlConnection = CnxServer;
                }
                else
                {
                    sqlConnection = alternativeCnx;
                }

                using (var myConnection = new SqlConnection(sqlConnection))
                {
                    SqlCommand command = new SqlCommand(sQuery, myConnection);
                    command.Connection.Open();
                    return command.ExecuteScalar();
                }
            }
            catch (Exception)
            {

                return null;
            }
        }


        public static ResultAdoCls ExecSqlWithTransaction(List<string> operations, string alternativeCnx = "")
        {

            var conexionString = string.IsNullOrEmpty(alternativeCnx) ? CnxServer : alternativeCnx;

            var result = new ResultAdoCls(); //
            using (var connection = new SqlConnection(conexionString))
            {
                connection.Open();
                SqlTransaction sqlTran = connection.BeginTransaction();
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                try
                {
                    //aqui insertamos todas la lineas como conjunto
                    foreach (var operation in operations)
                    {
                        command.CommandText = operation;
                        command.ExecuteNonQuery();
                    }

                    sqlTran.Commit();
                    Console.WriteLine("cambios realizados!");

                }
                catch (Exception ex)
                {
                    result.Error = true;
                    result.Message = ex.Message;
                    Console.WriteLine(ex.Message);

                    try
                    {

                        sqlTran.Rollback();
                    }
                    catch (Exception exRollback)
                    {
                        Console.WriteLine(exRollback.Message);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

    }

    public class ResultAdoCls
    {
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
