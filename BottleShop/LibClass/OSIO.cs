using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BottleShop
{
    public class OSIO
    {

        /// <summary>
        /// 로그 기록
        /// </summary>
        /// <param name="str">로그내용
        public static void Log(string str)
        {

            string FilePath = HttpContext.Current.Server.MapPath("/Logs/") + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string DirPath = HttpContext.Current.Server.MapPath("/Logs/");
            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (di.Exists != true) Directory.CreateDirectory(DirPath);

                if (fi.Exists != true)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] : {1}", GetDateTime(), str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = System.IO.File.AppendText(FilePath))
                    {
                        temp = string.Format("[{0}] : {1}", GetDateTime(), str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private static string GetDateTime()
        {
            DateTime NowDate = DateTime.Now;
            return NowDate.ToString("yyyy-MM-dd HH:mm:ss") + ":" + NowDate.Millisecond.ToString("000");
        }


    }
}