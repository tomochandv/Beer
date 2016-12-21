using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bottleshop.Api.Lib
{
    public class DataType
    {
        public static string String(object obj)
        {
            string txt = string.Empty;
            try
            {
                txt = obj.ToString();
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
            }
            return txt;
        }

        public static int Int(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return 0;
                }
                else
                {
                    return int.Parse(obj.ToString());
                }
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
                return 0;
            }
        }

        public static float Float(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return 0;
                }
                else
                {
                    return float.Parse(obj.ToString());
                }
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
                return 0;
            }
        }

        public static DateTime Datetime(object obj)
        {
            DateTime now = new DateTime();
            try
            {
                now = DateTime.Parse(obj.ToString());
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
            }
            return now;
        }
    }
}