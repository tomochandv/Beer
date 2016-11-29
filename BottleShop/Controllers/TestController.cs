using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace BottleShop.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {

            SmtpClient client = new SmtpClient("smtp.worksmobile.com", 587);
            client.UseDefaultCredentials = false; // 시스템에 설정된 인증 정보를 사용하지 않는다.
            client.EnableSsl = true;  // SSL을 사용한다.
            client.DeliveryMethod = SmtpDeliveryMethod.Network; // 이걸 하지 않으면 Gmail에 인증을 받지 못한다.
            client.Credentials = new System.Net.NetworkCredential("support@thebottleshop.co.kr", "ejqkxmftiq2");

            MailAddress from = new MailAddress("support@thebottleshop.co.kr", "the bottle shop", System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress("aka.tomochan@gmail.com");

            MailMessage message = new MailMessage(from, to);

            message.Body = "The bottleshop 비밀번호 찾기";
            message.Body += Environment.NewLine;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = "The bottleshop 비밀번호 찾기";
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
            ViewBag.pwd = new Security().Encription("1111");
            return View();
        }

    }
}
