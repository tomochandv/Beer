using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BottleShop.Dac
{
    public class Dac_Promo : DbHelper
    {
        public int CreatePromo(int count,string cuser)
        {
            List<List<Parameter>> list = new List<List<Parameter>>();
            List<string> qry = new List<string>();
            for(int i=0; i< count; i++)
            {
                List<Parameter> paramlist = new List<Parameter>();
                paramlist.Add(new Parameter(Guid.NewGuid().ToString().Replace("-", ""), "@PORO_CODE", SqlDbType.VarChar));
                paramlist.Add(new Parameter(cuser, "@CUSER", SqlDbType.VarChar));
                list.Add(paramlist);
                qry.Add("SP_PROMOTION_MASTER_I");
            }
           
            return ExcuteNonQueryTran(qry, list);
        }

        public int UsePromo(string poro_code, string useid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(poro_code, "@PORO_CODE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(useid, "@USEID", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_PROMOTION_MASTER_U", paramlist);
        }

        public DataSet PromoList(int sidx, int eidx, string poro_code, string use, string useid, string usedate)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(poro_code, "@PORO_CODE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(use, "@ISUSE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(useid, "@USEID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(usedate, "@USEDATE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(sidx, "@SIDX", SqlDbType.Int));
            paramlist.Add(new Parameter(eidx, "@EIDX", SqlDbType.Int));
            return ExcuteToDataSet("SP_PROMOTION_MASTER_S", paramlist);
        }
    }
}