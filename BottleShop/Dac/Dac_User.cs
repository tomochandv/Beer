using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BottleShop.Dac
{
    public class Dac_User : DbHelper
    {
        public DataTable Login(string id)
        {
            List<Parameter> paramArray = new List<Parameter>();
            paramArray.Add(new Parameter(id, "@USERID", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_MEMBER_INFO_S", paramArray).Tables[0];
        }
    }
}