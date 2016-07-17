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

        public int UserJon(string id = "", string pwd = "", string name = "", string tell = "", string email = "", string addr = "", string sms = "Y", string isemail = "Y")
        {
            List<Parameter> paramArray = new List<Parameter>();
            paramArray.Add(new Parameter(id, "@USERID", SqlDbType.VarChar));
            paramArray.Add(new Parameter(pwd, "@PWD", SqlDbType.VarChar));
            paramArray.Add(new Parameter(name, "@NAME", SqlDbType.VarChar));
            paramArray.Add(new Parameter(tell, "@TELL", SqlDbType.VarChar));
            paramArray.Add(new Parameter(email, "@EMAIL", SqlDbType.VarChar));
            paramArray.Add(new Parameter(addr, "@ADDR", SqlDbType.VarChar));
            paramArray.Add(new Parameter(sms, "@ISSMS", SqlDbType.VarChar));
            paramArray.Add(new Parameter(isemail, "@ISEMAIL", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_INFO_I", paramArray);
        }

        public int UserPay(string id, string ptype, float price, DateTime sdate, DateTime edate, string use)
        {
            List<Parameter> paramArray = new List<Parameter>();
            paramArray.Add(new Parameter(id, "@USERID", SqlDbType.VarChar));
            paramArray.Add(new Parameter(ptype, "@PTYPE", SqlDbType.VarChar));
            paramArray.Add(new Parameter(price, "@PRICE", SqlDbType.Float));
            paramArray.Add(new Parameter(sdate, "@SDATE", SqlDbType.DateTime));
            paramArray.Add(new Parameter(edate, "@EDATE", SqlDbType.DateTime));
            paramArray.Add(new Parameter(ptype, "@ISUSE", SqlDbType.VarChar));

            return ExcuteNonQuery("SP_MEMBER_PAYINFO_I", paramArray);
        }

        public object CheckEmailAndId(string id = "", string email = "", string type ="")
        {
            List<Parameter> paramArray = new List<Parameter>();
            paramArray.Add(new Parameter(id, "@USERID", SqlDbType.VarChar));
            paramArray.Add(new Parameter(email, "@EMAIL", SqlDbType.VarChar));
            paramArray.Add(new Parameter(type, "@TYPE", SqlDbType.VarChar));

            return ExcuteScalar("SP_MEMBER_INFO_S3", paramArray);
        }

        public DataTable PayInfoUse(string id)
        {
            List<Parameter> paramArray = new List<Parameter>();
            paramArray.Add(new Parameter(id, "@USERID", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_MEMBER_PAYINFO_S", paramArray).Tables[0];
        }
    }
}