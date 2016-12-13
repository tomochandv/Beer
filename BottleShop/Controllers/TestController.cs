using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using BottleShop.Dac;
using INIPAY41Lib;

namespace BottleShop.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index(string pwd ="")
        {

            //SmtpClient client = new SmtpClient("smtp.worksmobile.com", 587);
            //client.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
            //client.EnableSsl = true;  // SSL을 사용한다.
            //client.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다.
            //client.Credentials = new System.Net.NetworkCredential("support@thebottleshop.co.kr", "ejqkxmftiq2");

            //MailAddress from = new MailAddress("support@thebottleshop.co.kr", "the bottle shop", System.Text.Encoding.UTF8);
            //MailAddress to = new MailAddress("aka.tomochan@gmail.com");

            //MailMessage message = new MailMessage(from, to);

            //message.Body = "The bottleshop 비밀번호 찾기";
            //message.Body += Environment.NewLine;
            //message.BodyEncoding = System.Text.Encoding.UTF8;
            //message.Subject = "The bottleshop 비밀번호 찾기";
            //message.SubjectEncoding = System.Text.Encoding.UTF8;
            //try
            //{
            //    // 동기로 메일을 보낸다.
            //    client.Send(message);

            //    // Clean up.
            //    message.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    //MessageBox.Show(ex.ToString());
            //}
            ViewBag.sss = new Security().Description("DC0Xdral4s2mJcoNietWvdqDu7pYPIyKokFW/NPMsx3WDPEvB8j4bJhGYdTabytH2G8CcEoZhcQ4bmSSngo7NpssTvVdOPRnBt+Zg47hez1Dlov2Hi5glKQY0S/yoHFDFGUeTfaX/CM2dtfE3CIGk9wP4W3M94X8kmDyMdHHXkHXaPt7Xq59WXfUN5BOi/t3tG0Xr5JdYNiiEdbr4xmh3mFGlNUmIdohq6OVhI10CF0wsDv5Q5q1OW4fMxIIk9TPgP32KBe2UtIqXGo/qxjvIBYbTHBBfFJG/NfD0e2iwk4mq2ff9M1n74M/qJXE/+aIUUZZBF5Zpg7NaCo7UG7xPA==");
            ViewBag.pwd = new Security().Encription(pwd == "" ? "0" : pwd);
            return View();
        }
        public JsonResult BillApi(string key)
        {
            string result = "N";
            if (new Security().Description(key) == "apiqwe123!@#")
            {
                int count = 0;
                DataSet ds = new Dac_Cart().SelectBilTargetlList1();
                List<AdminBillListModel> listModel = DataType.ConvertToList<AdminBillListModel>(ds.Tables[0]);
                int target = listModel.Count;
                if (listModel != null && listModel.Count > 0)
                {
                    foreach (var data in listModel)
                    {
                        //TX
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
                        INIpay.SetField(ref intPInst, "billkey", data.BILL_KEY); //BillKey
                        /*01:비인증 (공인인증으로 인증받은 빌키를 이용하는 경우, 비밀번호 + 주민번호 필요없음.)
                         *00:인증 (공인인증서로 인증받지 않은 경우, 비밀번호 + 주민번호 필요)
                         */
                        INIpay.SetField(ref intPInst, "authentification", "01"); //본인인증 여부
                        /*5. 빌링 승인 요청*/
                        INIpay.StartAction(ref intPInst);
                        string resultCode = INIpay.GetResult(ref intPInst, "resultcode");
                        if (resultCode == "00")
                        {
                            count++;
                            result = "Y";
                        }
                        new Dac_Cart().AutoBill_Insert(data.USERID, "S", 20000, data.SDATE.AddDays(1), data.EDATE.AddMonths(1), result, INIpay.GetResult(ref intPInst, "tid"), data.BILL_KEY);
                        new Dac_Cart().OrderBillResult(data.BILL_KEY, INIpay.GetResult(ref intPInst, "tid"), INIpay.GetResult(ref intPInst, "resultcode"), INIpay.GetResult(ref intPInst, "resultmsg"),
                            INIpay.GetResult(ref intPInst, "authcode"), INIpay.GetResult(ref intPInst, "pgauthdate"), INIpay.GetResult(ref intPInst, "pgauthtime"));
                        INIpay.Destroy(ref intPInst);
                    }
                }
                result = "target : " + target.ToString() + " || Sucess : " + count.ToString();
            }
            else
            {
                result = "Fuck You";
            }
            return Json(result);
        }

        public JsonResult Test()
        {
            return Json("YES");
        }
    }
}
