using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bottleshop.Api.Controllers
{
    public class PayController : BaseController
    {
        //
        // GET: /Pay/

        public ActionResult Index()
        {
            string userAgent = Request.UserAgent;
            bool isMobile = false;
            String[] browser = { "iphone", "ipod", "ipad", "android", "blackberry", "windows ce", "nokia", "webos", "opera mini", "sonyericsson", "opera mobi", "iemobile", "windows phone" };
            for (int i = 0; i < browser.Length; i++)
            {
                if (userAgent.ToLower().Contains(browser[i]) == true)
                {
                    isMobile = true;
                    break;
                }
            }
            ViewBag.mobile = isMobile == true ? "M" : "D";
            return View();
        }

        public ActionResult Desk()
        {
            var filter = Builders<Member>.Filter.Eq("Uid", AUser().Uid);
            var member = MongodbHelper.FindOne<Member>(filter, "Member");
            return View(member);
        }

        public ActionResult DeskTopReturn()
        {
            string result = "N";
            MemberPayInfo payInfo = StartINIStdReturn();
            MemberBillingInfo billInfo = new MemberBillingInfo();
            if(payInfo != null)
            {
                if (payInfo.BillingUse == true && payInfo.IsAuth == true)
                {
                    try
                    {
                        //기존에 빌링키 내역이있으면 업데이트 아니면 인서트
                        var filter = Builders<MemberPayInfo>.Filter.Eq("Uid", AUser().Uid);
                        var model = MongodbHelper.FindOne<MemberPayInfo>(filter, "MemberPayInfo");
                        if (model != null)
                        {
                            var update = Builders<MemberPayInfo>.Update.Set("BillingKey", payInfo.BillingKey).Set("Orderid", payInfo.Orderid).Set("InDate", payInfo.InDate)
                                .Set("IsAuth", payInfo.IsAuth).Set("BillingUse", payInfo.BillingUse);
                            MongodbHelper.Update<MemberPayInfo>(filter, update, "MemberPayInfo");
                        }
                        else
                        {
                            MongodbHelper.InsertOneModel<MemberPayInfo>(payInfo, "MemberPayInfo");
                        }

                        //Tx모듈 실행후 다시 읽어서 업데이트
                        List<MemberBillingInfo> listBillInfo = new List<MemberBillingInfo>();
                        var model1 = MongodbHelper.FindOne<MemberPayInfo>(filter, "MemberPayInfo");

                        TxBilling bill = new TxBilling("20000", payInfo.BillingKey);
                        billInfo.BillingKey = payInfo.BillingKey;
                        billInfo.BillType = "S";
                        billInfo.EndDate = DateTime.Now.AddMonths(1);
                        billInfo.StartDate = DateTime.Now;
                        billInfo.InDate = DateTime.Now;
                        billInfo.InicisId = bill.tid;
                        model1.InicisId = bill.tid;
                        billInfo.Orderid = payInfo.Orderid;
                        billInfo.PgauthDate = bill.pgauthdate;
                        billInfo.PgauthTime = bill.pgauthtime;
                        billInfo.ResultCode = bill.resultcode;
                        billInfo.ResultMsg = bill.resultmsg;
                        billInfo.Use = bill.result;
                        model1.BillingUse = bill.result;
                        if (model1.billList != null && model1.billList.Count > 0)
                        {
                            foreach (var data in model1.billList)
                            {
                                data.Use = false;
                            }
                            listBillInfo = model1.billList;

                        }
                        listBillInfo.Add(billInfo);
                        model1.billList = listBillInfo;
                        MongodbHelper.ReplaceOne<MemberPayInfo>(filter, model1, "MemberPayInfo");
                        result = "S";
                    }
                    catch(Exception ex)
                    {
                        new Log().Error(ex);
                    }
                }
            }
            ViewBag.result = result;
            return View();
        }

        public ActionResult Mobile()
        {
            ViewBag.Moid = "mbthebottle_" + DateTime.Now.ToString("yyyyMMddhhmmssfff");
            ViewBag.timestmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            //mobile 은 시작 전에 정보를 가입력해야함.(사이트 전환의 이유)
            var filter1 = Builders<MemberPayInfo>.Filter.Eq("Uid", AUser().Uid);
            var info = MongodbHelper.FindOne<MemberPayInfo>(filter1, "MemberPayInfo");
            if(info == null)
            {
                info.IsAuth = false;
                info.Orderid = ViewBag.Moid;
                info.Uid = AUser().Uid;
                info.BillingUse = false;
                MongodbHelper.InsertOneModel<MemberPayInfo>(info, "MemberPayInfo");
            }
            else
            {
                info.IsAuth = false;
                info.Orderid = ViewBag.Moid;
                info.Uid = AUser().Uid;
                info.BillingUse = false;
                MongodbHelper.ReplaceOne<MemberPayInfo>(filter1, info, "MemberPayInfo");
            }
            var filter = Builders<Member>.Filter.Eq("Uid", AUser().Uid);
            var member = MongodbHelper.FindOne<Member>(filter, "Member");

            return View(member);
        }

        public ActionResult MobileReturn()
        {
            string result = Request["resultcode"] != null ? Request["resultcode"] : "";
            string orderid = Request["orderid"] != null ? Request["orderid"] : "";
            string tid = Request["tid"] != null ? Request["tid"] : "";
            string billkey = Request["billkey"] != null ? Request["billkey"] : "";

            if (orderid != "" && result == "00")
            {
                MemberBillingInfo billInfo = new MemberBillingInfo();
                List<MemberBillingInfo> listBillInfo = new List<MemberBillingInfo>();

                var filter1 = Builders<MemberPayInfo>.Filter.Eq("Orderid", orderid);
                var info = MongodbHelper.FindOne<MemberPayInfo>(filter1, "MemberPayInfo");
                if(info != null)
                {
                    info.BillingKey = billkey;
                    info.InicisId = tid;
                    info.Orderid = orderid;
                    info.InDate = DateTime.Now;
                    info.IsAuth = true;

                    TxBilling bill = new TxBilling("20000", billkey);
                    billInfo.BillingKey = billkey;
                    billInfo.BillType = "S";
                    billInfo.EndDate = DateTime.Now.AddMonths(1);
                    billInfo.StartDate = DateTime.Now;
                    billInfo.InDate = DateTime.Now;
                    billInfo.InicisId = bill.tid;
                    info.InicisId = bill.tid;
                    billInfo.Orderid = orderid;
                    billInfo.PgauthDate = bill.pgauthdate;
                    billInfo.PgauthTime = bill.pgauthtime;
                    billInfo.ResultCode = bill.resultcode;
                    billInfo.ResultMsg = bill.resultmsg;
                    billInfo.Use = bill.result;
                    info.BillingUse = bill.result;

                    if (info.billList != null && info.billList.Count > 0)
                    {
                        foreach (var data in info.billList)
                        {
                            data.Use = false;
                        }
                        listBillInfo = info.billList;

                    }
                    listBillInfo.Add(billInfo);
                    info.billList = listBillInfo;
                    MongodbHelper.ReplaceOne<MemberPayInfo>(filter1, info, "MemberPayInfo");
                    result = "S";

                }
            }
            ViewBag.result = result;
            return View();
        }

        public JsonResult PromotionPayment(string code = "")
        {
            string message = string.Empty;
            var filter = Builders<Promotion>.Filter.Eq("PromotionCode", code);
            Promotion promotion = MongodbHelper.FindOne<Promotion>(filter, "Promotion");
            if (promotion != null)
            {
                if (!promotion.Use)
                {
                    var filter_payinfo = Builders<MemberPayInfo>.Filter.Eq("Uid", AUser().Uid);
                    var memberPayInfo = MongodbHelper.FindOne<MemberPayInfo>(filter_payinfo, "MemberPayInfo");

                    List<MemberBillingInfo> list_memberBillInfo = new List<MemberBillingInfo>();

                    MemberBillingInfo billInfo = new MemberBillingInfo();
                    billInfo.BillingKey = "PROMOTION";
                    billInfo.InicisId = code;
                    billInfo.BillType = "P";
                    billInfo.StartDate = DateTime.Now;
                    billInfo.EndDate = DateTime.Now.AddMonths(1);
                    billInfo.InDate = DateTime.Now;
                    billInfo.Use = true;

                    if(memberPayInfo == null)
                    {
                        list_memberBillInfo.Add(billInfo);

                        MemberPayInfo data = new MemberPayInfo();
                        data.BillingKey = "";
                        data.BillingUse = false;
                        data.InDate = DateTime.Now;
                        data.InicisId = "";
                        data.IsAuth = true;
                        data.Orderid = "";
                        data.Uid = AUser().Uid;
                        data.billList = list_memberBillInfo;

                        MongodbHelper.InsertOneModel<MemberPayInfo>(data, "MemberPayInfo");
                    }
                    else
                    {
                        memberPayInfo.billList.Add(billInfo);
                        MongodbHelper.ReplaceOne<MemberPayInfo>(filter_payinfo, memberPayInfo, "MemberPayInfo");
                    }
                    //Promotion Update
                    var filter_promotion = Builders<Promotion>.Filter.Eq("Id",promotion.Id);
                    var update_promotion = Builders<Promotion>.Update.Set("Use", true).Set("Uid", AUser().Uid).Set("UseDate", DateTime.Now);
                    MongodbHelper.Update<Promotion>(filter, update_promotion, "Promotion");
                }
                else
                {
                    message = "프로모션 코드가 이미 사용되었습니다.";
                }
            }
            else
            {
                message = "프로모션 코드가 유효하지 않습니다.";
            }
            return Json(message);
        }

        #region Inicis Billng Key 로직
        private MemberPayInfo StartINIStdReturn()
        {
            MemberPayInfo payInfo = new MemberPayInfo();
            payInfo.Uid = AUser().Uid;
            payInfo.InDate = DateTime.Now;
            payInfo.IsAuth = false;
            payInfo.BillingUse = false;

            string url = string.Empty;
            string paybill = string.Empty;
            string result = "F";
            string orderid = string.Empty;
            NameValueCollection parameters = Request.Params;
            IEnumerator enumerator = parameters.GetEnumerator();
            StringBuilder sb = new StringBuilder("paramMap : ");
            while (enumerator.MoveNext())
            {
                // get the current query parameter
                string key = enumerator.Current.ToString();
                // insert the parameter into the url
                sb.Append(string.Format("{0}={1}&", key, HttpUtility.UrlEncode(parameters[key])));
            }
            //#####################
            // 인증이 성공일 경우만
            //#####################
            if ("0000".Equals(parameters["resultCode"]))
            {
                //############################################
                // 1.전문 필드 값 설정(***가맹점 개발수정***)
                //############################################
                String mid = parameters.Get("mid");					        // 가맹점 ID 수신 받은 데이터로 설정
                String signKey = "cnl4dktVbk5YcnYxeGdOT0JCM0RIZz09";	    // 가맹점에 제공된 키(이니라이트키) (가맹점 수정후 고정) !!!절대!! 전문 데이터로 설정금지
                string timeTemp = "" + DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
                string[] artime = timeTemp.Split('.');
                String timestamp = artime[0];
                String charset = "UTF-8";								    // 리턴형식[UTF-8,EUC-KR](가맹점 수정후 고정)
                String format = "JSON";								        // 리턴형식[XML,JSON,NVP](가맹점 수정후 고정)
                String authToken = parameters.Get("authToken");			    // 취소 요청 tid에 따라서 유동적(가맹점 수정후 고정)
                String authUrl = parameters.Get("authUrl");				    // 승인요청 API url(수신 받은 값으로 설정, 임의 세팅 금지)
                String netCancel = parameters.Get("netCancelUrl");			// 망취소 API url(수신 받은 값으로 설정, 임의 세팅 금지)
                String mKey = ProjectUtill.ComputeHash(signKey);	                        // 가맹점 확인을 위한 signKey를 해시값으로 변경 (SHA-256방식 사용)
                //#####################
                // 2.signature 생성
                //#####################
                String signParam = "authToken=" + @authToken + "&timestamp=" + timestamp;
                String signature = ProjectUtill.ComputeHash(signParam);
                //#####################
                // 3.API 요청 전문 생성
                //#####################
                System.Collections.Generic.Dictionary<String, String> authMap = new System.Collections.Generic.Dictionary<String, String>();
                authMap.Add("mid", mid);			            // 필수
                authMap.Add("authToken", HttpUtility.UrlEncode(authToken));	// 필수 - 반드시 urlencode 해서 전달.
                authMap.Add("timestamp", timestamp);	            // 필수
                authMap.Add("signature", signature);	            // 필수            
                authMap.Add("charset", charset);		            // default=UTF-8
                authMap.Add("format", format);		            // default=XML
                authMap.Add("mkey", mKey);		            // default=XML
                //Console.WriteLine("##승인요청 API 요청##");
                try
                {
                    //#####################
                    // 4.API 통신 시작
                    //#####################
                    String authResultString = "";
                    authResultString = processHTTP(authMap, authUrl);
                    //############################################################
                    //5.API 통신결과 처리(***가맹점 개발수정***)
                    //############################################################
                    //Response.Write("## 승인 API 결과 ##");
                    String strReplace = authResultString.Replace(",", "&").Replace(":", "=").Replace("\"", "").Replace(" ", "").Replace("\n", "").Replace("}", "").Replace("{", "");
                    System.Collections.Generic.Dictionary<string, string> resultMap = parseStringToMap(strReplace);         //문자열을 MAP형식으로 파싱
                    /*************************  결제보안 추가 START ****************************/
                    Dictionary<String, String> secureMap = new Dictionary<String, String>();
                    secureMap.Add("mid", mid);							//mid
                    secureMap.Add("tstamp", timestamp);					//timestemp
                    secureMap.Add("MOID", resultMap["MOID"]);			//MOID
                    secureMap.Add("TotPrice", resultMap["TotPrice"]);		//TotPrice
                    // signature 데이터 생성 
                    String secureSignature = makeSignatureAuth(secureMap);
                    /*************************  결제보안 추가 END ****************************/
                    if ("0000".Equals((resultMap.ContainsKey("resultCode") ? resultMap["resultCode"] : "null")) && secureSignature.Equals(resultMap["authSignature"]))	//결제보안 추가
                    {
                        /*****************************************************************************
                        * 여기에 가맹점 내부 DB에 결제 결과를 반영하는 관련 프로그램 코드를 구현한다.  
				   
                          [중요!] 승인내용에 이상이 없음을 확인한 뒤 가맹점 DB에 해당건이 정상처리 되었음을 반영함
                                  처리중 에러 발생시 망취소를 한다.
                        ******************************************************************************/
                        System.Collections.Generic.Dictionary<string, string> checkMap = new System.Collections.Generic.Dictionary<string, string>();
                        checkMap.Add("mid", mid);			// 필수
                        checkMap.Add("authToken", HttpUtility.UrlEncode(authToken));	// 필수 - 반드시 urlencode 해서 전달.
                        checkMap.Add("applDate", (resultMap.ContainsKey("applDate") ? resultMap["applDate"] : "null"));		// 필수					
                        checkMap.Add("applTime", (resultMap.ContainsKey("applTime") ? resultMap["applTime"] : "null"));		// 필수	
                        checkMap.Add("timestamp", timestamp);	// 필수
                        checkMap.Add("signature", signature);	// 필수            
                        checkMap.Add("charset", charset);		// default=UTF-8
                        checkMap.Add("format", format);		// default=XML
                        payInfo.Orderid = resultMap.ContainsKey("MOID") ? resultMap["MOID"] : "";
                        payInfo.BillingKey = resultMap.ContainsKey("CARD_BillKey") ? resultMap["CARD_BillKey"] : "";
                        payInfo.BillingUse = true;
                        payInfo.IsAuth = true;
                    }
                    else
                    {
                        payInfo.Orderid = resultMap.ContainsKey("MOID") ? resultMap["MOID"] : "";
                        payInfo.BillingKey = "";
                    }
                }
                catch (Exception ex)
                {
                    new Log().Error(ex);
                }
            }
            return payInfo;
        }
        private string processHTTP(System.Collections.Generic.Dictionary<string, string> mapParam, string url)
        {
            string postData = "";
            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in mapParam)
            {
                string param = kvp.Key + "=" + kvp.Value + "&";
                postData += param;
            }
            postData = postData.Substring(0, postData.Length - 1);
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            System.IO.Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            System.Net.WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((System.Net.HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
        System.Collections.Generic.Dictionary<string, string> parseStringToMap(string text)
        {
            System.Collections.Generic.Dictionary<string, string> retMap = new System.Collections.Generic.Dictionary<string, string>();
            string[] arText = text.Split('&');
            for (int i = 0; i < arText.Length; i++)
            {
                string[] arKeyVal = arText[i].Split('=');
                retMap.Add(arKeyVal[0], arKeyVal[1]);
            }
            return retMap;
        }
        private string makeSignatureAuth(Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                throw new Exception("Parameters can not be empty.");
            }
            string stringToSign = "";															//반환용 text
            string mid = parameters["mid"];                                                //mid
            string tstamp = parameters["tstamp"];												//auth timestamp
            string MOID = parameters["MOID"];												//OID
            string TotPrice = parameters["TotPrice"];											//total price
            string tstampKey = parameters["tstamp"].Substring(parameters["tstamp"].Length - 1);	// timestamp 마지막 자리 1자리 숫자
            switch (uint.Parse(tstampKey))
            {
                case 1:
                    stringToSign = "MOID=" + MOID + "&mid=" + mid + "&tstamp=" + tstamp;
                    break;
                case 2:
                    stringToSign = "MOID=" + MOID + "&tstamp=" + tstamp + "&mid=" + mid;
                    break;
                case 3:
                    stringToSign = "mid=" + mid + "&MOID=" + MOID + "&tstamp=" + tstamp;
                    break;
                case 4:
                    stringToSign = "mid=" + mid + "&tstamp=" + tstamp + "&MOID=" + MOID;
                    break;
                case 5:
                    stringToSign = "tstamp=" + tstamp + "&mid=" + mid + "&MOID=" + MOID;
                    break;
                case 6:
                    stringToSign = "tstamp=" + tstamp + "&MOID=" + MOID + "&mid=" + mid;
                    break;
                case 7:
                    stringToSign = "TotPrice=" + TotPrice + "&mid=" + mid + "&tstamp=" + tstamp;
                    break;
                case 8:
                    stringToSign = "TotPrice=" + TotPrice + "&tstamp=" + tstamp + "&mid=" + mid;
                    break;
                case 9:
                    stringToSign = "TotPrice=" + TotPrice + "&MOID=" + MOID + "&tstamp=" + tstamp;
                    break;
                case 0:
                    stringToSign = "TotPrice=" + TotPrice + "&tstamp=" + tstamp + "&MOID=" + MOID;
                    break;
            }
            string signature = ProjectUtill.ComputeHash(stringToSign);            // sha256 처리하여 hash 암호화
            return signature;
        } 
        #endregion

    }
}
