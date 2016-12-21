using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Bottleshop.Api.Lib
{
    public class Log
    {
        public Log()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(HttpContext.Current.Server.MapPath("~/Web.config")));
        }
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public void Error(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("StackTrace : {0}", ex.StackTrace));
            sb.AppendLine(string.Format("Message : {0}", ex.Message));
            logger.Error(sb.ToString());
        }

        public void Debug(string txt)
        {
            logger.Debug(txt);
        }

    }
}