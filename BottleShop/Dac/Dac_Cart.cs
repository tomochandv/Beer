using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace BottleShop.Dac
{
    public class Dac_Cart : DbHelper
    {
        public int AddCart(int pr_idx, string userid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(pr_idx, "@PR_IDX", SqlDbType.Int));
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_CART_I", paramlist);
        }

        public DataSet SelectCart(string userid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_MEMBER_CART_S", paramlist);
        }
    }
}