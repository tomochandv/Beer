using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace BottleShop.Dac
{
    public class Dac_Popup : DbHelper
    {
        public int SetPopUp(string title, string contents, string useyn)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(title, "@TITLE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(contents, "@CONTENTS", SqlDbType.VarChar));
            paramlist.Add(new Parameter(useyn, "@USEYN", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_POPUP_I", paramlist);
        }

        public DataSet GetPopUp()
        {
            return ExcuteToDataSet("SP_POPUP_S", null);
        }
    }
}