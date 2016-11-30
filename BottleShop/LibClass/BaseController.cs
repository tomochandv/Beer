using BottleShop.Dac;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace BottleShop
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext context)
        {
            var userInCookie = Request.Cookies["bottleshop"];
            if (userInCookie != null)
            {
                AUser();
                CurrentPayInfo();
            }
            else
            {
                //context.Result = new RedirectResult("/User/Login");
            }
            //if (context.HttpContext.User != null && context.HttpContext.User.Identity.IsAuthenticated == true)
            //{
            //    FormsIdentity id = (FormsIdentity)context.HttpContext.User.Identity;
            //    if (context.HttpContext.User.Identity.IsAuthenticated)
            //    {
            //        AUser();
            //        CurrentPayInfo();
            //    }
            //    else
            //    {
            //        context.Result = new RedirectResult("/User/Login");
            //    }
            //}
          
        }

        /// <summary>
        /// 사용자 정보 알아오기
        /// </summary>
        /// <returns></returns>
        public UserModel AUser()
        {
            UserModel info = new UserModel();
            var userInCookie = Request.Cookies["bottleshop"];

            if (userInCookie != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                info = serializer.Deserialize<UserModel>(userInCookie.Value);
                ViewBag.NAME = HttpContext.Server.UrlDecode(info.NAME);
                ViewBag.USERID = info.USERID;
                ViewBag.EMAIL = info.EMAIL;
                ViewBag.TELL = info.TELL;
            }
            return info;

            //UserModel info = new UserModel();
            //FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
            //if (id.IsAuthenticated)
            //{
            //    JavaScriptSerializer serializer = new JavaScriptSerializer();
            //    info = serializer.Deserialize<UserModel>(id.Ticket.UserData);
            //    ViewBag.NAME = info.NAME;
            //    ViewBag.USERID = info.USERID;
            //    ViewBag.EMAIL = info.EMAIL;
            //    ViewBag.TELL = info.TELL;
            //}
            //return info;
        }

        public PayInfoModel CurrentPayInfo()
        {
            ViewBag.pay = "N";
            PayInfoModel model = new PayInfoModel();
            UserModel uinfo = AUser();
            List<PayInfoModel> list = DataType.ConvertToList<PayInfoModel>(new Dac_User().PayInfoUse(uinfo.USERID));
            if(list.Count > 0)
            {
                foreach (var data in list)
                {
                    if (data.SDATE <= DateTime.Now && DateTime.Now <= data.EDATE)
                    {
                        ViewBag.pay = "Y";
                        model = data;
                    }
                }
            }
            return model;
        }

        public void SendEmail(string title, string body, string tomail)
        {

            SmtpClient client = new SmtpClient("smtp.worksmobile.com", 587);
            client.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
            client.EnableSsl = true;  // SSL을 사용한다.
            client.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다.
            client.Credentials = new System.Net.NetworkCredential("support@thebottleshop.co.kr", "ejqkxmftiq2");

            MailAddress from = new MailAddress("support@thebottleshop.co.kr", "the bottle shop", System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress(tomail);

            MailMessage message = new MailMessage(from, to);

            message.Body = body;
            message.Body += Environment.NewLine;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = title;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            try
            {
                // 동기로 메일을 보낸다.
                client.Send(message);

                // Clean up.
                message.Dispose();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

    }
}
