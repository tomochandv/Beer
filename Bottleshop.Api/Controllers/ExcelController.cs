using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Bson;

namespace Bottleshop.Api.Controllers
{
    public class ExcelController : BaseController
    {
        //
        // GET: /Excel/

        public ActionResult Product()
        {
            string returnFilename = string.Empty;
            string oledbProviderString_ver2 = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=Yes; IMEX=1\"";

            string file = string.Format("e:\\Excel_mongo_product.xlsx");
            returnFilename = String.Format(oledbProviderString_ver2, file);
            OleDbConnection oledbConn = new OleDbConnection(returnFilename);
            oledbConn.Open();
            // Create OleDbCommand object and select data from worksheet Sheet1
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn);
            // Create new OleDbDataAdapter
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            oleda.SelectCommand = cmd;
            DataSet ds = new DataSet();
            oleda.Fill(ds, "1");

            List<Product> list = ConvertToList<Product>(ds.Tables[0]);
            foreach(var data in list)
            {
                data.InDate = DateTime.Now;
                //data.Id = ObjectId.GenerateNewId();
            }

            MongodbHelper.InsertManyModel<Product>(list, "Product");

            oledbConn.Close();


            return View();
        }

        #region DataTable, DataRow Bind Class
        public List<T> ConvertToList<T>(DataTable datatable) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
                return Temp;
            }
            catch
            {
                return Temp;
            }

        }

        public  dynamic ConvertToRow<T>(DataRow datarow) where T : new()
        {
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datarow.Table.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                return getObject<T>(datarow, columnsNames);
            }
            catch
            {
                return null;
            }

        }

        public  dynamic ConvertToRow<T>(DataTable datatable) where T : new()
        {
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                if (datatable.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    return getObject<T>(datatable.Rows[0], columnsNames);
                }
            }
            catch
            {
                return null;
            }

        }

        private  T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties;
                Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
        #endregion

    }
}
