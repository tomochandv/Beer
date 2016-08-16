using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BottleShop.Dac
{
    public class Dac_Notice : DbHelper
    {
        public int SaveNotice(string title, string content)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(title, "@TITLE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(content, "@CONTENTS", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_NOTICE_I", paramlist);
        }

        public DataSet ViewNotice(int idx)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(idx, "@IDX", SqlDbType.Int));
            return ExcuteToDataSet("SP_NOTICE_S1", paramlist);
        }

        public DataSet NoticeList(int sidx, int eidx)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(sidx, "@SIDX", SqlDbType.Int));
            paramlist.Add(new Parameter(eidx, "@EIDX", SqlDbType.Int));
            return ExcuteToDataSet("SP_NOTICE_S", paramlist);
        }
    }
}