using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BottleShop
{
    public class Dac_Product : DbHelper
    {
        public DataSet ProductSelect(int category, string pr_name, int sidx, int eidx)
        {
            List<Parameter> paramArray = new List<Parameter>();
            paramArray.Add(new Parameter(category, "@BC_IDX", SqlDbType.Int));
            paramArray.Add(new Parameter(pr_name, "@PR_NAME", SqlDbType.VarChar));
            paramArray.Add(new Parameter(sidx, "@SIDX", SqlDbType.Int));
            paramArray.Add(new Parameter(eidx, "@EIDX", SqlDbType.Int));
            return ExcuteToDataSet("SP_PRODUCT_INFO_S", paramArray) ;
        }

        public List<Parameter>  Createparameter(object bc_idx, object pr_name, object pr_country, object pr_gubun, object pr_qty, object pr_liter
            , object pr_income, object pr_in_price, object pr_price, object pr_sale, object issale, object pr_weight, object pe_desc)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(bc_idx, "@BC_IDX", SqlDbType.Int));
            paramlist.Add(new Parameter(pr_name, "@PR_NAME", SqlDbType.VarChar));
            paramlist.Add(new Parameter(pr_country, "@PR_COUNTRY", SqlDbType.VarChar));
            paramlist.Add(new Parameter(pr_gubun, "@PR_GUBUN", SqlDbType.VarChar));
            paramlist.Add(new Parameter(pr_qty, "@PR_QTY", SqlDbType.Float));
            paramlist.Add(new Parameter(pr_liter, "@PR_LITER", SqlDbType.Int));
            paramlist.Add(new Parameter(pr_income, "@PR_INCOME", SqlDbType.VarChar));
            paramlist.Add(new Parameter(pr_in_price, "@PR_IN_PRICE", SqlDbType.Float));
            paramlist.Add(new Parameter(pr_price, "@PR_PRICE", SqlDbType.Float));
            paramlist.Add(new Parameter(pr_sale, "@PR_SALE", SqlDbType.Float));
            paramlist.Add(new Parameter(issale, "@ISSALE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(pr_weight, "@PR_WEIGHT", SqlDbType.Int));
            paramlist.Add(new Parameter(pe_desc, "@PE_DESC", SqlDbType.VarChar));

            return paramlist;
        }

        public void ProductInsert(List<List<Parameter>> paramList)
        {
           if(paramList.Count > 0)
           {
               List<string> qry = new List<string>();
               foreach(var data in paramList)
               {
                   qry.Add("SP_PRODUCT_INFO_I");
                   ExcuteNonQueryTran(qry, paramList);
               }
           }
        }

        public List<Parameter> CreateUpdateParameter(object pr_idx, object issale)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(pr_idx, "@PR_IDX", SqlDbType.Int));
            paramlist.Add(new Parameter(issale, "@ISSALE", SqlDbType.VarChar));
            return paramlist;
        }

        public int ProductUpdate(List<List<Parameter>> paramList)
        {
            int r = 0;
            if (paramList.Count > 0)
            {
                List<string> qry = new List<string>();
                foreach (var data in paramList)
                {
                    qry.Add("SP_PRODUCT_INFO_U");
                    r= ExcuteNonQueryTran(qry, paramList);
                }
            }
            return r;
        }
    }
}