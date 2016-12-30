using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using BottleShop.Api.Lib;
using MongoDB.Driver;

namespace Bottleshop.Api.Controllers
{
    public class ApiTxController : Controller
    {
        [HttpPost]
        public JsonResult Index(string key = "")
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            int target = 0;
            int sucess = 0;
            int fail = 0;
            if(key != "")
            {
                if (new Security().Description(key) == ConfigurationManager.AppSettings["ApiToken"])
                {
                    //1. Select Target
                    var filter = Builders<MemberPayInfo>.Filter.Eq("BillingUse", true);
                    var memberPayinfoCollection = MongodbHelper.Find<MemberPayInfo>(filter, "MemberPayInfo");
                    if(memberPayinfoCollection != null && memberPayinfoCollection.Count > 0)
                    { 
                        foreach(var data in memberPayinfoCollection)
                        {
                            MemberBillingInfo billInfo = new MemberBillingInfo();
                            foreach(var data1 in data.billList)
                            {
                                if(data1.BillType == "S" && data1.Use == true && data1.EndDate.AddDays(1).ToShortDateString() == DateTime.Now.ToShortDateString())
                                {
                                    target++;

                                    #region TX
                                    TxBilling bill = new TxBilling("20000", data.BillingKey);
                                    if (bill.result)
                                    {
                                        billInfo.BillingKey = data.BillingKey;
                                        billInfo.BillType = "S";
                                        billInfo.EndDate = DateTime.Now.AddMonths(1);
                                        billInfo.StartDate = DateTime.Now;
                                        billInfo.InDate = DateTime.Now;
                                        billInfo.InicisId = bill.tid;
                                        billInfo.Orderid = data.Orderid;
                                        billInfo.PgauthDate = bill.pgauthdate;
                                        billInfo.PgauthTime = bill.pgauthtime;
                                        billInfo.ResultCode = bill.resultcode;
                                        billInfo.ResultMsg = bill.resultmsg;
                                        billInfo.Use = bill.result;

                                        data.InicisId = bill.tid;
                                        data.IsAuth = true;
                                        data.BillingUse = true;
                                        sucess++;
                                    }
                                    else
                                    {
                                        fail++;
                                    } 
                                    #endregion
                                    
                                }
                                data1.Use = false;
                            }

                            if(billInfo != null && billInfo.Use)
                            {
                                var filters = Builders<MemberPayInfo>.Filter.Eq("Id", data.Id);
                                data.billList.Add(billInfo);
                                MongodbHelper.ReplaceOne<MemberPayInfo>(filters, data, "MemberPayInfo");
                            }
                        }
                    }


                    dic.Add("Target", target);
                    dic.Add("Sucess", sucess);
                    dic.Add("Fail", fail);

                }
            }
            return Json(dic.ToList(), JsonRequestBehavior.AllowGet);
        }



    }
}
