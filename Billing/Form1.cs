using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Billing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        delegate void TimerEventFiredDelegate();

       

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Threading.Timer timer = new System.Threading.Timer(Callback);
            timer.Change(0, 1000 *60 *60);    // dueTime 은 Timer 가 시작되기 전 대기 시간 (ms)
        }

        void Callback(object status) {
            // UI 에서 사용할 경우는 Cross-Thread 문제가 발생하므로 Invoke 또는 BeginInvoke 를 사용해서 마샬링을 통한 호출을 처리하여야 한다.
            BeginInvoke(new TimerEventFiredDelegate(Work));
        }

        private void Work() 
        {
            if(DateTime.Now.Hour == 22)
            {
                string result = string.Empty;
                // UI 처리 작업...
                string key = HttpUtility.UrlEncode("PN8+jbwM+1zg3NLHHF2MI2suBCNel5m1OU8I2MdfwhUSy1lF+K5lxOb0O2OIhjWJYO3KhYjiMKO1u7HgDiDxayyZlVjSXAaMCT3XA19c9ztpGMiX/cdS/+HvrAokXanh9RL/behGZR3OpHGPmMaw4cdmi/fmWdQCMwJEowyC2c9iQMolmRyW7eEUE6uCcwuLOW43rlUfm2QVu2EtK3+djk5iDPQhWNIW7SABx84GEVE8Ormvd0FYbZD3Wc50TBDxGVYyzRtPAU0CNZ+orLSNGuVGvwM7dxoTtsd9VVOt2L0gPs0JZAcRnHX+iTashGRCrDppBDcsdrWjDrxqoHjrJA==");
                try
                {
                    result = string.Format("{0}  :::::   {1}", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"), HttpPostRequest("http://thebottleshop.co.kr/Test/BillApi", "key=" + key));
                }
                catch (Exception ex)
                {
                    result = string.Format("{0}  :::::   {1}", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"), ex.Message + ex.StackTrace);
                }
                richTextBox1.Text = richTextBox1.Text + "\r\n" + result;
            }
           
        }

        private string HttpPostRequest(string url, string postData)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            myHttpWebRequest.Method = "POST";

            byte[] data = Encoding.ASCII.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = data.Length;

            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            Stream responseStream = myHttpWebResponse.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

            string pageContent = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            responseStream.Close();

            myHttpWebResponse.Close();

            return pageContent;
        }



    }
}
