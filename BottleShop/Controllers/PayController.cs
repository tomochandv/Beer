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
            //임시결재
            //new Dac_User().UserPay(AUser().USERID, "S", 20000, DateTime.Now, DateTime.Now.AddMonths(1), "Y");
            return View();
        }

        public ActionResult CardPayResult()
        { 
             //임시결재
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
            new Dac_User().UserPay(AUser().USERID, "R", 20000, DateTime.Now, DateTime.Now.AddMonths(1), "Y", ViewBag.Moid);
            return View();
        }

        public ActionResult MobileResult()
        {
            string result = Request["P_STATUS"] != null ? Request["P_STATUS"] : "";
            string moid = Request["P_NOTI"] != null ? Request["P_NOTI"] : "";
            string P_RMESG1 = Request["P_RMESG1"] != null ? Request["P_RMESG1"] : "";
            string P_REQ_URL = Request["P_REQ_URL"] != null ? Request["P_REQ_URL"] : "";
            string P_TID = Request["P_TID"] != null ? Request["P_TID"] : "";

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
                Response.Write(strResult);
                
            }
            return View();
        }




    }
}
