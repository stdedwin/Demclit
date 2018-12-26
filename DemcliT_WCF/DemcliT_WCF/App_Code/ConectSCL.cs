using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OracleClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using DemcliT_WCF.App_Data;

namespace DemcliT_WCF.App_Code
{
    public class ConectSCL
    {
        #region Campos
        private static bool modeTest = ConfigurationManager.AppSettings["ModeTestSCL"] == "true";
        private OracleConnection ConnectionOracle = new OracleConnection(modeTest ? ConnectionSources.SCL : ConnectionSources.SCL);
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        public Double timeQuery;
        public int numRows = 0;
        public int numColumns = 0;
        public bool error = false;
        #endregion

        #region Propiedades
        public string CommandQuery { get; set; }
        #endregion

        private string connectionString { get; set; }

        private string connectionStringOracle { get; set; }

        //private int connectionTimes;

        public static string SafeSqlLiteral(System.Object theValue, System.Object theLevel)
        {
            // Written by user CWA, CoolWebAwards.com Forums. 2 February 2010
            // http://forum.coolwebawards.com/threads/12-Preventing-SQL-injection-attacks-using-C-NET

            // intLevel represent how thorough the value will be checked for dangerous code
            // intLevel (1) - Do just the basic. This level will already counter most of the SQL injection attacks
            // intLevel (2) -   (non breaking space) will be added to most words used in SQL queries to prevent unauthorized access to the database. Safe to be printed back into HTML code. Don't use for usernames or passwords

            string strValue = (string)theValue;
            int intLevel = (int)theLevel;

            if (strValue != null)
            {
                if (intLevel > 0)
                {
                    strValue = strValue.Replace("'", "''"); // Most important one! This line alone can prevent most injection attacks
                    strValue = strValue.Replace("--", "");
                    strValue = strValue.Replace("[", "[[]");
                    strValue = strValue.Replace("%", "[%]");
                }
                if (intLevel > 1)
                {
                    string[] myArray = new string[] { "xp_ ", "update ", "insert ", "select ", "drop ", "alter ", "create ", "rename ", "delete ", "replace " };
                    int i = 0;
                    int i2 = 0;
                    int intLenghtLeft = 0;
                    for (i = 0; i < myArray.Length; i++)
                    {
                        string strWord = myArray[i];
                        Regex rx = new Regex(strWord, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        MatchCollection matches = rx.Matches(strValue);
                        i2 = 0;
                        foreach (Match match in matches)
                        {
                            GroupCollection groups = match.Groups;
                            intLenghtLeft = groups[0].Index + myArray[i].Length + i2;
                            strValue = strValue.Substring(0, intLenghtLeft - 1) + "&nbsp;" + strValue.Substring(strValue.Length - (strValue.Length - intLenghtLeft), strValue.Length - intLenghtLeft);
                            i2 += 5;
                        }
                    }
                }
                return strValue;
            }
            else
            {
                return strValue;
            }
        }

        public bool ConnectOracle()
        {
            bool ans = false;
            try
            {
                if (ConnectionOracle != null && ConnectionOracle.State == ConnectionState.Executing)
                {
                    throw new Exception("No se puede ejecutar mas de una consulta al tiempo desde el mismo controlador");
                }
                if (ConnectionOracle != null && ConnectionOracle.State == ConnectionState.Fetching)
                {
                    throw new Exception("No se puede ejecutar mas de una consulta al tiempo desde el mismo controlador");
                }
                if (ConnectionOracle != null)
                {
                    if (ConnectionOracle.State == ConnectionState.Closed || ConnectionOracle.State == ConnectionState.Broken)
                    {
                        ConnectionOracle.Open();
                    }
                }
                else
                {
                    ConnectionOracle = new OracleConnection(this.connectionStringOracle);
                    ConnectionOracle.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (ConnectionOracle.State == ConnectionState.Open)
            {
                ans = true;
            }
            return ans;
        }

        public DataTable ExecuteQueryOracle()
        {
            DataTable ans = null;
            if (ConnectionOracle.State != ConnectionState.Open)
            {
                ConnectionOracle.Open();
                //throw new Exception("No se puede Ejecutar una consulta con una conexión Cerrada");
            }

            /*OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "alter session set NLS_NUMERIC_CHARACTERS='.,'";
            cmd.Connection = ConnectionOracle;
            cmd.ExecuteNonQuery();*/

            DataTable dt = new DataTable();
            OracleDataAdapter oAdapterOracle = new OracleDataAdapter(this.CommandQuery, ConnectionOracle);
            oAdapterOracle.Fill(dt);
            ans = dt;

            ConnectionOracle.Close();

            return ans;
        }

        public static object ConvertNumberSCL(object value, string tipo)
        {
            object result = null;

            if (tipo.ToString() == "System.Int32")
            {
                result = string.IsNullOrEmpty(value.ToString()) ? (int?)null : Int32.Parse(value.ToString());
            }
            else
            {
                if (tipo.ToString() == "System.Int64")
                {
                    result = string.IsNullOrEmpty(value.ToString()) ? (long?)null : Int64.Parse(value.ToString());
                }
                else
                {
                    if (tipo.ToString() == "System.Decimal")
                    {
                        result = string.IsNullOrEmpty(value.ToString()) ? (decimal?)null : decimal.Parse(value.ToString());
                    }
                }
            }
            return result;
        }

        private static bool validateNumber(object value)
        {
            bool ans = false;
            float dummyValue;

            ans = float.TryParse(value.ToString(), out dummyValue);

            return ans;
        }

        private static bool validateString(string value)
        {
            bool ans = false;
            string safeVersion = string.Empty;
            safeVersion = SafeSqlLiteral(value, 1);
            if (value != safeVersion)
            {
                throw new Exception("Se ha detectado posible ataque de Inyección de SQL, la consulta NO se ejecutará");
            }
            return ans;
        }
    }
}