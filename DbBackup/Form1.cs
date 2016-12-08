using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace DbBackup
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
            timer.Change(0, 1000 * 60 * 60);    // dueTime 은 Timer 가 시작되기 전 대기 시간 (ms)
        }

        void Callback(object status)
        {
            // UI 에서 사용할 경우는 Cross-Thread 문제가 발생하므로 Invoke 또는 BeginInvoke 를 사용해서 마샬링을 통한 호출을 처리하여야 한다.
            BeginInvoke(new TimerEventFiredDelegate(Work));
        }
        private void Work()
        {
            if(DateTime.Now.Hour == 2)
            {
                try
                {
                    DataSet dsList = GetdataSet("select TABLE_NAME from information_schema.tables");
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        StringBuilder sw = new StringBuilder();
                        sw.AppendLine("===========================================================================================================================================");
                        sw.AppendLine("Date : " + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                        WriteFile(sw);
                        for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                        {
                            string tbName = dsList.Tables[0].Rows[i]["TABLE_NAME"].ToString();
                            DataSet ds = GetdataSet("select * from " + tbName);
                            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                WriteFile(CreateQuery(ds.Tables[0], tbName));
                            }
                        }
                    }
                    richTextBox1.Text += string.Format("======================================================\r\n{0}\r\n sucess.", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                }
                catch (Exception ex)
                {
                    richTextBox1.Text += string.Format("======================================================\r\n{0}\r\n{1}\r\n{2}\r\n{3}", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"),
                        ex.Message, ex.StackTrace, ex.Source);
                }
            }

        }

        private void WriteFile(StringBuilder text)
        {
            string temp;
            string FilePath = "C:\\dblog\\" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            try
            {
                DirectoryInfo dir = new DirectoryInfo("C:\\dblog");
                FileInfo fi = new FileInfo(FilePath);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                if (fi.Exists != true)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        sw.WriteLine(text.ToString());
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = System.IO.File.AppendText(FilePath))
                    {
                        sw.WriteLine(text.ToString());
                        sw.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                richTextBox1.Text += string.Format("======================================================\r\n{0}\r\n{1}\r\n{2}\r\n{3}", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"),
                    ex.Message, ex.StackTrace, ex.Source);
            }
        }

        private DataSet GetdataSet(string qry)
        {
            SqlConnection sqlCon = new SqlConnection();
            SqlCommand sqlCom = new SqlCommand();
            SqlDataAdapter sqlAdapeter = new SqlDataAdapter();
            string connectionString = "user id=tomochan80;data source=thebottleshop.co.kr;initial catalog=tomochan80;password=xhahcks80";
            DataSet ds = new DataSet();
            try
            {
                sqlCon.ConnectionString = connectionString;
                sqlCom.CommandText = qry;
                sqlCom.CommandType = CommandType.Text;
                sqlCom.Connection = sqlCon;
                sqlAdapeter.SelectCommand = sqlCom;
                sqlCon.Open();
                sqlAdapeter.Fill(ds);
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        private StringBuilder CreateQuery(DataTable dt, string name)
        {
            StringBuilder sw = new StringBuilder();
            sw.Append("-- " + name);
            sw.AppendLine();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string temp = string.Format("insert into {0} values(", name);
                for(int i=0; i < dt.Columns.Count; i++)
                {
                   if(i==0)
                   {
                       temp += string.Format("'{0}'", dt.Rows[j][i]);
                   }
                   else
                   {
                       temp += string.Format(", '{0}'", dt.Rows[j][i]);
                   }
                }
                temp += ");";
                sw.Append(temp);
                sw.AppendLine();
            }
            return sw;
        }

    }
}
