using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BottleShop
{
    public class DbHelper : DbAgent
    {
        private string ConnectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        Parameter parameter = new Parameter();
        public DbHelper()
        { }

        /// <summary>
        /// DataSet return
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public DataSet ExcuteToDataSet(string qry, List<Parameter> paramList)
        {
            DataSet ds = new DataSet();
            try
            {
                ds = SqlGetDataSet(ConnectionString, qry, parameter.SqlParameterList(paramList), CommandType.StoredProcedure);
            }
            catch (Exception ex)
            { throw ex; }
            return ds;
        }

        /// <summary>
        /// 저장,인서트,딜리트
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string qry, List<Parameter> paramList)
        {
            int row = 0;
            try
            {
                row = SqlExcuteNonQuery(ConnectionString, qry, parameter.SqlParameterList(paramList), CommandType.StoredProcedure);
            }
            catch (Exception ex)
            { throw ex; }
            return row;
        }

        public int ExcuteNonQueryTran(List<string> qry, List<List<Parameter>> paramList)
        {
            int row = 0;
            try
            {
                List<List<SqlParameter>> list = new List<List<SqlParameter>>();
                foreach (var param in paramList)
                {
                    list.Add(parameter.SqlParameterList(param));
                }
                row = SqlExcuteNonQuery(ConnectionString, qry, list, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            { throw ex; }
            return row;
        }

        /// <summary>
        /// 오브젝트 한개 리턴
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public object ExcuteScalar(string qry, List<Parameter> paramList)
        {
            object obj;
            try
            {
                obj = SqlGetScarlar(ConnectionString, qry, parameter.SqlParameterList(paramList), CommandType.StoredProcedure);
            }
            catch (Exception ex)
            { throw ex; }
            return obj;
        }

    }

    #region Parameter Class
    public class Parameter
    {
        /// <summary>
        /// parameter
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="dbType">Column type</param>
        /// <param name="size">size</param>
        public Parameter(object value, string parameterName, SqlDbType dbType, int size)
        {
            Value = value;
            ParameterName = parameterName;
            DbType = dbType;
            Size = size;
        }
        /// <summary>
        /// parameter
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="dbType">Column type</param>
        public Parameter(object value, string parameterName, SqlDbType dbType)
        {
            Value = value;
            ParameterName = parameterName;
            DbType = dbType;
        }
        public Parameter()
        {

        }
        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Db type
        /// </summary>
        public SqlDbType DbType { get; set; }
        /// <summary>
        /// Size 
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Parameter Name
        /// </summary>
        public string ParameterName { get; set; }

        public List<SqlParameter> SqlParameterList(List<Parameter> paramList)
        {
            List<SqlParameter> paramCollection = new List<SqlParameter>();
            if (paramList != null)
            {
                foreach (var data in paramList)
                {
                    SqlParameter param = new SqlParameter();
                    param.Value = data.Value;
                    param.ParameterName = data.ParameterName.Replace(":", "@");
                    if (data.Size != 0)
                    {
                        param.Size = data.Size;
                    }
                    param.SqlDbType = data.DbType;
                    paramCollection.Add(param);
                }
            }
            return paramCollection;
        }
    }
    #endregion

    #region DbAgent Class
    public class DbAgent
    {
        #region Global Property
        private SqlConnection sqlCon = new SqlConnection();
        private SqlCommand sqlCom = new SqlCommand();
        private SqlDataAdapter sqlAdapeter = new SqlDataAdapter();
        #endregion

        /// <summary>
        /// Sql Transection
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="query">query</param>
        /// <param name="param">parameter</param>
        /// <param name="type">query type</param>
        /// <returns>effect rows</returns>
        #region protected int SqlExcuteNonQuery(string connectionString, string query, SqlParameterCollection param, CommandType type)
        protected int SqlExcuteNonQuery(string connectionString, string query, List<SqlParameter> param, CommandType type)
        {
            int result = 0;
            try
            {
                sqlCon.ConnectionString = connectionString;
                sqlCom.CommandText = Build(query);
                sqlCom.CommandType = type;
                sqlCom.Connection = sqlCon;
                sqlCom.Parameters.Clear();
                if (param != null)
                {
                    foreach (var data in param)
                    {
                        sqlCom.Parameters.Add(data);
                    }
                }

                sqlCon.Open();
                result = sqlCom.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Oracle Transection Query
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="query">query list</param>
        /// <param name="param">parameter list</param>
        /// <param name="type">query type</param>
        /// <returns></returns>
        #region protected int SqlExcuteNonQuery(string connectionString, string query, SqlParameterCollection param, CommandType type)
        protected int SqlExcuteNonQuery(string connectionString, List<string> query, List<List<SqlParameter>> param, CommandType type)
        {

            int result = 0;
            try
            {
                if (query.Count == param.Count)
                {
                    sqlCon.ConnectionString = connectionString;
                    sqlCon.Open();
                    sqlCom = sqlCon.CreateCommand();
                    SqlTransaction tran = sqlCon.BeginTransaction();
                    sqlCom.Transaction = tran;
                    for (int i = 0; i < query.Count; i++)
                    {
                        sqlCom.CommandText = Build(query[i]);
                        sqlCom.CommandType = type;
                        if (param != null)
                        {
                            sqlCom.Parameters.Clear();
                            foreach (var data in param[i])
                            {
                                sqlCom.Parameters.Add(data);
                            }
                        }
                        result += sqlCom.ExecuteNonQuery();
                    }
                    sqlCom.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                sqlCom.Transaction.Rollback();
                throw ex;
            }
            sqlCon.Close();
            return result;
        }
        #endregion


        /// <summary>
        /// Sql Get DataSet
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="query">query</param>
        /// <param name="param">parameter</param>
        /// <param name="type">query type</param>
        /// <returns>dataset</returns>
        #region protected DataSet SqlGetDataSet(string connectionString, string query, SqlParameterCollection param, CommandType type)
        protected DataSet SqlGetDataSet(string connectionString, string query, List<SqlParameter> param, CommandType type)
        {
            DataSet ds = new DataSet();
            try
            {
                sqlCon.ConnectionString = connectionString;
                sqlCom.CommandText = Build(query);
                sqlCom.CommandType = type;
                sqlCom.Connection = sqlCon;
                sqlCom.Parameters.Clear();
                if (param != null)
                {
                    foreach (var data in param)
                    {
                        sqlCom.Parameters.Add(data);
                    }
                }
                sqlAdapeter.SelectCommand = sqlCom;
                sqlCon.Open();
                sqlAdapeter.Fill(ds);
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        #endregion

        /// <summary>
        /// Sql Get Scalar
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="query">query</param>
        /// <param name="param">parameter</param>
        /// <param name="type">query type</param>
        /// <returns>string data</returns>
        #region protected string SqlGetScarlar(string connectionString, string query, SqlParameterCollection param, CommandType type)
        protected string SqlGetScarlar(string connectionString, string query, List<SqlParameter> param, CommandType type)
        {
            string result = string.Empty;
            try
            {
                sqlCon.ConnectionString = connectionString;
                sqlCom.CommandText = Build(query);
                sqlCom.CommandType = type;
                sqlCom.Connection = sqlCon;
                sqlCom.Parameters.Clear();
                if (param != null)
                {
                    foreach (var data in param)
                    {
                        sqlCom.Parameters.Add(data);
                    }
                }

                sqlCon.Open();
                result = sqlCom.ExecuteScalar().ToString();
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion


        /// <summary>
        /// query convert
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        #region query convert
        private string Build(string qry)
        {
            string returnQuery = string.Empty;
            returnQuery = qry.ToUpper().
                Replace("NVL", "ISNULL").
                Replace("LENGTH", "LEN").
                Replace("SYSDATE", "GETDATE()").
                //Replace("' '", "''").
                Replace("||", "+").
                Replace(":", "@").
                Replace("\r\n", " ");

            return returnQuery;
        }
        #endregion
    }
    #endregion
}
