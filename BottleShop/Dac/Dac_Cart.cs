﻿using System;
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
        public int DeleteCart(int pr_idx, string userid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(pr_idx, "@PR_IDX", SqlDbType.Int));
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_CART_D", paramlist);
        }

        public int OrderSp(string message, string userid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(message, "@MESSAGE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_ORDER_PRODUCT_I", paramlist);
        }

        public DataSet SelectCart(string userid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_MEMBER_CART_S", paramlist);
        }

        public object OrderInfo(string userid, float totalPrice, DateTime endDate)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(totalPrice, "@TOTAL_PRICE", SqlDbType.Float));
            paramlist.Add(new Parameter(endDate, "@END_DATE", SqlDbType.DateTime));
            return ExcuteScalar("SP_ORDER_INFO_I", paramlist);
        }

        public int OrderProduct(int or_idx, int pr_idx, int qty,float price)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(pr_idx, "@PR_IDX", SqlDbType.Int));
            paramlist.Add(new Parameter(or_idx, "@OR_IDX", SqlDbType.Int));
            paramlist.Add(new Parameter(qty, "@QTY", SqlDbType.Int));
            paramlist.Add(new Parameter(price, "@PRICE", SqlDbType.Float));
            return ExcuteNonQuery("SP_ORDER_PRODUCT_I1", paramlist);
        }

        public DataSet OrderHistory(string userid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_ORDER_PRODUCT_S", paramlist);
        }

        public DataSet OrderHistory(int sidx, int eidx, string name, string userid, int or_idx, string email, string birth)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(or_idx, "@OR_IDX", SqlDbType.Int));
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(email, "@EMAIL", SqlDbType.VarChar));
            paramlist.Add(new Parameter(name, "@NAME", SqlDbType.VarChar));
            paramlist.Add(new Parameter(sidx, "@SIDX", SqlDbType.Int));
            paramlist.Add(new Parameter(eidx, "@EIDX", SqlDbType.Int));
            paramlist.Add(new Parameter(birth, "@BIRTH", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_ORDER_PRODUCT_S1", paramlist);
        }

        public int OrderPrStatusUpdates(int or_idx, string or_status)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(or_status, "@OR_STATUS", SqlDbType.VarChar));
            paramlist.Add(new Parameter(or_idx, "@OR_IDX", SqlDbType.Int));
            return ExcuteNonQuery("SP_ORDER_INFO_U", paramlist);
        }

        public int OrderStatusUpdate(string moid, string or_status)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(moid, "@MOID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(or_status, "@PTYPE", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_PAYINFO_U1", paramlist);
        }
        public int OrderStatusUpdate1(string moid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(moid, "@MOID", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_PAYINFO_U2", paramlist);
        }

        public int OrderBillKey(string moid, string billkey)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(moid, "@MOID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(billkey, "@BILL_KEY", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_BILL_INFO_I", paramlist);
        }

        public int OrderBillResult(string billkey, string tid, string recode, string remsg, string authcode, string pgauthdate, string pgauthtime)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(tid, "@TID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(billkey, "@BILL_KEY", SqlDbType.VarChar));
            paramlist.Add(new Parameter(recode, "@RESULTCODE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(remsg, "@RESULTMSG", SqlDbType.VarChar));
            paramlist.Add(new Parameter(authcode, "@AUTHCODE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(pgauthdate, "@PGAUTHDATE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(pgauthtime, "@PGAUTHTIME", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_BILL_INFO_RESULT_I", paramlist);
        }

        public DataSet SelectCurrentProduct()
        {
            return ExcuteToDataSet("SP_ORDER_PRODUCT_CURRENT_S", null);
        }

        public DataSet SelectBillList(DateTime sdate, DateTime edate)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(sdate, "@SDATE", SqlDbType.DateTime));
            paramlist.Add(new Parameter(edate, "@EDATE", SqlDbType.DateTime));
            return ExcuteToDataSet("SP_AUTO_TARGET_LIST", paramlist);
        }

        public DataSet SelectBilTargetlList()
        {
            return ExcuteToDataSet("SP_AUTO_TARGET_S", null);
        }

        public DataSet SelectBilTargetlList1()
        {
            return ExcuteToDataSet("SP_AUTO_TARGET_S1", null);
        }

        public int AutoBill_Insert(string userid, string ptype, float price, DateTime sdate, DateTime edate, string isuse, string moid, string billkey)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(ptype, "@PTYPE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(price, "@PRICE", SqlDbType.Float));
            paramlist.Add(new Parameter(sdate, "@SDATE", SqlDbType.DateTime));
            paramlist.Add(new Parameter(edate, "@EDATE", SqlDbType.DateTime));
            paramlist.Add(new Parameter(isuse, "@ISUSE", SqlDbType.VarChar));
            paramlist.Add(new Parameter(moid, "@MOID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(billkey, "@BILL_KEY", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_AUTO_TARGET_BILL_I", paramlist);
        }

        public DataSet Selectautobillresult(string sdate, string edate)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(sdate, "@SDATE", SqlDbType.DateTime));
            paramlist.Add(new Parameter(edate, "@EDATE", SqlDbType.DateTime));
            return ExcuteToDataSet("SP_AUTO_RESULT", paramlist);
        }


        public DataSet SelectBillInfo(string moid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(moid, "@MOID", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_MEBER_BILL_INFO_S", paramlist);
        }

        public int OrderStatusUpdateCancle(string moid)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(moid, "@MOID", SqlDbType.VarChar));
            paramlist.Add(new Parameter("C", "@PTYPE", SqlDbType.VarChar));
            return ExcuteNonQuery("SP_MEMBER_PAYINFO_U", paramlist);
        }

        public DataSet SelectTotalBill(string userid, string name, DateTime sdate, DateTime edate, string isCancle)
        {
            List<Parameter> paramlist = new List<Parameter>();
            paramlist.Add(new Parameter(userid, "@USERID", SqlDbType.VarChar));
            paramlist.Add(new Parameter(name, "@NAME", SqlDbType.VarChar));
            paramlist.Add(new Parameter(sdate, "@SDATE", SqlDbType.DateTime));
            paramlist.Add(new Parameter(edate, "@EDATE", SqlDbType.DateTime));
            paramlist.Add(new Parameter(isCancle, "@ISCANCLE", SqlDbType.VarChar));
            return ExcuteToDataSet("SP_TOTAL_BILL_S", paramlist);
        }
    }
}