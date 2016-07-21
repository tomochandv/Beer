using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace BottleShop.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
        //    SmtpClient client = new SmtpClient("smtp.works.naver.com", 465);
        //    client.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
        //    client.EnableSsl = true;  // SSL을 사용한다.
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다.
        //    client.Credentials = new System.Net.NetworkCredential("admin@thebottleshop.co.kr", "xhah0224");
        //    MailAddress from = new MailAddress("admin@thebottleshop.co.kr", "the bottle shop", System.Text.Encoding.UTF8);
        //    MailAddress to = new MailAddress("aka.tomochan@gmail.com");

        //    MailMessage message = new MailMessage(from, to);

        //    message.Body = "asdasdaasdasd";
        //    message.Body += Environment.NewLine;
        //    message.BodyEncoding = System.Text.Encoding.UTF8;
        //    message.Subject = "test";
        //    message.SubjectEncoding = System.Text.Encoding.UTF8;
        //    try
        //    {
        //        // 동기로 메일을 보낸다.
        //        client.Send(message);

        //        // Clean up.
        //        message.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.ToString());
        //    }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult coming()
        {
            return View();
        }

    }
}
