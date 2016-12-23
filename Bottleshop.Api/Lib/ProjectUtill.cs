using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Bottleshop.Api.Lib
{
    public class ProjectUtill
    {
        public static  string GetCategoryName(int idx)
        {
            string txt = "";
            switch (idx)
            {
                case 1:
                    txt = "WINE";
                    break;
                case 2:
                    txt = "BEER";
                    break;
                case 3:
                    txt = "WHISKY";
                    break;
                case 6:
                    txt = "SPIRIT";
                    break;
            }
            return txt;
        }

        // SHA256  256bit 암호화
        public static string ComputeHash(string input)
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

        public static void SendEmail(string title, string body, string tomail)
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