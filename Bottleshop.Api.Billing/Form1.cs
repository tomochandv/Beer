using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Bottleshop.Api.Billing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        delegate void TimerEventFiredDelegate();
        private void btnRun_Click(object sender, EventArgs e)
        {
            System.Threading.Timer timer = new System.Threading.Timer(Callback);
            timer.Change(0, 1000 * 60 * 60);    // dueTime 은 Timer 가 시작되기 전 대기 시간 (ms)
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string url = string.Format("{0}ApiTx", ConfigurationManager.AppSettings["Domain"]);
            textBox1.Text = url;
        }

        void Callback(object status)
        {
            // UI 에서 사용할 경우는 Cross-Thread 문제가 발생하므로 Invoke 또는 BeginInvoke 를 사용해서 마샬링을 통한 호출을 처리하여야 한다.
            BeginInvoke(new TimerEventFiredDelegate(Work));
        }

        private void Work()
        {
            StringBuilder sw = new StringBuilder();
            //if (DateTime.Now.Hour == 22)
            //{
                // UI 처리 작업...
                try
                {
                    sw.AppendLine("/*     Start     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "     */");
                    dynamic jsonDe = JsonConvert.DeserializeObject(HttpPostRequest(textBox1.Text, "key=" + key));
                    foreach(var data in jsonDe)
                    {
                        sw.AppendLine(string.Format("{0}: {1} ", data.Key, data.Value));
                    }
                }
                catch (Exception ex)
                {
                    sw.AppendLine(string.Format("{0}", ex.Message + ex.StackTrace));
                }
                sw.AppendLine("/*     end    */");
                richTextBox1.Text = sw.ToString();
           //}

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

        private string key = HttpUtility.UrlEncode("kyPNdqpBoYhIl/GoP9bsZ5y7UUNXCowFWBVmjKstIpNs7hr/j5JJPqjp9zxq9E6pHSs3N5Zz+XfLZXUJKi1oj/v2X/m9hp+ViJXp7camA6CyXYClfh/PgrwTbuR1KqmNgM2fpmdKtlyp/Dm9PGI1rsZ7PajzuunLc7gkXX7xxlQjery79nDCFV1HKBO0rwGpITX8KO4jdkkrdrkje3nVeE5SLRl4+DvJcoWf3AsJUUlwyOqxVnVGhIpdT0XmvyQxRX+ESdeXqU2kk7a6ufAaFTZpEBfxugvCtFcEPPrugzjLUBUgUskko7vxT5AApJikJJmsHewtTo0YBO5ZQO/PnQ==");
       
    }

    public class ResultModel
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
}
