using Demclit_Pagos_Reporte.App_Data;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Demclit_Pagos_Reporte.App_Code
{
    public class Conect
    {
        #region constantes
        private const string NAB_SP_GLOBAL_LOG_EXCEPCIONES = "Nab_SP_Global_Log_Excepciones_Registro";
        #endregion
        #region Campos
        private static bool modeDeveloper = ConfigurationManager.AppSettings["ModeDeveloper"] == "true";
        private static bool modeTest = ConfigurationManager.AppSettings["ModeTest"] == "true";
        private SqlConnection conexion = new SqlConnection(modeDeveloper ? ConnectionSources.NabisDeveloper : (modeTest ? ConnectionSources.NabisTest : ConnectionSources.Nabis));
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        private Dictionary<string, SqlParameter> parametersOutput = new Dictionary<string, SqlParameter>();
        public Double timeQuery;
        public int numRows = 0;
        public int numColumns = 0;
        public bool error = false;
        #endregion

        #region Propiedades
        public string CommandQuery { get; set; }
        #endregion

        #region Metodos
        public static SqlConnection GetConexion()
        {
            Conect conect = new Conect();
            return conect.conexion;
        }

        public string GetCommand()
        {
            StringBuilder command = new StringBuilder();
            command.Append(this.CommandQuery);
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    command.Append(parameter.Key + ": '" + parameter.Value + "'");
                    command.Append(", ");
                }
            }
            if (parametersOutput != null)
            {
                foreach (SqlParameter parameter in parametersOutput.Values)
                {
                    command.Append(parameter.ParameterName + ": '" + parameter.Value + "'");
                    command.Append(", ");
                }
            }
            return command.ToString();
        }


        public void AddParameters(string key, object value)
        {
            if (!parameters.ContainsKey(key))
            {
                if (value == null)
                {
                    value = DBNull.Value;
                }
                else if (value.GetType().ToString() == "System.String")
                {
                    string valueTrim = value.ToString().Trim();
                    value = (valueTrim == "") ? (DBNull.Value) : (value);
                }

                parameters.Add(key, value);
            }
        }

        public void AddParametersOutPut(string key, object value, SqlDbType type)
        {
            if (!parametersOutput.ContainsKey(key))
            {
                SqlParameter param = new SqlParameter(key, value);
                param.SqlDbType = type;
                param.Direction = ParameterDirection.Output;
                parametersOutput.Add(key, param);
            }
        }

        public void AddParametersOutPut(string key, object value, SqlDbType type, int longVarchar)
        {
            if (!parametersOutput.ContainsKey(key))
            {
                SqlParameter param = new SqlParameter(key, value);
                param.SqlDbType = type;
                param.Direction = ParameterDirection.Output;
                param.Size = longVarchar;
                parametersOutput.Add(key, param);
            }
        }

        public void AddParametersInputOutPut(string key, object value, SqlDbType type)
        {
            if (!parametersOutput.ContainsKey(key))
            {
                SqlParameter param = new SqlParameter(key, value);
                param.SqlDbType = type;
                param.Direction = ParameterDirection.InputOutput;
                parametersOutput.Add(key, param);
            }
        }

        public object GetValueParameterOut(string keyParameter)
        {

            object value = parametersOutput[keyParameter].Value;
            return value;
        }

        public void ExecTransac(Boolean IsStoreProcedure = true)
        {
            SqlDataReader data;
            try
            {
                SqlCommand command = new SqlCommand(CommandQuery, conexion);
                //verifica si es procedimiento almacenado
                if (IsStoreProcedure)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //Asigna parametros de consulta o de procedimiento
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                        {
                            string typeParam = parameter.Value.GetType().ToString();

                            if (typeParam == "System.String")
                            {
                                SqlParameter param = new SqlParameter("@" + parameter.Key, SqlDbType.VarChar);
                                param.Value = parameter.Value;
                                command.Parameters.Add(param);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
                            }
                        }
                    }
                    //Asigna parametros Output
                    if (parametersOutput != null)
                    {
                        foreach (SqlParameter parameter in parametersOutput.Values)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                }
                //Time out de conexión a 0
                command.CommandTimeout = 0;
                //Abre conexión
                conexion.Open();
                //Ejecuta sentencia
                data = command.ExecuteReader();

                if (parametersOutput != null)
                {
                    foreach (SqlParameter item in parametersOutput.Values)
                    {
                        item.Value = command.Parameters[item.ParameterName].Value;
                    }
                }

                if (data.RecordsAffected > 0)
                {
                    numRows = data.RecordsAffected;
                }
            }
            catch (Exception ex)
            {
                conexion.Close();
                if (CommandQuery != NAB_SP_GLOBAL_LOG_EXCEPCIONES)
                {
                    this.Reset();
                    error = true;
                    this.Exception_Log(ex);
                }
            }
            finally
            {
                conexion.Close();
            }
        }

        public DataTable GetDataTable(bool IsStoreProcedure = true)
        {
            DataTable dataTable = new DataTable();

            SqlCommand command = new SqlCommand(CommandQuery, conexion);
            try
            {
                //verifica si es procedimiento almacenado
                if (IsStoreProcedure)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //Asigna parametros de consulta o de procedimiento
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                        {
                            if (parameter.Value.GetType().ToString() == "System.String")
                            {
                                SqlParameter param = new SqlParameter("@" + parameter.Key, SqlDbType.VarChar);
                                param.Value = parameter.Value;
                                command.Parameters.Add(param);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
                            }
                        }
                    }
                    if (parametersOutput != null)
                    {
                        foreach (SqlParameter parameter in parametersOutput.Values)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                }
                //Time out de conexión a 0
                command.CommandTimeout = 0;
                //Abre conexión
                conexion.Open();
                //Ejecuta sentencia
                SqlDataAdapter adapterData = new SqlDataAdapter();
                adapterData.SelectCommand = command;
                dataTable.Locale = CultureInfo.InvariantCulture;
                adapterData.Fill(dataTable);
                adapterData.Dispose();
            }
            catch (Exception ex)
            {
                error = true;
                conexion.Close();
                if (CommandQuery != NAB_SP_GLOBAL_LOG_EXCEPCIONES)
                {
                    this.Reset();
                    this.Exception_Log(ex);
                }
            }
            finally
            {
                conexion.Close();
                conexion.Dispose();
            }
            return dataTable;
        }

        public DataTable GetDataTable(string DataTablename, bool IsStoreProcedure = true)
        {
            DataTable dataTable = new DataTable(DataTablename);


            SqlCommand command = new SqlCommand(CommandQuery, conexion);
            try
            {
                //verifica si es procedimiento almacenado
                if (IsStoreProcedure)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //Asigna parametros de consulta o de procedimiento
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                        {
                            if (parameter.Value.GetType().ToString() == "System.String")
                            {
                                SqlParameter param = new SqlParameter("@" + parameter.Key, SqlDbType.VarChar);
                                param.Value = parameter.Value;
                                command.Parameters.Add(param);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
                            }
                        }
                    }
                    if (parametersOutput != null)
                    {
                        foreach (SqlParameter parameter in parametersOutput.Values)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                }
                //Time out de conexión a 0
                command.CommandTimeout = 0;
                //Abre conexión
                conexion.Open();
                //Ejecuta sentencia
                SqlDataAdapter adapterData = new SqlDataAdapter();
                adapterData.SelectCommand = command;
                dataTable.Locale = CultureInfo.InvariantCulture;
                adapterData.Fill(dataTable);
                adapterData.Dispose();
            }
            catch (Exception ex)
            {
                error = true;
                conexion.Close();
                if (CommandQuery != NAB_SP_GLOBAL_LOG_EXCEPCIONES)
                {
                    this.Reset();
                    this.Exception_Log(ex);
                }
            }
            finally
            {
                conexion.Close();
                conexion.Dispose();
            }
            return dataTable;
        }

        public void Exception_Log(Exception ex)
        {
            SqlDataReader data;
            string usrID = HttpContext.Current.User.Identity.Name;
            int codError = -1;
            if (ex is HttpException)
            {
                HttpException checkException = (HttpException)ex;
                codError = checkException.GetHttpCode();
            }

            try
            {
                string mensaje = ex.Message.ToString();
                AddParameters("host", HttpContext.Current.Request.Url);
                AddParameters("url_path", HttpContext.Current.Request.UserHostName);
                AddParameters("path", HttpContext.Current.Request.Path);
                AddParameters("linea", 0);
                AddParameters("usr_Login", usrID);
                AddParameters("ip", "");
                AddParameters("equipo", "");
                AddParameters("script", this.CommandQuery);
                AddParameters("cod_excepcion", codError);
                AddParameters("mensaje", mensaje);
                AddParameters("notas", "");
                AddParameters("fec_evento", DateTime.Now);
                SqlCommand command = new SqlCommand(NAB_SP_GLOBAL_LOG_EXCEPCIONES, conexion);
                command.CommandType = CommandType.StoredProcedure;
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    if (item.Value.GetType().ToString() == "System.String")
                    {
                        SqlParameter param = new SqlParameter("@" + item.Key, SqlDbType.VarChar);
                        param.Value = item.Value;
                        command.Parameters.Add(param);
                    }
                    else
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }
                //Time out de conexión a 0
                command.CommandTimeout = 0;
                //Abre conexión
                conexion.Open();
                //Ejecuta sentencia
                data = command.ExecuteReader();
                data.Close();

            }
            catch (Exception exLog)
            {
                throw exLog;
            }
            finally
            {
                this.Reset();
                conexion.Close();
                conexion.Dispose();
            }
        }

        public void Reset()
        {
            if (parameters.Count > 0) { this.parameters.Clear(); }
            if (this.parametersOutput != null) { this.parametersOutput.Clear(); }
            this.numRows = 0;
            this.numColumns = 0;
            this.timeQuery = 0;
            this.parametersOutput = null;
        }

        #endregion

        #region Constructores
        public Conect()
        {

        }

        public Conect(string conexionName)
        {
            string conexionString = (HttpContext.GetGlobalResourceObject("SourcesConection", modeTest ? conexionName + "_Test" : conexionName) ?? "").ToString();
            if (!String.IsNullOrEmpty(conexionString))
            {
                this.conexion = new SqlConnection(conexionString);
            }
            else
            {
                throw new Exception("El nombre de conexión " + conexionName + " no es válido.");
            }
        }

        public Conect(string ConectionString, bool ModeTest = true)
        {
            this.conexion = new SqlConnection(ConectionString);
        }
        #endregion
    }
}