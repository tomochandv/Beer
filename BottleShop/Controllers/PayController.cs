using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BottleShop.Dac;
using System.Data;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using INIPAY41Lib;

namespace BottleShop.Controllers
{
    public class PayController : BaseController
    {
        //
        // GET: /Pay/

        public ActionResult Retutrn()
        {
            return View();
        }

        public ActionResult Pay()
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
            //임시결제
            //new Dac_User().UserPay(AUser().USERID, "S", 20000, DateTime.Now, DateTime.Now.AddMonths(1), "Y");
            return View();
        }

        public ActionResult CardPayResult()
        { 
             //임시결제
             //new Dac_User().UserPay(AUser().USERID, "S", 20000, DateTime.Now, DateTime.Now.AddMonths(1), "Y");
            return View();
        }  
  
        public JsonResult PromoPayCheck(string poro_code = "")
        {
            string message = string.Empty;
            DataTable dt = new Dac_Promo().CheckPromo(poro_code).Tables[0];
            if(dt.Rows.Count > 0)
            {
                if(dt.Rows[0]["ISUSE"].ToString() == "Y")
                {
                    message = "프로모션 코드가 이미 사용되었습니다.";
                }
                else
                {
                    new Dac_Promo().UsePromo(poro_code, AUser().USERID);
                    new Dac_User().UserPay(AUser().USERID, "P", 0, DateTime.Now, DateTime.Now.AddMonths(1), "Y", "");
                    message = "Y";
                }
            }
            else
            {
                message = "프로모션 코드가 유효하지 않습니다.";
            }
            return Json(message);
        }

        public ActionResult Mobile()
        {
            ViewBag.Moid = "order" + DateTime.Now.ToString("yyyyMMddhhmmssfff");
            ViewBag.IDD = new Security().Encription(ViewBag.USERID);
            ViewBag.timestmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            new Dac_User().UserPay(AUser().USERID, "R", 20000, DateTime.Now, DateTime.Now.AddMonths(1), "N", ViewBag.Moid);
            return View();
        }

        public ActionResult MobileResult()
        {
        
            string result = Request["P_STATUS"] != null ? Request["P_STATUS"] : "";
            string moid = Request["P_NOTI"] != null ? Request["P_NOTI"] : "";
            string P_RMESG1 = Request["P_RMESG1"] != null ? Request["P_RMESG1"] : "";
            string P_REQ_URL = Request["P_REQ_URL"] != null ? Request["P_REQ_URL"] : "";
            string P_TID = Request["P_TID"] != null ? Request["P_TID"] : "";
            string P_OID = Request["P_TID"] != null ? Request["P_OID"] : "";
         
            if (result == "00")
            {
                string strUri = P_REQ_URL;

                // POST, GET 보낼 데이터 입력
                StringBuilder dataParams = new StringBuilder();
                dataParams.Append("P_TID=" + P_TID);
                dataParams.Append("&P_MID=INIpayTest");

                // 요청 String -> 요청 Byte 변환
                byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(dataParams.ToString());

                /////////////////////////////////////////////////////////////////////////////////////
                /* POST */
                // HttpWebRequest 객체 생성, 설정
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUri);
                request.Method = "POST";    // 기본값 "GET"
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteDataParams.Length;


                // 요청 Byte -> 요청 Stream 변환
                Stream stDataParams = request.GetRequestStream();
                stDataParams.Write(byteDataParams, 0, byteDataParams.Length);
                stDataParams.Close();

                // 요청, 응답 받기
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // 응답 Stream 읽기
                Stream stReadData = response.GetResponseStream();
                StreamReader srReadData = new StreamReader(stReadData, Encoding.GetEncoding("UTF-8"));

                // 응답 Stream -> 응답 String 변환
                string strResult = srReadData.ReadToEnd();

                if (strResult.Contains("P_STATUS=00"))
                {
                    new Dac_Cart().OrderStatusUpdate(moid, "S");
                }
                else
                {
                    new Dac_Cart().OrderStatusUpdate(moid, "F");
                }
                OSIO.Log(strResult);
                Response.Write(strResult);
                
            }
            return View();
        }

        //isp 모듈
        public ActionResult MobileResult1()
        {
            string result = Request["P_STATUS"] != null ? Request["P_STATUS"] : "";
            string moid = Request["P_NOTI"] != null ? Request["P_NOTI"] : "";
            string P_RMESG1 = Request["P_RMESG1"] != null ? Request["P_RMESG1"] : "";
            string P_REQ_URL = Request["P_REQ_URL"] != null ? Request["P_REQ_URL"] : "";
            string P_TID = Request["P_TID"] != null ? Request["P_TID"] : "";
            string P_OID = Request["P_TID"] != null ? Request["P_OID"] : "";
            OSIO.Log(string.Format("{0} || {1} || {2} || {3} || {4} || {5}", result, moid, P_RMESG1, P_REQ_URL, P_TID, P_OID));
            if (result == "00")
            {

                new Dac_Cart().OrderStatusUpdate(P_OID, "S");
            
            }
            else
            {
                new Dac_Cart().OrderStatusUpdate(P_OID, "F");
            } 

    
            return View();
        }

        public ActionResult MobileResult2()
        {
            List<string> sss = new List<string>();
            string result = Request["resultcode"] != null ? Request["resultcode"] : "";
            string orderid = Request["orderid"] != null ? Request["orderid"] : "";
            string tid = Request["tid"] != null ? Request["tid"] : "";
            string billkey = Request["billkey"] != null ? Request["billkey"] : "";

            sss.Add(result);
            sss.Add(orderid);
            sss.Add(tid);
            sss.Add(billkey);
            string url = Url.Action("TxModule") + "?billkey=" + billkey + "&orderid=" + orderid + "&result=";
            if (result == "00")
            {
                new Dac_Cart().OrderBillKey(orderid, billkey);
                TxModuleMobile();
                //new Dac_Cart().OrderStatusUpdate(orderid, "S");
            }
            else
            {
                //new Dac_Cart().OrderStatusUpdate(orderid, "F");
            } 
            return View();
        }

        private string ComputeHash(string input)
        {
            System.Security.Cryptography.SHA256 algorithm = System.Security.Cryptography.SHA256Managed.Create();
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(String.Format("{0:x2}", hashedBytes[i]));
            }


            return sb.ToString();
        }

        public void TxModuleMobile()
        {
            List<PayInfoModel> list = DataType.ConvertToList<PayInfoModel>(new Dac_User().PayInfoUse(ViewBag.USERID));
            if (list != null && list.Count > 0)
            {
                DataSet dsBill = new Dac_Cart().SelectBillInfo(list[0].MOID);
                if (dsBill.Tables[0].Rows.Count > 0)
                {
                    OSIO.Log("set moid " + list[0].MOID);
                    bool results = INIrealbill("20000", dsBill.Tables[0].Rows[0]["BILL_KEY"].ToString());
                    if (results == true)
                    {
                        new Dac_Cart().OrderStatusUpdate(list[0].MOID, "S");
                        new Dac_Cart().OrderStatusUpdate1(list[0].MOID);
                    }
                    else
                    {
                        new Dac_Cart().OrderStatusUpdate(list[0].MOID, "F");
                    }
                }
                else
                {
                    //billing id 실패
                    new Dac_Cart().OrderStatusUpdate(list[0].MOID, "F");
                }
            }
        }

        public JsonResult TxModule()
        {
            string resultStr = "N";
            List<PayInfoModel> list = DataType.ConvertToList<PayInfoModel>(new Dac_User().PayInfoUse(ViewBag.USERID));
            if (list != null && list.Count > 0)
            {
                DataSet dsBill = new Dac_Cart().SelectBillInfo(list[0].MOID);
                if (dsBill.Tables[0].Rows.Count > 0)
                {
                    bool results = INIrealbill("20000", dsBill.Tables[0].Rows[0]["BILL_KEY"].ToString());
                    if (results == true)
                    {
                        new Dac_Cart().OrderStatusUpdate(list[0].MOID, "S");
                        new Dac_Cart().OrderStatusUpdate1(list[0].MOID);
                        resultStr = "S";
                    }
                    else
                    {
                        new Dac_Cart().OrderStatusUpdate(list[0].MOID, "F");
                    }
                }
                else
                {
                    //billing id 실패
                    new Dac_Cart().OrderStatusUpdate(list[0].MOID, "F");
                }
            }
            return Json(resultStr);
           
        }

        private bool INIrealbill(string price, string billkey)
        {
            bool result = false;
            try
            {
                
                /*1. 객체 생성*/
                INItx41 INIpay = new INItx41();

                /*2. 인스턴스 초기화*/
                int intPInst = INIpay.Initialize("");

                /*3. 거래 유형 설정*/
                INIpay.SetActionType(ref intPInst, "REQREALBILL");

                /*4. 정보 설정*/
                INIpay.SetField(ref intPInst, "pgid", "INIpayBill");//PG ID(고정)
                INIpay.SetField(ref intPInst, "spgip", "203.238.3.10");//예비 PG ID(고정)
                INIpay.SetField(ref intPInst, "uip", "203.238.3.10");//예비 PG ID(고정)
                INIpay.SetField(ref intPInst, "mid", "bthebottle");//상점 아이디
                INIpay.SetField(ref intPInst, "admin", "1111");//키 패스워드 (상점아이디에 따라 변경)
                INIpay.SetField(ref intPInst, "url", "http://thebottleshop.co.kr");//홈페이지 주소 (URL)
                INIpay.SetField(ref intPInst, "paymethod", "Card");//지불방법
                INIpay.SetField(ref intPInst, "cardquota", "00");//할부기간 (변경시에만 설정)
                INIpay.SetField(ref intPInst, "price", "20000");//가격 (변경시에만 설정)
                INIpay.SetField(ref intPInst, "currency", "WON");//화폐단위 (변경시에만 설정)
                //INIpay.SetField(ref intPInst, "quotainterest", "1");//무이자할부 여부(1:Yes, 0:No)
                INIpay.SetField(ref intPInst, "billkey", billkey); //BillKey


                /*01:비인증 (공인인증으로 인증받은 빌키를 이용하는 경우, 비밀번호 + 주민번호 필요없음.)
                 *00:인증 (공인인증서로 인증받지 않은 경우, 비밀번호 + 주민번호 필요)
                 */
                INIpay.SetField(ref intPInst, "authentification", "01"); //본인인증 여부

                /*5. 빌링 승인 요청*/
                INIpay.StartAction(ref intPInst);
                string resultCode = INIpay.GetResult(ref intPInst, "resultcode");
                /*6. 빌링 승인 결과*/
                //tid.Text = INIpay.GetResult(ref intPInst, "tid");
                //resultCode.Text = INIpay.GetResult(ref intPInst, "resultcode");
                //resultMsg.Text = INIpay.GetResult(ref intPInst, "resultmsg");
                //AuthCode.Text = INIpay.GetResult(ref intPInst, "authcode");
                //PGAuthDate.Text = INIpay.GetResult(ref intPInst, "pgauthdate");
                //PGAuthTime.Text = INIpay.GetResult(ref intPInst, "pgauthtime");

                /*6. 인스턴스 해제*/

                new Dac_Cart().OrderBillResult(billkey, INIpay.GetResult(ref intPInst, "tid"), INIpay.GetResult(ref intPInst, "resultcode"), INIpay.GetResult(ref intPInst, "resultmsg"),
                  INIpay.GetResult(ref intPInst, "authcode"), INIpay.GetResult(ref intPInst, "pgauthdate"), INIpay.GetResult(ref intPInst, "pgauthtime"));

                if (resultCode == "00")
                {
                    result = true;
                }

                INIpay.Destroy(ref intPInst);
                
              
            }
            catch(Exception ex)
            {
                OSIO.Log("Bill error : " + ex.Message);
            }

            return result;
        }


    }
}
